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

        // Inside BusScheduleRepository class

        // --- NEW HELPER METHOD ---
        private async Task InitializeSeatStatusesAsync(Guid busScheduleId, decimal basePrice)
        {
            // Generates 32 seats: A1, A2, A3, A4... H1, H2, H3, H4
            var seatStatuses = new List<SeatStatus>();
            var seatLetters = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H' };

            foreach (var rowLetter in seatLetters)
            {
                for (int seatIndex = 1; seatIndex <= 4; seatIndex++)
                {
                    seatStatuses.Add(new SeatStatus
                    {
                        BusScheduleId = busScheduleId,
                        SeatNumber = $"{rowLetter}{seatIndex}",
                        Status = (int)SeatStatusCode.Available,
                        Price = basePrice
                    });
                }
            }

            await _context.SeatStatuses.AddRangeAsync(seatStatuses);
            await _context.SaveChangesAsync();
        }

        public async Task<AvailableBusDto?> GetBusScheduleByIdAsync(Guid busScheduleId)
        {
            // Step 1: Check if the schedule exists and has existing seats
            var scheduleWithSeats = await _context.BusSchedules
                .Include(s => s.Route)
                .Include(s => s.Bus)
                .Include(s => s.SeatStatuses) // <--- INCLUDE THE NEW TABLE
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.BusScheduleId == busScheduleId);

            // Step 2: TEMPORARY SEEDING - If schedule exists but has NO seats, create them
            if (scheduleWithSeats != null && scheduleWithSeats.SeatStatuses.Count == 0)
            {
                // Use the hardcoded price for simplicity
                decimal basePrice = 700;

                // Note: InitializeSeatStatusesAsync will save changes to the DB.
                await InitializeSeatStatusesAsync(busScheduleId, basePrice);

                // Refetch the schedule with the new seats (turning off AsNoTracking for the refetch)
                scheduleWithSeats = await _context.BusSchedules
                    .Include(s => s.Route)
                    .Include(s => s.Bus)
                    .Include(s => s.SeatStatuses)
                    .FirstOrDefaultAsync(s => s.BusScheduleId == busScheduleId);
            }

            // Step 3: Projection to DTO
            if (scheduleWithSeats == null) return null;

            var dto = new AvailableBusDto
            {
                BusScheduleId = scheduleWithSeats.BusScheduleId,
                CompanyName = scheduleWithSeats.Bus.CompanyName,
                BusName = scheduleWithSeats.Bus.BusName,
                BusType = scheduleWithSeats.Bus.BusType,
                StartTime = scheduleWithSeats.StartTime,
                // Use hardcoded data for now
                BoardingPoint = "Kallyanpur",
                ArrivalTime = scheduleWithSeats.StartTime.Add(TimeSpan.FromHours(5)),
                DroppingPoint = "Rajshahi Counter",
                SeatsLeft = scheduleWithSeats.Bus.TotalSeats, // Needs update with real logic later
                Price = 700,
                CancellationPolicy = "Standard Policy",

                // --- NEW SEAT LAYOUT PROJECTION ---
                SeatLayout = scheduleWithSeats.SeatStatuses
                    .Select(ss => new SeatStatusDto
                    {
                        SeatNumber = ss.SeatNumber,
                        Status = ss.Status,
                        Price = ss.Price
                    }).ToList()
            };

            return dto;
        }
    }

}