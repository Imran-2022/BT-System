using BusTicketReservationSystem.Application.Contracts.Dtos;

namespace BusTicketReservationSystem.Application.Contracts.Services
{
    public interface ISearchService
    {
        Task<List<AvailableBusDto>> SearchAvailableBusesAsync(string from, string to, DateTime journeyDate);
        Task<AvailableBusDto?> GetScheduleAndSeatDetailsAsync(Guid scheduleId); 
    }
}