using AutoMapper;
using Moq;
using restaurant_reservation.Data.Abstract;
using restaurant_reservation.Models;
using restaurant_reservation.Services.Concrete;
using restaurant_reservation_api.Dto;

namespace restaurant_reservation.Tests.Services;

public class DrinkServiceTests
{
    private readonly Mock<IDrinkRepository> _drinkRepositoryMock;
    private readonly Mock<IMenuRepository> _menuRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly DrinkService _service;

    public DrinkServiceTests()
    {
        _drinkRepositoryMock = new Mock<IDrinkRepository>();
        _menuRepositoryMock = new Mock<IMenuRepository>();
        _mapperMock = new Mock<IMapper>();

        _service = new DrinkService(
            _drinkRepositoryMock.Object,
            _menuRepositoryMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public void GetDrinkById_Found_ReturnsDrinkDto()
    {
        // Arrange
        var drink = new Drink { Id = 1, Name = "Cola", Description = "Soft drink", Price = 3 };
        var drinkDto = new DrinkDto { Id = 1, Name = "Cola", Description = "Soft drink", Price = 3 };

        _drinkRepositoryMock.Setup(r => r.GetById(1)).Returns(drink);
        _mapperMock.Setup(m => m.Map<DrinkDto>(drink)).Returns(drinkDto);

        // Act
        var result = _service.GetDrinkById(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(drinkDto.Name, result!.Name);
    }

    [Fact]
    public void GetDrinkById_NotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        _drinkRepositoryMock.Setup(r => r.GetById(It.IsAny<int>())).Throws(new KeyNotFoundException());

        // Act & Assert
        Assert.Throws<KeyNotFoundException>(() => _service.GetDrinkById(1));
    }

    [Fact]
    public void CreateDrink_ValidDto_CreatesAndReturnsDrink()
    {
        // Arrange
        var drinkDto = new DrinkDto { Name = "Lemonade", Description = "Fresh", Price = 5, MenuId = 1 };
        var drink = new Drink { Id = 1, Name = "Lemonade", Description = "Fresh", Price = 5, MenuId = 1 };
        var menu = new Menu { Id = 1, Name = "Beverages" };

        _mapperMock.Setup(m => m.Map<Drink>(drinkDto)).Returns(drink);
        _menuRepositoryMock.Setup(r => r.Menus()).Returns(new List<Menu> { menu }.AsQueryable());
        _mapperMock.Setup(m => m.Map<DrinkDto>(drink)).Returns(drinkDto);

        // Act
        var result = _service.CreateDrink(drinkDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(drinkDto.Name, result.Name);
        _drinkRepositoryMock.Verify(r => r.Add(drink), Times.Once);
    }

    [Fact]
    public void UpdateDrink_NotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        _drinkRepositoryMock.Setup(r => r.GetById(It.IsAny<int>())).Throws(new KeyNotFoundException());
        var drinkDto = new DrinkDto { Name = "Updated", Description = "Desc", Price = 10, MenuId = 1 };

        // Act & Assert
        Assert.Throws<KeyNotFoundException>(() => _service.UpdateDrink(1, drinkDto));
        _drinkRepositoryMock.Verify(r => r.Update(It.IsAny<Drink>()), Times.Never);
    }

    [Fact]
    public void UpdateDrink_ValidData_UpdatesDrink()
    {
        // Arrange
        var existing = new Drink { Id = 1, Name = "Old", Description = "Old Desc", Price = 5, MenuId = 1 };
        var drinkDto = new DrinkDto { Name = "New", Description = "New Desc", Price = 10, MenuId = 2 };
        var menu = new Menu { Id = 2, Name = "Hot Drinks" };

        _drinkRepositoryMock.Setup(r => r.GetById(1)).Returns(existing);
        _menuRepositoryMock.Setup(r => r.GetById(2)).Returns(menu);

        // Act
        _service.UpdateDrink(1, drinkDto);

        // Assert
        _drinkRepositoryMock.Verify(r => r.Update(existing), Times.Once);
    }

    [Fact]
    public void DeleteDrink_CallsRepository()
    {
        // Act
        _service.DeleteDrink(1);

        // Assert
        _drinkRepositoryMock.Verify(r => r.Delete(1), Times.Once);
    }
}
