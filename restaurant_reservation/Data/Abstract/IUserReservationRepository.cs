using restaurant_reservation.Models;

namespace restaurant_reservation.Data.Abstract
{
    public interface IUserReservationRepository
    {
        void Add(Reservation userReservation);
        void Update(Reservation userReservation);
        void Delete(int id);
        Reservation GetById(int id);
        IQueryable<Reservation> UserReservations();
    }
}
