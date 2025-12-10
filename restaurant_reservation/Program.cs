using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using restaurant_reservation.Data.Abstract;
using restaurant_reservation.Data.Concrete;
using restaurant_reservation.Models;
using restaurant_reservation.Services.Abstract;
using restaurant_reservation.Services.Concrete;
using restaurant_reservation_api.Data;
using restaurant_reservation_api.Hubs;
using restaurant_reservation_api.Mapping;
using restaurant_reservation_api.Messaging;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//Add services to the container.
builder.Services.AddDbContext<RestaurantContext>(options => 
    options.UseSqlite("Data Source=restaurant_reservation"));

builder.Services.AddSignalR();
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379";
    options.InstanceName = "RestaurantReservation_";
});

// RabbitMQ Services
builder.Services.AddSingleton<IRabbitMQPublisher, RabbitMQPublisher>();

builder.Services.AddHostedService<EmailConsumerService>();

builder.Services.AddIdentity<AppUser, AppRole>()
   .AddEntityFrameworkStores<RestaurantContext>(); // This line requires the above using directive  
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireDigit = false;

    options.User.RequireUniqueEmail = true;

    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
});

// JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            builder.Configuration.GetSection("AppSettings:SecretKey").Value ?? "")),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS politikasýný ekle
builder.Services.AddCors(options =>
{
    options.AddPolicy("SignalR",
        builder => builder.WithOrigins("https://localhost:7100")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});

// Repositories
builder.Services.AddScoped<ITableRepository, TableRepository>();
builder.Services.AddScoped<IDrinkRepository, DrinkRepository>();
builder.Services.AddScoped<IFoodRepository, FoodRepository>();
builder.Services.AddScoped<IMenuRepository, MenuRepository>();
builder.Services.AddScoped<IGuestReservationRepository, GuestReservationRepository>();
builder.Services.AddScoped<IUserReservationRepository, UserReservationRepository>();

// Services
builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<IDrinkService, DrinkService>();
builder.Services.AddScoped<IFoodService, FoodService>();
builder.Services.AddScoped<ITableService, TableService>();
builder.Services.AddScoped<IUserReservationService, UserReservationService>();
builder.Services.AddScoped<IGuestReservationService, GuestReservationService>();
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// CORS politikasýný etkinleþtir
app.UseCors("SignalR");

app.UseAuthorization();


app.MapControllers();
app.MapHub<AdminHub>("/adminHub");
app.MapHub<UserNotificationHub>("/userNotificationHub");

app.Run();
