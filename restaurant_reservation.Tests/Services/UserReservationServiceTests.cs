using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Moq;
using restaurant_reservation.Data.Abstract;
using restaurant_reservation.Dto;
using restaurant_reservation.Models;
using restaurant_reservation.Services.Concrete;
using restaurant_reservation_api.Messaging;

namespace restaurant_reservation.Tests.Services;

public class UserReservationServiceTests
{
    private readonly Mock<IUserReservationRepository> _userReservationRepositoryMock;
    private readonly Mock<ITableRepository> _tableRepositoryMock;
    private readonly Mock<UserManager<AppUser>> _userManagerMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRabbitMQPublisher> _rabbitMQPublisherMock;
    private readonly UserReservationService _service;

    public UserReservationServiceTests()
    {
        _userReservationRepositoryMock = new Mock<IUserReservationRepository>();
        _tableRepositoryMock = new Mock<ITableRepository>();
        _mapperMock = new Mock<IMapper>();
        _rabbitMQPublisherMock = new Mock<IRabbitMQPublisher>();

        var store = new Mock<IUserStore<AppUser>>();
        _userManagerMock = new Mock<UserManager<AppUser>>(
            store.Object, null, null, null, null, null, null, null, null);

        _service = new UserReservationService(
            _userReservationRepositoryMock.Object,
            _tableRepositoryMock.Object,
            _userManagerMock.Object,
            _mapperMock.Object,
            _rabbitMQPublisherMock.Object);
    }

    [Fact]
    public async Task CreateUserReservationAsync_AvailableTableAndCustomer_CreatesReservationAndReturnsSuccess()
    {
        // Arrange
        var dto = new UserReservationDto
        {
            CustomerId = 1,
            ReservationDate = DateTime.Today.AddDays(1),
            NumberOfGuests = 2,
            ReservationHour = "19:00"
        };

        var table = new Table
        {
            Id = 10,
            Number = 5,
            GuestReservations = new List<GuestReservation>(),
            UserReservations = new List<UserReservation>()
        };

        var tables = new List<Table> { table }.AsQueryable();

        _tableRepositoryMock
            .Setup(r => r.Tables())
            .Returns(tables);

        var customer = new AppUser
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            UserName = "john"
        };

        _userManagerMock
            .Setup(m => m.FindByIdAsync(dto.CustomerId.ToString()))
            .ReturnsAsync(customer);

        UserReservation? capturedReservation = null;
        _userReservationRepositoryMock
            .Setup(r => r.Add(It.IsAny<UserReservation>()))
            .Callback<UserReservation>(r => capturedReservation = r);

        // Act
        var (success, errorMessage, reservation) = await _service.CreateUserReservationAsync(dto);

        // Assert
        Assert.True(success);
        Assert.Null(errorMessage);
        Assert.NotNull(reservation);
        Assert.Equal(customer, reservation!.Customer);
        Assert.Equal(dto.NumberOfGuests, reservation.NumberOfGuests);
        Assert.Equal(table.Id, reservation.TableId);
        Assert.Equal(19, reservation.ReservationDate.Hour);

        _userReservationRepositoryMock.Verify(r => r.Add(It.IsAny<UserReservation>()), Times.Once);
        Assert.Same(capturedReservation, reservation);
    }

    [Fact]
    public async Task CreateUserReservationAsync_NoAvailableTables_ReturnsFailure()
    {
        // Arrange
        var dto = new UserReservationDto
        {
            CustomerId = 1,
            ReservationDate = DateTime.Today.AddDays(1),
            NumberOfGuests = 2,
            ReservationHour = "19:00"
        };

        var tables = new List<Table>().AsQueryable();

        _tableRepositoryMock
            .Setup(r => r.Tables())
            .Returns(tables);

        // Act
        var (success, errorMessage, reservation) = await _service.CreateUserReservationAsync(dto);

        // Assert
        Assert.False(success);
        Assert.Equal("No available tables for the requested date and time.", errorMessage);
        Assert.Null(reservation);
        _userReservationRepositoryMock.Verify(r => r.Add(It.IsAny<UserReservation>()), Times.Never);
    }

    [Fact]
    public void GetUserReservationById_Found_ReturnsReservation()
    {
        // Arrange
        var reservation = new UserReservation
        {
            Id = 1,
            NumberOfGuests = 2,
            ReservationDate = DateTime.UtcNow,
            Customer = new AppUser { Id = 1, UserName = "john" },
            Table = new Table { Id = 1, Number = 5 }
        };

        _userReservationRepositoryMock
            .Setup(r => r.GetById(reservation.Id))
            .Returns(reservation);

        // Act
        var result = _service.GetUserReservationById(reservation.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(reservation, result);
    }

    [Fact]
    public void GetUserReservationById_NotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        _userReservationRepositoryMock
            .Setup(r => r.GetById(It.IsAny<int>()))
            .Throws(new KeyNotFoundException());

        // Act & Assert
        Assert.Throws<KeyNotFoundException>(() => _service.GetUserReservationById(1));
    }

    [Fact]
    public void UpdateUserReservation_NotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        _userReservationRepositoryMock
            .Setup(r => r.GetById(It.IsAny<int>()))
            .Returns((UserReservation?)null!);

        var dto = new UserReservationDto
        {
            ReservationDate = DateTime.Today.AddDays(1),
            NumberOfGuests = 3,
            ReservationHour = "20:00",
            Status = ReservationStatus.Pending.ToString()
        };

        // Act & Assert
        Assert.Throws<KeyNotFoundException>(() => _service.UpdateUserReservation(1, dto));
        _userReservationRepositoryMock.Verify(r => r.Update(It.IsAny<UserReservation>()), Times.Never);
    }

    [Fact]
    public void UpdateUserReservation_ValidData_UpdatesAndCallsRepository()
    {
        // Arrange
        var existing = new UserReservation
        {
            Id = 1,
            NumberOfGuests = 2,
            ReservationDate = DateTime.Today,
            Status = ReservationStatus.Pending.ToString(),
            Table = new Table
            {
                Id = 1,
                UserReservations = new List<UserReservation>(),
                GuestReservations = new List<GuestReservation>()
            }
        };

        existing.Table.UserReservations.Add(existing);

        var dto = new UserReservationDto
        {
            ReservationDate = DateTime.Today.AddDays(1),
            NumberOfGuests = 4,
            ReservationHour = "21:00",
            Status = ReservationStatus.Confirmed.ToString()
        };

        _userReservationRepositoryMock
            .Setup(r => r.GetById(existing.Id))
            .Returns(existing);

        // Act
        _service.UpdateUserReservation(existing.Id, dto);

        // Assert
        Assert.Equal(4, existing.NumberOfGuests);
        Assert.Equal(dto.Status, existing.Status);
        Assert.Equal(21, existing.ReservationDate.Hour);
        Assert.Equal(dto.ReservationDate.Date, existing.ReservationDate.Date);

        _userReservationRepositoryMock.Verify(r => r.Update(existing), Times.Once);
    }

    [Fact]
    public async Task ToggleReservationStatusAsync_NotFound_ReturnsFalse()
    {
        // Arrange
        _userReservationRepositoryMock
           .Setup(r => r.GetById(It.IsAny<int>()))
                 .Returns((UserReservation?)null!);

        // Act
        var result = await _service.ToggleReservationStatusAsync(1);

        // Assert
        Assert.False(result);
        _userReservationRepositoryMock.Verify(r => r.Update(It.IsAny<UserReservation>()), Times.Never);
    }

    [Fact]
    public async Task ToggleReservationStatusAsync_PendingToConfirmed_UpdatesStatusAndReturnsTrue()
    {
        // Arrange
        var customer = new AppUser
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            UserName = "john"
        };

        var reservation = new UserReservation
        {
            Id = 1,
            Status = ReservationStatus.Pending.ToString(),
            ReservationDate = DateTime.Today.AddDays(1),
            NumberOfGuests = 2,
            Customer = customer
        };

        _userReservationRepositoryMock
            .Setup(r => r.GetById(reservation.Id))
     .Returns(reservation);

        // Act
        var result = await _service.ToggleReservationStatusAsync(reservation.Id);

        // Assert
        Assert.True(result);
        Assert.Equal(ReservationStatus.Confirmed.ToString(), reservation.Status);
        _userReservationRepositoryMock.Verify(r => r.Update(reservation), Times.Once);
        _rabbitMQPublisherMock.Verify(p => p.PublishEmailAsync(It.IsAny<EmailMessage>()), Times.Once);
    }

    [Fact]
    public async Task ToggleReservationStatusAsync_ConfirmedToPending_UpdatesStatusAndReturnsTrue()
    {
        // Arrange
        var reservation = new UserReservation
        {
            Id = 1,
            Status = ReservationStatus.Confirmed.ToString(),
            ReservationDate = DateTime.Today.AddDays(1),
            NumberOfGuests = 2
        };

        _userReservationRepositoryMock
        .Setup(r => r.GetById(reservation.Id))
           .Returns(reservation);

        // Act
        var result = await _service.ToggleReservationStatusAsync(reservation.Id);

        // Assert
        Assert.True(result);
        Assert.Equal(ReservationStatus.Pending.ToString(), reservation.Status);
        _userReservationRepositoryMock.Verify(r => r.Update(reservation), Times.Once);
        _rabbitMQPublisherMock.Verify(p => p.PublishEmailAsync(It.IsAny<EmailMessage>()), Times.Never);
    }

    [Fact]
    public void DeleteUserReservation_NotFound_ReturnsFailure()
    {
        // Arrange
        _userReservationRepositoryMock
            .Setup(r => r.GetById(It.IsAny<int>()))
            .Returns((UserReservation?)null!);

        // Act
        var (success, reservation) = _service.DeleteUserReservation(1);

        // Assert
        Assert.False(success);
        Assert.Null(reservation);
        _userReservationRepositoryMock.Verify(r => r.Delete(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public void DeleteUserReservation_Found_DeletesAndReturnsSuccess()
    {
        // Arrange
        var reservation = new UserReservation
        {
            Id = 1,
            ReservationDate = DateTime.Today.AddDays(1),
            NumberOfGuests = 2
        };

        _userReservationRepositoryMock
            .Setup(r => r.GetById(reservation.Id))
            .Returns(reservation);

        // Act
        var (success, deletedReservation) = _service.DeleteUserReservation(reservation.Id);

        // Assert
        Assert.True(success);
        Assert.Equal(reservation, deletedReservation);
        _userReservationRepositoryMock.Verify(r => r.Delete(reservation.Id), Times.Once);
    }
}
