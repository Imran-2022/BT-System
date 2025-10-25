using BusTicketReservationSystem.Application.Contracts.Repositories;
using BusTicketReservationSystem.Application.Contracts.Dtos;
using BusTicketReservationSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using BusTicketReservationSystem.Domain.Entities;

namespace BusTicketReservationSystem.Infrastructure.Repositories
{
    public class BusScheduleRepository : IBusScheduleRepository
    {
        private readonly BusTicketDbContext _context;

        public BusScheduleRepository(BusTicketDbContext context)
        {
            _context = context;
        }

        // Finds available buses based on origin, destination, and journey date
        public async Task<List<AvailableBusDto>> FindAvailableBusesAsync(string from, string to, DateTime journeyDate)
        {
            DateTime utcJourneyDate = DateTime.SpecifyKind(journeyDate.Date, DateTimeKind.Utc);

            // Find the route matching the origin and destination
            var route = await _context.Routes
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Origin == from && r.Destination == to);

            if (route == null)
            {
                return new List<AvailableBusDto>();
            }

            // Query schedules for the route on the specified date
            var schedules = await _context.BusSchedules
                .AsNoTracking()
                .Include(s => s.Route)
                .Include(s => s.Bus).ThenInclude(b => b.Layout) 
                .Include(s => s.SeatStatuses)
                .Include(s => s.Route.BoardingPoints)

                .Where(s => s.RouteId == route.RouteId && 
                            s.JourneyDate.Date == utcJourneyDate.Date)

                .Select(s => new AvailableBusDto
                {
                    BusScheduleId = s.BusScheduleId,
                    CompanyName = s.Bus.CompanyName,
                    BusName = s.Bus.BusName,
                    BusType = s.Bus.BusType,
                    StartTime = s.StartTime,
                    ArrivalTime = s.StartTime.Add(TimeSpan.FromHours(5)),

                    SeatsLeft = s.Bus.Layout.TotalSeats -
                                s.SeatStatuses.Count(ss => ss.Status != (int)SeatStatusCode.Available),

                    Price = s.SeatStatuses.Any()
                    ? s.SeatStatuses.First().Price
                    : s.Bus.BasePrice,

                    CancellationPolicy = "Flexible",
                    LayoutId = s.Bus.Layout.BusSeatLayoutId,
                    SeatConfiguration = s.Bus.Layout.SeatConfiguration,

                    // Boarding points for the route
                    BoardingPoints = s.Route.BoardingPoints
                        .Where(p => !p.IsDroppingPoint)
                        .Select(p => new PointOptionDto
                        {
                            PointId = p.PointId,
                            LocationName = p.LocationName,
                            Time = s.StartTime.Add(p.DepartureTimeOffset)
                        }).ToList(),

                    // Dropping points for the route
                    DroppingPoints = s.Route.BoardingPoints
                        .Where(p => p.IsDroppingPoint)
                        .Select(p => new PointOptionDto
                        {
                            PointId = p.PointId,
                            LocationName = p.LocationName,
                            Time = s.StartTime.Add(p.DepartureTimeOffset)
                        }).ToList(),

                    SeatLayout = new List<SeatStatusDto>()
                })
                .ToListAsync();

            return schedules;
        }
        // Gets detailed schedule and seat information by schedule ID
        public async Task<AvailableBusDto?> GetBusScheduleAndSeatDetailsByIdAsync(Guid busScheduleId)
        {
            var schedule = await _context.BusSchedules
                .Include(s => s.Route).ThenInclude(r => r.BoardingPoints)
                .Include(s => s.Bus).ThenInclude(b => b.Layout)
                .Include(s => s.SeatStatuses)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.BusScheduleId == busScheduleId);

            if (schedule == null) return null;

            var availableSeatCount = schedule.SeatStatuses.Count(ss => ss.Status == (int)SeatStatusCode.Available);

            return new AvailableBusDto
            {
                BusScheduleId = schedule.BusScheduleId,
                CompanyName = schedule.Bus.CompanyName,
                BusName = schedule.Bus.BusName,
                BusType = schedule.Bus.BusType,
                StartTime = schedule.StartTime,
                SeatsLeft = availableSeatCount,
                Price = schedule.SeatStatuses.Any() ? schedule.SeatStatuses.First().Price : schedule.Bus.BasePrice,
                CancellationPolicy = "Flexible",
                ArrivalTime = schedule.StartTime.Add(TimeSpan.FromHours(5)),
                LayoutId = schedule.Bus.Layout.BusSeatLayoutId,
                SeatConfiguration = schedule.Bus.Layout.SeatConfiguration,

                BoardingPoints = schedule.Route.BoardingPoints
                    .Where(p => !p.IsDroppingPoint)
                    .Select(p => new PointOptionDto { PointId = p.PointId, LocationName = p.LocationName, Time = schedule.StartTime.Add(p.DepartureTimeOffset) })
                    .ToList(),
                DroppingPoints = schedule.Route.BoardingPoints
                    .Where(p => p.IsDroppingPoint)
                    .Select(p => new PointOptionDto { PointId = p.PointId, LocationName = p.LocationName, Time = schedule.StartTime.Add(p.DepartureTimeOffset) })
                    .ToList(),

                // SEAT LAYOUT PROJECTION
                SeatLayout = schedule.SeatStatuses
                    .Select(ss => new SeatStatusDto
                    {
                        SeatNumber = ss.SeatNumber,
                        Status = ss.Status,
                        Price = ss.Price
                    }).ToList()
            };
        }
        
        // Returns booked seat numbers for a given schedule
        public Task<List<string>> GetBookedSeatNumbersAsync(Guid busScheduleId)
        {
            return Task.FromResult(new List<string>());
        }
    }

}