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
            // Business Rule: Check if the date is valid (e.g., not in the past).
            if (journeyDate.Date < DateTime.Today.Date)
            {
                // In a real system, you'd throw a domain/application exception.
                return new List<AvailableBusDto>(); 
            }
            
            // Delegate data retrieval to the Infrastructure layer (Repository)
            var results = await _busScheduleRepository.FindAvailableBusesAsync(from, to, journeyDate);

            // Application-level sorting/filtering (e.g., sort by StartTime)
            return results.OrderBy(b => b.StartTime).ToList();
        }
    }
}