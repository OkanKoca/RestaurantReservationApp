using Moq;
using Microsoft.EntityFrameworkCore;
using restaurant_reservation.Data.Abstract;
using restaurant_reservation.Dto;
using restaurant_reservation.Models;
using restaurant_reservation.Services.Concrete;
using restaurant_reservation_api.Models;

namespace restaurant_reservation.Tests.Services;

public class GuestReservationServiceTests
{
    private readonly Mock<IGuestReservationRepository> _guestReservationRepositoryMock;
    private readonly Mock<ITableRepository> _tableRepositoryMock;
    private readonly Mock<IUserReservationRepository> _userReservationRepositoryMock;
    private readonly GuestReservationService _service;

    public GuestReservationServiceTests()
    {
        _guestReservationRepositoryMock = new Mock<IGuestReservationRepository>();
        _tableRepositoryMock = new Mock<ITableRepository>();
        _userReservationRepositoryMock = new Mock<IUserReservationRepository>();

        _service = new GuestReservationService(
            _guestReservationRepositoryMock.Object,
            _tableRepositoryMock.Object,
            _userReservationRepositoryMock.Object);
    }

    [Fact]
    public async Task CreateGuestReservationAsync_NoAvailableTables_ReturnsFailure()
    {
        // Arrange
        var dto = new GuestReservationDto
        {
            FullName = "Test User",
            Email = "test@example.com",
            PhoneNumber = "1234567890",
            NumberOfGuests = 2,
            ReservationDate = DateTime.Today.AddDays(1),
            ReservationHour = "19:00"
        };

        var tablesQueryable = new List<Table>().AsQueryable();

        _tableRepositoryMock
            .Setup(r => r.Tables())
            .Returns(tablesQueryable);

        // Act
        var (success, errorMessage, reservation) = await _service.CreateGuestReservationAsync(dto);

        // Assert
        Assert.False(success);
        Assert.Equal("No available tables for the requested date and time.", errorMessage);
        Assert.Null(reservation);
        _guestReservationRepositoryMock.Verify(r => r.Add(It.IsAny<GuestReservation>()), Times.Never);
    }

    [Fact]
    public async Task CreateGuestReservationAsync_AvailableTable_CreatesReservationAndReturnsSuccess()
    {
        // Arrange
        var dto = new GuestReservationDto
        {
            FullName = "Test User",
            Email = "test@example.com",
            PhoneNumber = "1234567890",
            NumberOfGuests = 2,
            ReservationDate = DateTime.Today.AddDays(1),
            ReservationHour = "19:00"
        };

        var table = new Table
        {
            Id = 1,
            Number = 5,
            GuestReservations = new List<GuestReservation>(),
            UserReservations = new List<UserReservation>()
        };

        var tablesQueryable = new List<Table> { table }.AsQueryable();

        _tableRepositoryMock.Setup(r => r.Tables()).Returns(tablesQueryable);

        GuestReservation? capturedReservation = null;
        _guestReservationRepositoryMock
            .Setup(r => r.Add(It.IsAny<GuestReservation>()))
            .Callback<GuestReservation>(r => capturedReservation = r);

        // Act
        var (success, errorMessage, reservation) = await _service.CreateGuestReservationAsync(dto);

        // Assert
        Assert.True(success);
        Assert.Null(errorMessage);
        Assert.NotNull(reservation);
        Assert.Equal(dto.FullName, reservation!.FullName);
        Assert.Equal(dto.Email, reservation.Email);
        Assert.Equal(dto.PhoneNumber, reservation.PhoneNumber);
        Assert.Equal(dto.NumberOfGuests, reservation.NumberOfGuests);
        Assert.Equal(table.Id, reservation.TableId);
        Assert.Equal(19, reservation.ReservationDate.Hour);

        _guestReservationRepositoryMock.Verify(r => r.Add(It.IsAny<GuestReservation>()), Times.Once);
        Assert.Same(capturedReservation, reservation);
    }

    [Fact]
    public void ToggleReservationStatus_ReservationNotFound_ReturnsFalse()
    {
        // Arrange
        _guestReservationRepositoryMock
            .Setup(r => r.GetById(It.IsAny<int>()))
            .Returns((GuestReservation?)null!);

        // Act
        var result = _service.ToggleReservationStatus(1);

        // Assert
        Assert.False(result);
        _guestReservationRepositoryMock.Verify(r => r.Update(It.IsAny<GuestReservation>()), Times.Never);
    }

    [Fact]
    public void ToggleReservationStatus_PendingToConfirmed_UpdatesStatusAndReturnsTrue()
    {
        // Arrange
        var reservation = new GuestReservation
        {
            Id = 1,
            Status = ReservationStatus.Pending.ToString(),
            FullName = "Test User",
            Email = "test@example.com",
            PhoneNumber = "1234567890"
        };

        _guestReservationRepositoryMock
            .Setup(r => r.GetById(reservation.Id))
            .Returns(reservation);

        // Act
        var result = _service.ToggleReservationStatus(reservation.Id);

        // Assert
        Assert.True(result);
        Assert.Equal(ReservationStatus.Confirmed.ToString(), reservation.Status);
        _guestReservationRepositoryMock.Verify(r => r.Update(reservation), Times.Once);
    }

    [Fact]
    public void ToggleReservationStatus_ConfirmedToPending_UpdatesStatusAndReturnsTrue()
    {
        // Arrange
        var reservation = new GuestReservation
        {
            Id = 1,
            Status = ReservationStatus.Confirmed.ToString(),
            FullName = "Test User",
            Email = "test@example.com",
            PhoneNumber = "1234567890"
        };

        _guestReservationRepositoryMock
            .Setup(r => r.GetById(reservation.Id))
            .Returns(reservation);

        // Act
        var result = _service.ToggleReservationStatus(reservation.Id);

        // Assert
        Assert.True(result);
        Assert.Equal(ReservationStatus.Pending.ToString(), reservation.Status);
        _guestReservationRepositoryMock.Verify(r => r.Update(reservation), Times.Once);
    }

    [Fact]
    public void DeleteGuestReservation_NotFound_ReturnsFailure()
    {
        // Arrange
        _guestReservationRepositoryMock
            .Setup(r => r.GetById(It.IsAny<int>()))
            .Returns((GuestReservation?)null!);

        // Act
        var (success, reservation) = _service.DeleteGuestReservation(1);

        // Assert
        Assert.False(success);
        Assert.Null(reservation);
        _guestReservationRepositoryMock.Verify(r => r.Delete(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public void DeleteGuestReservation_Found_DeletesAndReturnsSuccess()
    {
        // Arrange
        var reservation = new GuestReservation
        {
            Id = 1,
            FullName = "Test User",
            Email = "test@example.com",
            PhoneNumber = "1234567890"
        };

        _guestReservationRepositoryMock
            .Setup(r => r.GetById(reservation.Id))
            .Returns(reservation);

        // Act
        var (success, deletedReservation) = _service.DeleteGuestReservation(reservation.Id);

        // Assert
        Assert.True(success);
        Assert.Equal(reservation, deletedReservation);
        _guestReservationRepositoryMock.Verify(r => r.Delete(reservation.Id), Times.Once);
    }

    [Fact]
    public void GetGuestReservationById_Found_ReturnsReservation()
    {
        // Arrange
        var reservation = new GuestReservation
        {
            Id = 1,
            FullName = "Test User",
            Email = "test@example.com",
            PhoneNumber = "1234567890"
        };

        _guestReservationRepositoryMock
            .Setup(r => r.GetById(reservation.Id))
            .Returns(reservation);

        // Act
        var result = _service.GetGuestReservationById(reservation.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(reservation, result);
    }

    [Fact]
    public void GetGuestReservationById_NotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        _guestReservationRepositoryMock
            .Setup(r => r.GetById(It.IsAny<int>()))
            .Throws(new KeyNotFoundException("GuestReservation with ID 1 not found."));

        // Act & Assert
        Assert.Throws<KeyNotFoundException>(() => _service.GetGuestReservationById(1));
    }
}
