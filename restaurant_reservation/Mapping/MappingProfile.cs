using AutoMapper;
using restaurant_reservation_api.Models;
using restaurant_reservation_api.Dto;
using restaurant_reservation.Models;
using restaurant_reservation.Dto;


namespace restaurant_reservation_api.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
        // ========== UserReservation Mappings ========= =
  
            // UserReservation -> UserReservationDto
   CreateMap<UserReservation, UserReservationDto>()
     .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.Id : 0))
      .ForMember(dest => dest.ReservationHour, opt => opt.MapFrom(src => src.ReservationDate.Hour.ToString()))
           .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
        .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));

     // Admin view of user reservations with extra customer info
   CreateMap<UserReservation, AdminUserReservationDto>()
          .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.Id : 0))
   .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer != null ? $"{src.Customer.FirstName} {src.Customer.LastName}" : null))
         .ForMember(dest => dest.CustomerEmail, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.Email : null))
 .ForMember(dest => dest.CustomerPhone, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.PhoneNumber : null))
        .ForMember(dest => dest.ReservationHour, opt => opt.MapFrom(src => src.ReservationDate.Hour.ToString()))
             .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
         .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));

            // ========== GuestReservation Mappings ========= =
 
   // GuestReservation -> AdminGuestReservationTableDto
       CreateMap<GuestReservation, AdminGuestReservationTableDto>()
      .ForMember(dest => dest.ReservationHour, opt => opt.MapFrom(src => src.ReservationDate.Hour.ToString()));

          // ========== Table Mappings ========= =
            
  // Table -> TableDto
            CreateMap<Table, TableDto>();
            
   // TableDto -> Table (for create/update operations)
 CreateMap<TableDto, Table>()
      .ForMember(dest => dest.UserReservations, opt => opt.Ignore())
      .ForMember(dest => dest.GuestReservations, opt => opt.Ignore())
                .ForMember(dest => dest.ReservedUntil, opt => opt.Ignore());

 // ========== Food Mappings ========= =
        
            // Food -> FoodDto
   CreateMap<Food, FoodDto>();
          
      // FoodDto -> Food (for create/update operations)
         CreateMap<FoodDto, Food>()
                .ForMember(dest => dest.Menu, opt => opt.Ignore());

            // ========== Drink Mappings ========= =
         
    // Drink -> DrinkDto
            CreateMap<Drink, DrinkDto>();
   
          // DrinkDto -> Drink (for create/update operations)
     CreateMap<DrinkDto, Drink>()
    .ForMember(dest => dest.Menu, opt => opt.Ignore());
   }
    }
}
