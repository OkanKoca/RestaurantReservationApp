using Microsoft.EntityFrameworkCore;
using restaurant_reservation.Data.Abstract;
using restaurant_reservation.Models;
using restaurant_reservation_api.Data;

namespace restaurant_reservation.Data.Concrete
{
    public class UserReservationRepository : IUserReservationRepository
    {
        private readonly RestaurantContext _restaurantContext;
        public UserReservationRepository(RestaurantContext restaurantContext)
        {
            _restaurantContext = restaurantContext;
        }

        public async void Add(UserReservation userReservation)
        {
            _restaurantContext.Reservations.Add(userReservation);
            await _restaurantContext.SaveChangesAsync();
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
            var userReservation = _restaurantContext.Reservations.Include(c => c.Customer).Include(t=> t.Table).FirstOrDefault(t => t.Id == id);

            if (userReservation == null)
            {
                throw new KeyNotFoundException($"UserReservation with ID {id} not found.");
            }

            return userReservation;
        }

        public async void Update(UserReservation userReservation)
        {
            _restaurantContext.Reservations.Update(userReservation);
            await _restaurantContext.SaveChangesAsync();
        }
    }
}
