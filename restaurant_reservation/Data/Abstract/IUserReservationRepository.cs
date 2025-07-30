using restaurant_reservation.Models;

namespace restaurant_reservation.Data.Abstract
{
    public interface IUserReservationRepository
    {
        void Add(UserReservation userReservation);
        void Update(UserReservation userReservation);
        void Delete(int id);
        UserReservation GetById(int id);
        IQueryable<UserReservation> UserReservations();
    }
}
