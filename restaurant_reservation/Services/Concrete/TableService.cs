using AutoMapper;
using restaurant_reservation.Data.Abstract;
using restaurant_reservation.Dto;
using restaurant_reservation.Models;
using restaurant_reservation.Services.Abstract;
using restaurant_reservation_api.Dto;
using restaurant_reservation_api.Models;

namespace restaurant_reservation.Services.Concrete
{
    public class TableService : ITableService
    {
        private readonly ITableRepository _tableRepository;
        private readonly IMapper _mapper;

        public TableService(ITableRepository tableRepository, IMapper mapper)
        {
            _tableRepository = tableRepository;
            _mapper = mapper;
        }

        public List<TableDto> GetAllTables()
        {
            var tables = _tableRepository.Tables().ToList();
            return _mapper.Map<List<TableDto>>(tables);
        }

        public Table? GetTableById(int id)
        {
            return _tableRepository.GetById(id);
        }

        public List<UserReservationDto> GetTableUserReservations(int id, DateTime? dateTime)
        {
            var table = _tableRepository.GetById(id);
            if (table == null)
            {
                return new List<UserReservationDto>();
            }

            var query = table.UserReservations.AsQueryable();
            if (dateTime.HasValue)
            {
                var d = dateTime.Value.Date;
                query = query.Where(r => r.ReservationDate.Date == d);
            }

            return _mapper.Map<List<UserReservationDto>>(query.ToList());
        }

        public List<AdminGuestReservationTableDto> GetTableGuestReservations(int id, DateTime? dateTime)
        {
            var table = _tableRepository.GetById(id);
            if (table == null)
            {
                return new List<AdminGuestReservationTableDto>();
            }

            var query = table.GuestReservations.AsQueryable();
            if (dateTime.HasValue)
            {
                var d = dateTime.Value.Date;
                query = query.Where(g => g.ReservationDate.Date == d);
            }

            return _mapper.Map<List<AdminGuestReservationTableDto>>(query.ToList());
        }

        public double GetTableOccupancy(int id, DateTime dateTime)
        {
            var table = _tableRepository.GetById(id);
            if (table == null)
            {
                return 0;
            }

            var countOfWorkingHours = WorkingHours.Hours.Length;

            var userReservations = table.UserReservations
                      .Where(u => u.ReservationDate.Date == dateTime.Date)
                   .ToList();
            var guestReservations = table.GuestReservations
               .Where(g => g.ReservationDate.Date == dateTime.Date)
                        .ToList();

            int countOfReservationsAtDate = userReservations.Count + guestReservations.Count;

            return ((double)countOfReservationsAtDate / countOfWorkingHours) * 100;
        }

        public Table CreateTable(TableDto tableDto)
        {
            var table = _mapper.Map<Table>(tableDto);
            _tableRepository.Add(table);
            return table;
        }

        public void UpdateTable(int id, TableDto tableDto)
        {
            var table = _tableRepository.GetById(id);
            if (table == null)
            {
                throw new KeyNotFoundException($"Table with ID {id} not found.");
            }

            _mapper.Map(tableDto, table);
            _tableRepository.Update(table);
        }

        public bool DeleteTable(int id)
        {
            if (_tableRepository.GetById(id) == null)
            {
                return false;
            }

            _tableRepository.Delete(id);
            return true;
        }
    }
}
