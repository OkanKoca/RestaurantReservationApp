using AutoMapper;
using restaurant_reservation_system.Models.Dto;
using restaurant_reservation_system.Models.ViewModel;

namespace restaurant_reservation_system.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // ========== User Reservation Mappings ==========

            // AdminUserReservationDto -> UserReservationViewModel
            CreateMap<AdminUserReservationDto, UserReservationViewModel>();

            // UserReservationDto -> MyReservationsViewModel
            CreateMap<UserReservationDto, MyReservationsViewModel>()
                .ForMember(dest => dest.ReservationHour, 
                    opt => opt.MapFrom(src => src.ReservationDate.Hour.ToString()));

            // UserReservationViewModel -> UserReservationDto (for creating reservations)
            CreateMap<UserReservationViewModel, UserReservationDto>();

            // ========== Guest Reservation Mappings ==========

            // AdminGuestReservationDto -> AdminGuestReservationViewModel
            CreateMap<AdminGuestReservationDto, AdminGuestReservationViewModel>();
        }
    }
}
