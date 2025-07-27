using restaurant_reservation.Models;

namespace restaurant_reservation.Data.Abstract
{
    public interface IGuestReservationRepository
    {
        void Add(GuestReservation guestReservation);
        void Update(GuestReservation guestReservation);
        void Delete(int id);
        GuestReservation GetById(int id);
        IQueryable<GuestReservation> GuestReservations();
    }
}
