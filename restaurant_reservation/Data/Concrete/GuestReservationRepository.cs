using Microsoft.EntityFrameworkCore;
using restaurant_reservation.Data.Abstract;
using restaurant_reservation.Models;

namespace restaurant_reservation.Data.Concrete
{
    public class GuestReservationRepository : IGuestReservationRepository
    {
        private readonly RestaurantContext _restaurantContext;
        public GuestReservationRepository(RestaurantContext restaurantContext)
        {
            _restaurantContext = restaurantContext;
        }

        public void Add(GuestReservation guestReservation)
        {
            _restaurantContext.GuestReservations.Add(guestReservation);
            _restaurantContext.SaveChanges();
        }

        public void Delete(int id)
        {
            _restaurantContext.GuestReservations.Remove(GetById(id));
            _restaurantContext.SaveChanges();
        }

        public IQueryable<GuestReservation> GuestReservations()
        {
            return _restaurantContext.GuestReservations;
        }

        public GuestReservation GetById(int id)
        {
            var guestReservation = _restaurantContext.GuestReservations.FirstOrDefault(t => t.Id == id);

            if (guestReservation == null)
            {
                throw new KeyNotFoundException($"GuestReservation with ID {id} not found.");
            }

            return guestReservation;
        }

        public void Update(GuestReservation guestReservation)
        {
            _restaurantContext.GuestReservations.Update(guestReservation);
            _restaurantContext.SaveChanges();
        }
    }
}
