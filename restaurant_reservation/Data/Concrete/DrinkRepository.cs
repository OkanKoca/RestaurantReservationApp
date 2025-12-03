using Microsoft.AspNetCore.Mvc.ModelBinding;
using restaurant_reservation.Data.Abstract;
using restaurant_reservation.Models;
using restaurant_reservation_api.Data;

namespace restaurant_reservation.Data.Concrete
{
    public class DrinkRepository : IDrinkRepository
    {
        private readonly RestaurantContext _restaurantContext;
        public DrinkRepository(RestaurantContext restaurantContext) { 
            _restaurantContext = restaurantContext;
        }
        public void Add(Drink drink)
        {
            _restaurantContext.Drinks.Add(drink);
            _restaurantContext.SaveChanges();
        }
        public Drink GetById(int id)
        {
            var drink = _restaurantContext.Drinks.FirstOrDefault(d => d.Id == id);

            if(drink == null)
            {
                throw new KeyNotFoundException($"Drink with ID {id} not found.");
            }

            return drink;
        }
        public void Update(Drink drink)
        {
            if(drink == null)
            {
                throw new ArgumentNullException(nameof(drink), "Drink cannot be null.");
            }

            _restaurantContext.Drinks.Update(drink);
            _restaurantContext.SaveChanges();
        }

        public void Delete(int id)
        {
            var drinkToDelete = GetById(id);

            _restaurantContext.Drinks.Remove(drinkToDelete);
            _restaurantContext.SaveChanges();
        }

        public IQueryable<Drink> Drinks()
        {
            return _restaurantContext.Drinks; 
        }


    }
}
