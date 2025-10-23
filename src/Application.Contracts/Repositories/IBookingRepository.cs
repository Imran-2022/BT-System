// src/Application.Contracts/Repositories/IBookingRepository.cs (NEW FILE)
using BusTicketReservationSystem.Application.Contracts.Dtos;
using System.Threading.Tasks;

namespace BusTicketReservationSystem.Application.Contracts.Repositories
{
    public interface IBookingRepository
    {
        // 🎯 NEW: Dedicated booking transaction method
        Task<BookingResponseDto> BookSeatsTransactionAsync(BookSeatInputDto input);
    }
}