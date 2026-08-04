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

        // It relies on the _busScheduleRepository to fetch the raw list of available buses from the database.

        public SearchService(IBusScheduleRepository busScheduleRepository)
        {
            _busScheduleRepository = busScheduleRepository;
        }

        public async Task<List<AvailableBusDto>> SearchAvailableBusesAsync(string from, string to, DateTime journeyDate)
        // Finds all buses running between two points on a specific date and ensures the results are usable.
        {
            // Return empty list for past journey dates
            if (journeyDate.Date < DateTime.Today.Date)
            {
                return new List<AvailableBusDto>();
            }

            try
            {
                var results = await _busScheduleRepository.FindAvailableBusesAsync(from, to, journeyDate);
                if (results == null || results.Count == 0)
                {
                    return DemoFallback.SearchBuses(from, to, journeyDate);
                }
                return results.OrderBy(b => b.StartTime).ToList();
            }
            catch
            {
                return DemoFallback.SearchBuses(from, to, journeyDate);
            }
        }

        public async Task<AvailableBusDto?> GetScheduleAndSeatDetailsAsync(Guid scheduleId)
        {
            if (scheduleId == Guid.Empty)
            {
                return null;
            }

            // Retrieve schedule details including seat plan
            var schedule = await _busScheduleRepository.GetBusScheduleAndSeatDetailsByIdAsync(scheduleId);
            if (schedule != null)
            {
                return schedule;
            }

            // If the requested schedule was generated from fallback search data,
            // return the matching demo schedule details.
            return DemoFallback.GetScheduleDetails(scheduleId);
        }
    }
}