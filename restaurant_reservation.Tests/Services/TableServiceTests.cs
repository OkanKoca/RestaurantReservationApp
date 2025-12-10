using AutoMapper;
using Moq;
using restaurant_reservation.Data.Abstract;
using restaurant_reservation.Dto;
using restaurant_reservation.Models;
using restaurant_reservation.Services.Concrete;
using restaurant_reservation_api.Dto;
using restaurant_reservation_api.Models;

namespace restaurant_reservation.Tests.Services;

public class TableServiceTests
{
    private readonly Mock<ITableRepository> _tableRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly TableService _service;

    public TableServiceTests()
    {
        _tableRepositoryMock = new Mock<ITableRepository>();
        _mapperMock = new Mock<IMapper>();

        _service = new TableService(_tableRepositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public void GetAllTables_ReturnsMappedTableDtoList()
    {
        // Arrange
        var tables = new List<Table>
        {
            new() { Id = 1, Number = 1, Seats = 4 },
            new() { Id = 2, Number = 2, Seats = 6 }
        };

        var tableDtos = new List<TableDto>
        {
            new() { Id = 1, Number = 1, Seats = 4 },
            new() { Id = 2, Number = 2, Seats = 6 }
        };

        _tableRepositoryMock.Setup(r => r.Tables()).Returns(tables.AsQueryable());
        _mapperMock
            .Setup(m => m.Map<List<TableDto>>(It.IsAny<List<Table>>()))
            .Returns(tableDtos);

        // Act
        var result = _service.GetAllTables();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal(tableDtos, result);
    }

    [Fact]
    public void GetAllTables_EmptyList_ReturnsEmptyList()
    {
        // Arrange
        var tables = new List<Table>();
        var tableDtos = new List<TableDto>();

        _tableRepositoryMock.Setup(r => r.Tables()).Returns(tables.AsQueryable());
        _mapperMock
            .Setup(m => m.Map<List<TableDto>>(It.IsAny<List<Table>>()))
            .Returns(tableDtos);

        // Act
        var result = _service.GetAllTables();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void GetTableById_Found_ReturnsTable()
    {
        // Arrange
        var table = new Table { Id = 1, Number = 5, Seats = 4 };

        _tableRepositoryMock.Setup(r => r.GetById(table.Id)).Returns(table);

        // Act
        var result = _service.GetTableById(table.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(table, result);
    }

    [Fact]
    public void GetTableById_NotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        _tableRepositoryMock
            .Setup(r => r.GetById(It.IsAny<int>()))
            .Throws(new KeyNotFoundException());

        // Act & Assert
        Assert.Throws<KeyNotFoundException>(() => _service.GetTableById(1));
    }

    [Fact]
    public void CreateTable_ValidDto_CreatesAndReturnsTable()
    {
        // Arrange
        var tableDto = new TableDto { Number = 10, Seats = 4 };
        var table = new Table { Id = 1, Number = 10, Seats = 4 };

        _mapperMock
            .Setup(m => m.Map<Table>(tableDto))
            .Returns(table);

        // Act
        var result = _service.CreateTable(tableDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(table.Number, result.Number);
        Assert.Equal(table.Seats, result.Seats);
        _tableRepositoryMock.Verify(r => r.Add(table), Times.Once);
    }

    [Fact]
    public void UpdateTable_NotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        _tableRepositoryMock
            .Setup(r => r.GetById(It.IsAny<int>()))
            .Throws(new KeyNotFoundException());

        var tableDto = new TableDto { Number = 5, Seats = 4 };

        // Act & Assert
        Assert.Throws<KeyNotFoundException>(() => _service.UpdateTable(1, tableDto));
        _tableRepositoryMock.Verify(r => r.Update(It.IsAny<Table>()), Times.Never);
    }

    [Fact]
    public void UpdateTable_ValidData_UpdatesAndCallsRepository()
    {
        // Arrange
        var existing = new Table { Id = 1, Number = 1, Seats = 4 };
        var tableDto = new TableDto { Number = 5, Seats = 6 };

        _tableRepositoryMock
            .Setup(r => r.GetById(existing.Id))
            .Returns(existing);

        _mapperMock
            .Setup(m => m.Map(tableDto, existing))
            .Callback<TableDto, Table>((dto, table) =>
            {
                table.Number = dto.Number;
                table.Seats = dto.Seats;
            });

        // Act
        _service.UpdateTable(existing.Id, tableDto);

        // Assert
        Assert.Equal(5, existing.Number);
        Assert.Equal(6, existing.Seats);
        _tableRepositoryMock.Verify(r => r.Update(existing), Times.Once);
    }

    [Fact]
    public void DeleteTable_NotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        _tableRepositoryMock
            .Setup(r => r.GetById(It.IsAny<int>()))
            .Throws(new KeyNotFoundException());

        // Act & Assert
        Assert.Throws<KeyNotFoundException>(() => _service.DeleteTable(1));
        _tableRepositoryMock.Verify(r => r.Delete(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public void DeleteTable_Found_DeletesAndReturnsTrue()
    {
        // Arrange
        var table = new Table { Id = 1, Number = 5, Seats = 4 };

        _tableRepositoryMock
            .Setup(r => r.GetById(table.Id))
            .Returns(table);

        // Act
        var result = _service.DeleteTable(table.Id);

        // Assert
        Assert.True(result);
        _tableRepositoryMock.Verify(r => r.Delete(table.Id), Times.Once);
    }

    [Fact]
    public void GetTableOccupancy_NoReservations_ReturnsZero()
    {
        // Arrange
        var table = new Table
        {
            Id = 1,
            Number = 5,
            Seats = 4,
            UserReservations = new List<UserReservation>(),
            GuestReservations = new List<GuestReservation>()
        };

        _tableRepositoryMock
            .Setup(r => r.GetById(table.Id))
            .Returns(table);

        // Act
        var result = _service.GetTableOccupancy(table.Id, DateTime.Today);

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public void GetTableOccupancy_WithReservations_ReturnsCorrectPercentage()
    {
        // Arrange
        var today = DateTime.Today;
        var table = new Table
        {
            Id = 1,
            Number = 5,
            Seats = 4,
            UserReservations = new List<UserReservation>
            {
                new()
                {
                    Id = 1,
                    ReservationDate = today.AddHours(12),
                    NumberOfGuests = 2,
                    Status = ReservationStatus.Confirmed.ToString()
                }
            },
            GuestReservations = new List<GuestReservation>
            {
                new()
                {
                    Id = 1,
                    ReservationDate = today.AddHours(14),
                    FullName = "Test",
                    Email = "test@test.com",
                    PhoneNumber = "123"
                }
            }
        };

        _tableRepositoryMock
            .Setup(r => r.GetById(table.Id))
            .Returns(table);

        // WorkingHours.Hours has12 hours (10:00 -21:00)
        //2 reservations /12 hours =16.67%
        var expectedOccupancy = (2.0 / WorkingHours.Hours.Length) * 100;

        // Act
        var result = _service.GetTableOccupancy(table.Id, today);

        // Assert
        Assert.Equal(expectedOccupancy, result, precision: 2);
    }

    [Fact]
    public void GetTableOccupancy_TableNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        _tableRepositoryMock
            .Setup(r => r.GetById(It.IsAny<int>()))
            .Throws(new KeyNotFoundException());

        // Act & Assert
        Assert.Throws<KeyNotFoundException>(() => _service.GetTableOccupancy(1, DateTime.Today));
    }

    [Fact]
    public void GetTableUserReservations_TableNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        _tableRepositoryMock.Setup(r => r.GetById(It.IsAny<int>())).Throws(new KeyNotFoundException());

        // Act & Assert
        Assert.Throws<KeyNotFoundException>(() => _service.GetTableUserReservations(1, null));
    }

    [Fact]
    public void GetTableUserReservations_WithDateFilter_ReturnsFilteredReservations()
    {
        // Arrange
        var today = DateTime.Today;
        var tomorrow = today.AddDays(1);

        var table = new Table
        {
            Id = 1,
            Number = 5,
            UserReservations = new List<UserReservation>
            {
                new()
                {
                    Id = 1,
                    ReservationDate = today.AddHours(12),
                    NumberOfGuests = 2,
                    Status = ReservationStatus.Confirmed.ToString()
                },
                new()
                {
                    Id = 2,
                    ReservationDate = tomorrow.AddHours(14),
                    NumberOfGuests = 3,
                    Status = ReservationStatus.Pending.ToString()
                }
            }
        };

        var expectedDtos = new List<UserReservationDto>
        {
            new()
            {
                Id = 1,
                ReservationDate = today.AddHours(12),
                NumberOfGuests = 2
            }
        };

        _tableRepositoryMock.Setup(r => r.GetById(table.Id)).Returns(table);
        _mapperMock
            .Setup(m => m.Map<List<UserReservationDto>>(It.IsAny<List<UserReservation>>()))
            .Returns(expectedDtos);

        // Act
        var result = _service.GetTableUserReservations(table.Id, today);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public void GetTableGuestReservations_TableNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        _tableRepositoryMock.Setup(r => r.GetById(It.IsAny<int>())).Throws(new KeyNotFoundException());

        // Act & Assert
        Assert.Throws<KeyNotFoundException>(() => _service.GetTableGuestReservations(1, null));
    }

    [Fact]
    public void GetTableGuestReservations_WithDateFilter_ReturnsFilteredReservations()
    {
        // Arrange
        var today = DateTime.Today;
        var tomorrow = today.AddDays(1);

        var table = new Table
        {
            Id = 1,
            Number = 5,
            GuestReservations = new List<GuestReservation>
            {
                new()
                {
                    Id = 1,
                    ReservationDate = today.AddHours(12),
                    FullName = "Test User",
                    Email = "test@test.com",
                    PhoneNumber = "123"
                },
                new()
                {
                    Id = 2,
                    ReservationDate = tomorrow.AddHours(14),
                    FullName = "Test User2",
                    Email = "test2@test.com",
                    PhoneNumber = "456"
                }
            }
        };

        var expectedDtos = new List<AdminGuestReservationTableDto>
        {
            new()
            {
                Id = 1,
                ReservationDate = today.AddHours(12),
                FullName = "Test User",
                Email = "test@test.com",
                PhoneNumber = "123",
                NumberOfGuests = 2
            }
        };

        _tableRepositoryMock.Setup(r => r.GetById(table.Id)).Returns(table);
        _mapperMock
            .Setup(m => m.Map<List<AdminGuestReservationTableDto>>(It.IsAny<List<GuestReservation>>()))
            .Returns(expectedDtos);

        // Act
        var result = _service.GetTableGuestReservations(table.Id, today);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
    }
}
