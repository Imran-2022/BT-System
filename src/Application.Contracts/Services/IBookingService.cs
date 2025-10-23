// src/Application.Contracts/Services/IBookingService.cs (NEW FILE)
using BusTicketReservationSystem.Application.Contracts.Dtos;
using System;
using System.Threading.Tasks;

namespace BusTicketReservationSystem.Application.Contracts.Services
{
    public interface IBookingService
    {
        // 🎯 Implementation of your requested method
        Task<AvailableBusDto?> GetSeatPlanAsync(Guid busScheduleId);
        
        // 🎯 Implementation of your requested method
        Task<BookingResponseDto> BookSeatAsync(BookSeatInputDto input);
    }
}