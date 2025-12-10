using AutoMapper;
using Microsoft.EntityFrameworkCore;
using restaurant_reservation.Data.Abstract;
using restaurant_reservation.Models;
using restaurant_reservation.Services.Abstract;
using restaurant_reservation_api.Dto;

namespace restaurant_reservation.Services.Concrete
{
    public class DrinkService : IDrinkService
    {
        private readonly IDrinkRepository _drinkRepository;
        private readonly IMenuRepository _menuRepository;
        private readonly IMapper _mapper;

        public DrinkService(IDrinkRepository drinkRepository,IMenuRepository menuRepository,IMapper mapper)
        {
            _drinkRepository = drinkRepository;
            _menuRepository = menuRepository;
            _mapper = mapper;
        }

        public List<DrinkDto> GetAllDrinks()
        {
            var drinks = _drinkRepository.Drinks()
                .Include(d => d.Menu)
                .ToList();

            return _mapper.Map<List<DrinkDto>>(drinks);
        }

        public DrinkDto? GetDrinkById(int id)
        {
            var drink = _drinkRepository.GetById(id);
            if (drink == null)
            {
                return null;
            }

            return _mapper.Map<DrinkDto>(drink);
        }

        public DrinkDto CreateDrink(DrinkDto drinkDto)
        {
            var drink = _mapper.Map<Drink>(drinkDto);

            var menu = _menuRepository.Menus().FirstOrDefault(m => m.Id == drinkDto.MenuId);
            if (menu != null)
            {
                drink.Menu = menu;
            }

            _drinkRepository.Add(drink);

            return _mapper.Map<DrinkDto>(drink);
        }

        public void UpdateDrink(int id, DrinkDto drinkDto)
        {
            var drinkToUpdate = _drinkRepository.GetById(id);
            if (drinkToUpdate == null)
            {
                throw new KeyNotFoundException($"Drink with ID {id} not found.");
            }

            _mapper.Map(drinkDto, drinkToUpdate);
            drinkToUpdate.Menu = _menuRepository.GetById(drinkDto.MenuId) ?? null;

            _drinkRepository.Update(drinkToUpdate);
        }

        public void DeleteDrink(int id)
        {
            _drinkRepository.Delete(id);
        }
    }
}
