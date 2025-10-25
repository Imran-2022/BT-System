using BusTicketReservationSystem.Application.Contracts.Dtos;

namespace BusTicketReservationSystem.Application.Contracts.Repositories
{
    public interface IBusScheduleRepository
    {
        Task<List<AvailableBusDto>> FindAvailableBusesAsync(string from, string to, DateTime journeyDate);
        Task<AvailableBusDto?> GetBusScheduleAndSeatDetailsByIdAsync(Guid busScheduleId); 
        // [testing]REQUIRED METHOD FOR VALIDATION
        Task<List<string>> GetBookedSeatNumbersAsync(Guid busScheduleId);
    }
}