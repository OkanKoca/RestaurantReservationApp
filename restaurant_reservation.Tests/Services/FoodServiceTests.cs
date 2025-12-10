using AutoMapper;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using restaurant_reservation.Data.Abstract;
using restaurant_reservation.Models;
using restaurant_reservation.Services.Concrete;
using restaurant_reservation_api.Dto;

namespace restaurant_reservation.Tests.Services;

public class FoodServiceTests
{
    private readonly Mock<IFoodRepository> _foodRepositoryMock;
    private readonly Mock<IMenuRepository> _menuRepositoryMock;
    private readonly Mock<IDistributedCache> _distributedCacheMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly FoodService _service;

    public FoodServiceTests()
    {
        _foodRepositoryMock = new Mock<IFoodRepository>();
        _menuRepositoryMock = new Mock<IMenuRepository>();
        _distributedCacheMock = new Mock<IDistributedCache>();
        _mapperMock = new Mock<IMapper>();

        _service = new FoodService(
        _foodRepositoryMock.Object,
        _menuRepositoryMock.Object,
        _distributedCacheMock.Object,
        _mapperMock.Object);
    }

    [Fact]
    public void GetFoodById_Found_ReturnsFoodDto()
    {
        // Arrange
        var food = new Food { Id = 1, Name = "Pizza", Description = "Italian", Price = 15 };
        var foodDto = new FoodDto { Id = 1, Name = "Pizza", Description = "Italian", Price = 15 };

        _foodRepositoryMock.Setup(r => r.GetById(1)).Returns(food);
        _mapperMock.Setup(m => m.Map<FoodDto>(food)).Returns(foodDto);

        // Act
        var result = _service.GetFoodById(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(foodDto.Name, result!.Name);
    }

    [Fact]
    public void GetFoodById_NotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        _foodRepositoryMock.Setup(r => r.GetById(It.IsAny<int>())).Throws(new KeyNotFoundException());

        // Act & Assert
        Assert.Throws<KeyNotFoundException>(() => _service.GetFoodById(1));
    }

    [Fact]
    public void CreateFood_ValidDto_CreatesAndReturnsFood()
    {
        // Arrange
        var foodDto = new FoodDto { Name = "Burger", Description = "American", Price = 12, MenuId = 1 };
        var food = new Food { Id = 1, Name = "Burger", Description = "American", Price = 12, MenuId = 1 };
        var menu = new Menu { Id = 1, Name = "Main" };

        _mapperMock.Setup(m => m.Map<Food>(foodDto)).Returns(food);
        _menuRepositoryMock.Setup(r => r.Menus()).Returns(new List<Menu> { menu }.AsQueryable());
        _mapperMock.Setup(m => m.Map<FoodDto>(food)).Returns(foodDto);

        // Act
        var result = _service.CreateFood(foodDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(foodDto.Name, result.Name);
        _foodRepositoryMock.Verify(r => r.Add(food), Times.Once);
    }

    [Fact]
    public void UpdateFood_NotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        _foodRepositoryMock.Setup(r => r.GetById(It.IsAny<int>())).Throws(new KeyNotFoundException());
        var foodDto = new FoodDto { Name = "Updated", Description = "Desc", Price = 10, MenuId = 1 };

        // Act & Assert
        Assert.Throws<KeyNotFoundException>(() => _service.UpdateFood(1, foodDto));
        _foodRepositoryMock.Verify(r => r.Update(It.IsAny<Food>()), Times.Never);
    }

    [Fact]
    public void UpdateFood_ValidData_UpdatesFood()
    {
        // Arrange
        var existing = new Food { Id = 1, Name = "Old", Description = "Old Desc", Price = 5, MenuId = 1 };
        var foodDto = new FoodDto { Name = "New", Description = "New Desc", Price = 10, MenuId = 2 };
        var menu = new Menu { Id = 2, Name = "Desserts" };

        _foodRepositoryMock.Setup(r => r.GetById(1)).Returns(existing);
        _menuRepositoryMock.Setup(r => r.GetById(2)).Returns(menu);

        // Act
        _service.UpdateFood(1, foodDto);

        // Assert
        _foodRepositoryMock.Verify(r => r.Update(existing), Times.Once);
    }

    [Fact]
    public void DeleteFood_CallsRepository()
    {
        // Act
        _service.DeleteFood(1);

        // Assert
        _foodRepositoryMock.Verify(r => r.Delete(1), Times.Once);
    }
}
