using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using restaurant_reservation.Data.Abstract;
using restaurant_reservation.Models;
using restaurant_reservation.Services.Abstract;
using restaurant_reservation_api.Dto;
using System.Text.Json;

namespace restaurant_reservation.Services.Concrete
{
    public class DrinkService : IDrinkService
    {
        private readonly IDrinkRepository _drinkRepository;
        private readonly IMenuRepository _menuRepository;
        private readonly IDistributedCache _distributedCache;
        private readonly IMapper _mapper;

        private const string DrinksCacheKey = "drinks_list";

        public DrinkService(IDrinkRepository drinkRepository,IMenuRepository menuRepository,IDistributedCache distributedCache,IMapper mapper)
        {
            _drinkRepository = drinkRepository;
            _menuRepository = menuRepository;
            _distributedCache = distributedCache;
            _mapper = mapper;
        }

        public async Task<List<DrinkDto>> GetAllDrinksAsync()
        {
            var cachedData = await _distributedCache.GetStringAsync(DrinksCacheKey);

            if (cachedData != null)
            {
                return JsonSerializer.Deserialize<List<DrinkDto>>(cachedData) ?? new List<DrinkDto>();
            }

            var drinks = _drinkRepository.Drinks()
                .Include(d => d.Menu)
                .ToList();

            var drinkDtos = _mapper.Map<List<DrinkDto>>(drinks);

            await _distributedCache.SetStringAsync(DrinksCacheKey,
                JsonSerializer.Serialize(drinkDtos),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(6)
                });

            return drinkDtos;
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

        public async Task<DrinkDto> CreateDrinkAsync(DrinkDto drinkDto)
        {
            var drink = _mapper.Map<Drink>(drinkDto);

            var menu = _menuRepository.Menus().FirstOrDefault(m => m.Id == drinkDto.MenuId);
            if (menu != null)
            {
                drink.Menu = menu;
            }

            _drinkRepository.Add(drink);
            await _distributedCache.RemoveAsync(DrinksCacheKey);

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

        public async Task UpdateDrinkAsync(int id, DrinkDto drinkDto)
        {
            var drinkToUpdate = _drinkRepository.GetById(id);
            if (drinkToUpdate == null)
            {
                throw new KeyNotFoundException($"Drink with ID {id} not found.");
            }

            _mapper.Map(drinkDto, drinkToUpdate);
            drinkToUpdate.Menu = _menuRepository.GetById(drinkDto.MenuId) ?? null;

            _drinkRepository.Update(drinkToUpdate);
            await _distributedCache.RemoveAsync(DrinksCacheKey);
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

        public async Task DeleteDrinkAsync(int id)
        {
            _drinkRepository.Delete(id);
            await _distributedCache.RemoveAsync(DrinksCacheKey);
        }

        public void DeleteDrink(int id)
        {
            _drinkRepository.Delete(id);
        }
    }
}
