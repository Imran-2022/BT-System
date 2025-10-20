using BusTicketReservationSystem.Application.Contracts.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusTicketReservationSystem.Application.Contracts.Repositories
{
    public interface IBusScheduleRepository
    {
        Task<List<AvailableBusDto>> FindAvailableBusesAsync(string from, string to, DateTime journeyDate);
    }
}