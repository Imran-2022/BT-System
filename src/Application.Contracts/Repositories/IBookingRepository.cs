using BusTicketReservationSystem.Application.Contracts.Dtos;

namespace BusTicketReservationSystem.Application.Contracts.Repositories
{
    public interface IBookingRepository
    {
        // booking transaction method
        Task<BookingResponseDto> BookSeatsTransactionAsync(BookSeatInputDto input);
    }
}