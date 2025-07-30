using Microsoft.EntityFrameworkCore;
using restaurant_reservation.Data.Abstract;
using restaurant_reservation.Models;

namespace restaurant_reservation.Data.Concrete
{
    public class UserReservationRepository : IUserReservationRepository
    {
        private readonly RestaurantContext _restaurantContext;
        public UserReservationRepository(RestaurantContext restaurantContext)
        {
            _restaurantContext = restaurantContext;
        }

        public void Add(UserReservation userReservation)
        {
            _restaurantContext.Reservations.Add(userReservation);
            _restaurantContext.SaveChanges();
        }

        public void Delete(int id)
        {
            _restaurantContext.Reservations.Remove(GetById(id));
            _restaurantContext.SaveChanges();
        }

        public IQueryable<UserReservation> UserReservations()
        {
            return _restaurantContext.Reservations.Include(r => r.Customer);
        }

        public UserReservation GetById(int id)
        {
            var userReservation = _restaurantContext.Reservations.FirstOrDefault(t => t.Id == id);

            if (userReservation == null)
            {
                throw new KeyNotFoundException($"UserReservation with ID {id} not found.");
            }

            return userReservation;
        }

        public void Update(UserReservation userReservation)
        {
            _restaurantContext.Reservations.Update(userReservation);
            _restaurantContext.SaveChanges();
        }
    }
}
