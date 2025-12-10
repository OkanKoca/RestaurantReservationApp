using AutoMapper;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using restaurant_reservation.Data.Abstract;
using restaurant_reservation.Dto;
using restaurant_reservation.Models;
using restaurant_reservation.Services.Concrete;
using restaurant_reservation_api.Dto;

namespace restaurant_reservation.Tests.Services;

public class MenuServiceTests
{
    private readonly Mock<IMenuRepository> _menuRepositoryMock;
    private readonly Mock<IDrinkRepository> _drinkRepositoryMock;
    private readonly Mock<IFoodRepository> _foodRepositoryMock;
    private readonly Mock<IDistributedCache> _cacheMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly MenuService _service;

    public MenuServiceTests()
    {
        _menuRepositoryMock = new Mock<IMenuRepository>();
        _drinkRepositoryMock = new Mock<IDrinkRepository>();
        _foodRepositoryMock = new Mock<IFoodRepository>();
        _cacheMock = new Mock<IDistributedCache>();
        _mapperMock = new Mock<IMapper>();

        _service = new MenuService(
        _menuRepositoryMock.Object,
        _drinkRepositoryMock.Object,
        _foodRepositoryMock.Object,
        _cacheMock.Object,
        _mapperMock.Object);
    }

    [Fact]
    public void GetMenuById_Found_ReturnsMenu()
    {
        // Arrange
        var menu = new Menu { Id = 1, Name = "Main Menu", Description = "Our main menu" };

        _menuRepositoryMock.Setup(r => r.GetById(1)).Returns(menu);

        // Act
        var result = _service.GetMenuById(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(menu.Name, result!.Name);
    }

    [Fact]
    public void GetMenuById_NotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        _menuRepositoryMock.Setup(r => r.GetById(It.IsAny<int>())).Throws(new KeyNotFoundException());

        // Act & Assert
        Assert.Throws<KeyNotFoundException>(() => _service.GetMenuById(1));
    }

    [Fact]
    public async Task CreateMenuAsync_ValidDto_CreatesAndReturnsMenu()
    {
        // Arrange
        var menuDto = new MenuDto
        {
            Name = "New Menu",
            Description = "Fresh items",
            DrinkIds = new List<int> { 1 },
            FoodIds = new List<int> { 1 }
        };

        var drink = new Drink { Id = 1, Name = "Water", Description = "Plain", Price = 1 };
        var food = new Food { Id = 1, Name = "Salad", Description = "Green", Price = 8 };

        _drinkRepositoryMock.Setup(r => r.GetById(1)).Returns(drink);
        _foodRepositoryMock.Setup(r => r.GetById(1)).Returns(food);

        // Act
        var result = await _service.CreateMenuAsync(menuDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(menuDto.Name, result.Name);
        Assert.Single(result.Drinks);
        Assert.Single(result.Foods);
        _menuRepositoryMock.Verify(r => r.Add(It.IsAny<Menu>()), Times.Once);
    }

    [Fact]
    public void UpdateMenu_ValidData_UpdatesMenu()
    {
        // Arrange
        var existing = new Menu
        {
            Id = 1,
            Name = "Old",
            Description = "Old Desc",
            Drinks = new List<Drink>(),
            Foods = new List<Food>()
        };

        var menuDto = new MenuDto
        {
            Name = "New",
            Description = "New Desc",
            DrinkIds = new List<int> { 1 },
            FoodIds = new List<int> { 1 }
        };

        var drink = new Drink { Id = 1, Name = "Tea", Description = "Hot", Price = 2 };
        var food = new Food { Id = 1, Name = "Soup", Description = "Warm", Price = 6 };

        _menuRepositoryMock.Setup(r => r.GetById(1)).Returns(existing);
        _drinkRepositoryMock.Setup(r => r.GetById(1)).Returns(drink);
        _foodRepositoryMock.Setup(r => r.GetById(1)).Returns(food);

        // Act
        _service.UpdateMenu(1, menuDto);

        // Assert
        Assert.Equal("New", existing.Name);
        Assert.Equal("New Desc", existing.Description);
        _menuRepositoryMock.Verify(r => r.Update(existing), Times.Once);
    }

    [Fact]
    public void DeleteMenu_CallsRepository()
    {
        // Act
        _service.DeleteMenu(1);

        // Assert
        _menuRepositoryMock.Verify(r => r.Delete(1), Times.Once);
    }

    [Fact]
    public void GetMenuDrinks_MenuNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        _menuRepositoryMock.Setup(r => r.GetById(It.IsAny<int>())).Throws(new KeyNotFoundException());

        // Act & Assert
        Assert.Throws<KeyNotFoundException>(() => _service.GetMenuDrinks(1));
    }

    [Fact]
    public void GetMenuDrinks_Found_ReturnsDrinkDtos()
    {
        // Arrange
        var drinks = new List<Drink>
 {
 new Drink { Id =1, Name = "Coffee", Description = "Hot", Price =3 }
 };

        var menu = new Menu { Id = 1, Name = "Drinks", Drinks = drinks };
        var drinkDtos = new List<DrinkDto> { new DrinkDto { Id = 1, Name = "Coffee" } };

        _menuRepositoryMock.Setup(r => r.GetById(1)).Returns(menu);
        _mapperMock.Setup(m => m.Map<List<DrinkDto>>(drinks)).Returns(drinkDtos);

        // Act
        var result = _service.GetMenuDrinks(1);

        // Assert
        Assert.Single(result);
        Assert.Equal("Coffee", result[0].Name);
    }

    [Fact]
    public void GetMenuFoods_MenuNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        _menuRepositoryMock.Setup(r => r.GetById(It.IsAny<int>())).Throws(new KeyNotFoundException());

        // Act & Assert
        Assert.Throws<KeyNotFoundException>(() => _service.GetMenuFoods(1));
    }

    [Fact]
    public void GetMenuFoods_Found_ReturnsFoodDtos()
    {
        // Arrange
        var foods = new List<Food>
 {
 new Food { Id =1, Name = "Pasta", Description = "Italian", Price =12 }
 };

        var menu = new Menu { Id = 1, Name = "Mains", Foods = foods };
        var foodDtos = new List<FoodDto> { new FoodDto { Id = 1, Name = "Pasta" } };

        _menuRepositoryMock.Setup(r => r.GetById(1)).Returns(menu);
        _mapperMock.Setup(m => m.Map<List<FoodDto>>(foods)).Returns(foodDtos);

        // Act
        var result = _service.GetMenuFoods(1);

        // Assert
        Assert.Single(result);
        Assert.Equal("Pasta", result[0].Name);
    }
}
