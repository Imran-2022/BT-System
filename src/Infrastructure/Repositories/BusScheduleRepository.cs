using BusTicketReservationSystem.Application.Contracts.Repositories;
using BusTicketReservationSystem.Application.Contracts.Dtos;
using BusTicketReservationSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using BusTicketReservationSystem.Domain.Entities;
using System; 

namespace BusTicketReservationSystem.Infrastructure.Repositories
{
    public class BusScheduleRepository : IBusScheduleRepository
    {
        private readonly BusTicketDbContext _context;

        public BusScheduleRepository(BusTicketDbContext context)
        {
            _context = context;
        }

        // FIX 1: Changed method name back to FindAvailableBusesAsync to match your interface
        public async Task<List<AvailableBusDto>> FindAvailableBusesAsync(string from, string to, DateTime journeyDate)
        {
            // CRITICAL FIX: Explicitly set the DateTimeKind to UTC for Npgsql compatibility.
            DateTime utcJourneyDate = DateTime.SpecifyKind(journeyDate.Date, DateTimeKind.Utc);

            var schedules = await _context.BusSchedules
                .AsNoTracking()
                .Include(s => s.Route)
                .Include(s => s.Bus)
                .Where(s => s.Route.Origin == from &&
                            s.Route.Destination == to &&
                            // Compare the date part against the corrected UTC parameter
                            s.JourneyDate.Date == utcJourneyDate.Date)
                .Select(s => new AvailableBusDto
                {
                    // FIX 2: Removed .ToString() because the DTO property is a GUID
                    BusScheduleId = s.BusScheduleId, 
                    CompanyName = s.Bus.CompanyName,
                    BusName = s.Bus.BusName,
                    BusType = s.Bus.BusType,
                    
                    // FIX 3: Removed .ToString() because the DTO property is a TimeSpan
                    StartTime = s.StartTime, 
                    BoardingPoint = "Kallyanpur",
                    
                    // FIX 4: Removed .ToString() because the DTO property is a TimeSpan
                    ArrivalTime = s.StartTime.Add(TimeSpan.FromHours(5)), 
                    DroppingPoint = "Rajshahi Counter",
                    SeatsLeft = s.Bus.TotalSeats,
                    Price = 700,
                    CancellationPolicy = "Standard Policy"
                })
                .ToListAsync();

            return schedules;
        }
    }
}