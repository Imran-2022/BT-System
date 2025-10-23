// src/Application.Contracts/Services/ISearchService.cs
using BusTicketReservationSystem.Application.Contracts.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusTicketReservationSystem.Application.Contracts.Services
{
    public interface ISearchService
    {
        Task<List<AvailableBusDto>> SearchAvailableBusesAsync(string from, string to, DateTime journeyDate);
        
        // 🎯 FIX: Renamed to GetScheduleAndSeatDetailsAsync for clarity
        Task<AvailableBusDto?> GetScheduleAndSeatDetailsAsync(Guid scheduleId); 
        
        // ❌ REMOVED: Task<BookingResponseDto> BookSeatsAsync(BookSeatInputDto input);
    }
}