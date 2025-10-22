using BusTicketReservationSystem.Application.Contracts.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusTicketReservationSystem.Application.Contracts.Services
{
    public interface ISearchService
    {
        Task<List<AvailableBusDto>> SearchAvailableBusesAsync(string from, string to, DateTime journeyDate);
        // NEW METHOD
        Task<AvailableBusDto?> GetScheduleDetailsAsync(Guid scheduleId);
        // 🎯 NEW METHOD: Handles the booking transaction
        Task<BookingResponseDto> BookSeatsAsync(BookSeatInputDto input);
    }
}