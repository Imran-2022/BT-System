// src/Infrastructure/Repositories/BusScheduleRepository.cs

using BusTicketReservationSystem.Application.Contracts.Repositories;
using BusTicketReservationSystem.Application.Contracts.Dtos;
using BusTicketReservationSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using BusTicketReservationSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusTicketReservationSystem.Infrastructure.Repositories
{
    public class BusScheduleRepository : IBusScheduleRepository
    {
        private readonly BusTicketDbContext _context;

        public BusScheduleRepository(BusTicketDbContext context)
        {
            _context = context;
        }

        // ❌ REMOVED: BookSeatsTransactionAsync (Moved to IBookingRepository)

        // 🎯 UPDATED: FindAvailableBusesAsync to use dynamic points and prices
        // src/Infrastructure/Repositories/BusScheduleRepository.cs

        public async Task<List<AvailableBusDto>> FindAvailableBusesAsync(string from, string to, DateTime journeyDate)
        {
            DateTime utcJourneyDate = DateTime.SpecifyKind(journeyDate.Date, DateTimeKind.Utc);

            // 1. Find the specific Route ID first based on exact origin/destination
            var route = await _context.Routes
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Origin == from && r.Destination == to);

            if (route == null)
            {
                // No exact route found, return empty list
                return new List<AvailableBusDto>();
            }

            // 2. Query schedules using the exact RouteId found in step 1
            var schedules = await _context.BusSchedules
                .AsNoTracking()
                // Include statements should now follow the query to ensure all data is projected
                .Include(s => s.Route)
                .Include(s => s.Bus).ThenInclude(b => b.Layout) // Ensure Layout is included for TotalSeats
                .Include(s => s.SeatStatuses)
                .Include(s => s.Route.BoardingPoints)

                // 🎯 FIX 2: Filter by the exact RouteId and JourneyDate
                .Where(s => s.RouteId == route.RouteId && // <-- Only schedules matching the exact route
                            s.JourneyDate.Date == utcJourneyDate.Date)

                .Select(s => new AvailableBusDto
                {
                    BusScheduleId = s.BusScheduleId,
                    CompanyName = s.Bus.CompanyName,
                    BusName = s.Bus.BusName,
                    BusType = s.Bus.BusType,
                    StartTime = s.StartTime,
                    ArrivalTime = s.StartTime.Add(TimeSpan.FromHours(5)),

                    // FIX 1: Correctly calculate SeatsLeft and use fallback price
                    SeatsLeft = s.Bus.Layout.TotalSeats -
                                s.SeatStatuses.Count(ss => ss.Status != (int)SeatStatusCode.Available),

                    Price = s.SeatStatuses.Any()
                        ? s.SeatStatuses.First().Price
                        : (s.Bus.BusType.Contains("AC") ? 1200m : 700m),

                    CancellationPolicy = "Flexible",
                // 🎯 FIX 3: Include the necessary layout details
                    LayoutId = s.Bus.Layout.BusSeatLayoutId,
                    SeatConfiguration = s.Bus.Layout.SeatConfiguration,

                    // DYNAMIC POINTS: Projecting Boarding Points
                    BoardingPoints = s.Route.BoardingPoints
                        .Where(p => !p.IsDroppingPoint)
                        .Select(p => new PointOptionDto
                        {
                            PointId = p.PointId,
                            LocationName = p.LocationName,
                            Time = s.StartTime.Add(p.DepartureTimeOffset)
                        }).ToList(),

                    // DYNAMIC POINTS: Projecting Dropping Points
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
        // 🎯 RENAMED & UPDATED: GetBusScheduleAndSeatDetailsByIdAsync
        public async Task<AvailableBusDto?> GetBusScheduleAndSeatDetailsByIdAsync(Guid busScheduleId)
        {
            var schedule = await _context.BusSchedules
                .Include(s => s.Route).ThenInclude(r => r.BoardingPoints)
                .Include(s => s.Bus).ThenInclude(b => b.Layout)
                .Include(s => s.SeatStatuses)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.BusScheduleId == busScheduleId);

            if (schedule == null) return null;

            // 🎯 Check if seats exist, if not, initialize them based on the Bus's Layout
            if (schedule.SeatStatuses.Count == 0)
            {
                // CRITICAL: We need to use the Bus's specific price
                decimal basePrice = schedule.Bus.BusType.Contains("AC") ? 1200 : 700;

                // Note: InitializeSeatStatusesAsync will save changes to the DB.
                await InitializeSeatStatusesAsync(schedule.BusScheduleId, basePrice, schedule.Bus.Layout.SeatConfiguration);

                // Refetch the schedule with the new seats
                schedule = await _context.BusSchedules
                    .Include(s => s.Route).ThenInclude(r => r.BoardingPoints)
                    .Include(s => s.Bus).ThenInclude(b => b.Layout)
                    .Include(s => s.SeatStatuses)
                    .FirstOrDefaultAsync(s => s.BusScheduleId == busScheduleId);

                if (schedule == null) return null;
            }

            var availableSeatCount = schedule.SeatStatuses.Count(ss => ss.Status == (int)SeatStatusCode.Available);

            return new AvailableBusDto
            {
                BusScheduleId = schedule.BusScheduleId,
                CompanyName = schedule.Bus.CompanyName,
                BusName = schedule.Bus.BusName,
                BusType = schedule.Bus.BusType,
                StartTime = schedule.StartTime,
                SeatsLeft = availableSeatCount,
                Price = schedule.SeatStatuses.First().Price,
                CancellationPolicy = "Flexible",
                ArrivalTime = schedule.StartTime.Add(TimeSpan.FromHours(5)),
            // 🎯 FIX 3: Include the necessary layout details
                LayoutId = schedule.Bus.Layout.BusSeatLayoutId,
                SeatConfiguration = schedule.Bus.Layout.SeatConfiguration,

                // 🎯 DYNAMIC POINTS
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

        // 🎯 UPDATED: InitializeSeatStatusesAsync to use BusSeatLayout
        private async Task InitializeSeatStatusesAsync(Guid busScheduleId, decimal basePrice, string seatConfiguration)
        {
            var seatStatuses = new List<SeatStatus>();

            // Seat Configuration: "A1,A2,A3,A4;B1,B2,B3,B4;..."
            var seatDefinitions = seatConfiguration.Split(';')
                .SelectMany(row => row.Split(',').Where(s => !string.IsNullOrWhiteSpace(s)))
                .ToList();

            foreach (var seatNumber in seatDefinitions)
            {
                seatStatuses.Add(new SeatStatus
                {
                    BusScheduleId = busScheduleId,
                    SeatNumber = seatNumber,
                    Status = (int)SeatStatusCode.Available,
                    Price = basePrice
                });
            }

            await _context.SeatStatuses.AddRangeAsync(seatStatuses);
            await _context.SaveChangesAsync();
        }
    }
}