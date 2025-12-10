using restaurant_reservation.Dto;
using restaurant_reservation.Models;
using restaurant_reservation_api.Dto;

namespace restaurant_reservation.Services.Abstract
{
    public interface ITableService
    {
        List<TableDto> GetAllTables();
        Table? GetTableById(int id);
        List<UserReservationDto> GetTableUserReservations(int id, DateTime? dateTime);
        List<AdminGuestReservationTableDto> GetTableGuestReservations(int id, DateTime? dateTime);
        double GetTableOccupancy(int id, DateTime dateTime);
        Table CreateTable(TableDto tableDto);
        void UpdateTable(int id, TableDto tableDto);
        bool DeleteTable(int id);
    }
}
