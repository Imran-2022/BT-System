using BusTicketReservationSystem.Application.Contracts.Dtos;
using BusTicketReservationSystem.Application.Contracts.Repositories;
using BusTicketReservationSystem.Application.Contracts.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace BusTicketReservationSystem.Application.Services
{
    public class SearchService : ISearchService
    {
        private readonly IBusScheduleRepository _busScheduleRepository;

        public SearchService(IBusScheduleRepository busScheduleRepository)
        {
            _busScheduleRepository = busScheduleRepository;
        }

        public async Task<List<AvailableBusDto>> SearchAvailableBusesAsync(string from, string to, DateTime journeyDate)
        {
            // Return empty list for past journey dates
            if (journeyDate.Date < DateTime.Today.Date)
            {
                return new List<AvailableBusDto>();
            }
            // Fetch available buses and sort by start time
            var results = await _busScheduleRepository.FindAvailableBusesAsync(from, to, journeyDate);

            return results.OrderBy(b => b.StartTime).ToList();
        }

        public async Task<AvailableBusDto?> GetScheduleAndSeatDetailsAsync(Guid scheduleId)
        {
            if (scheduleId == Guid.Empty)
            {
                return null;
            }

            // Retrieve schedule details including seat plan
            return await _busScheduleRepository.GetBusScheduleAndSeatDetailsByIdAsync(scheduleId);
        }
    }
}