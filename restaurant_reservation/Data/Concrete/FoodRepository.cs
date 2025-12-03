using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using restaurant_reservation.Data.Abstract;
using restaurant_reservation.Models;
using restaurant_reservation_api.Data;

namespace restaurant_reservation.Data.Concrete
{
    public class FoodRepository : IFoodRepository
    {
        private readonly RestaurantContext _restaurantContext;
        public FoodRepository(RestaurantContext restaurantContext) { 
            _restaurantContext = restaurantContext;
        }
        public void Add(Food food)
        {
            _restaurantContext.Foods.Add(food);
            _restaurantContext.SaveChanges();
        }
        public Food GetById(int id)
        {
            var food = _restaurantContext.Foods.FirstOrDefault(f => f.Id == id);

            if(food == null)
            {
                throw new KeyNotFoundException($"Food with ID {id} not found.");
            }

            return food;
        }
        public void Update(Food food)
        {
            if(food == null)
            {
                throw new ArgumentNullException(nameof(food), "food cannot be null.");
            }

            _restaurantContext.Foods.Update(food);
            _restaurantContext.SaveChanges();
        }

        public void Delete(int id)
        {
            var foodToDelete = GetById(id);

            _restaurantContext.Foods.Remove(foodToDelete);
            _restaurantContext.SaveChanges();
        }

        public IQueryable<Food> Foods()
        {
            return _restaurantContext.Foods; 
        }


    }
}
