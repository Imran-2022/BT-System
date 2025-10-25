using BusTicketReservationSystem.Application.Contracts.Dtos;

namespace BusTicketReservationSystem.Application.Contracts.Services
{
    public interface IBookingService
    {
        // Retrieves the seat plan for a specific bus schedule.
        Task<AvailableBusDto?> GetSeatPlanAsync(Guid busScheduleId);
        
        // Books a seat based on the provided input details and returns the booking response.
        Task<BookingResponseDto> BookSeatAsync(BookSeatInputDto input);
    }
}