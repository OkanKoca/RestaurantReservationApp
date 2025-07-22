using Microsoft.EntityFrameworkCore;
using restaurant_reservation;
using restaurant_reservation.Data.Abstract;
using restaurant_reservation.Data.Concrete;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<RestaurantContext>(options => 
    options.UseSqlite("Data Source=restaurant_reservation"));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ITableRepository, TableRepository>();
builder.Services.AddScoped<IDrinkRepository, DrinkRepository>();
builder.Services.AddScoped<IFoodRepository, FoodRepository>();
builder.Services.AddScoped<IMenuRepository, MenuRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
