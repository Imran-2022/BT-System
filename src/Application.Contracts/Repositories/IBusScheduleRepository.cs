// src/Application.Contracts/Repositories/IBusScheduleRepository.cs
using BusTicketReservationSystem.Application.Contracts.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusTicketReservationSystem.Application.Contracts.Repositories
{
    public interface IBusScheduleRepository
    {
        Task<List<AvailableBusDto>> FindAvailableBusesAsync(string from, string to, DateTime journeyDate);
        
        // 🎯 RENAMED: To reflect the new service method
        Task<AvailableBusDto?> GetBusScheduleAndSeatDetailsByIdAsync(Guid busScheduleId); 
        
        // ❌ REMOVED: Task<BookingResponseDto> BookSeatsTransactionAsync(BookSeatInputDto input);
    }
}