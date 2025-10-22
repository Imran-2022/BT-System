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
        // 🎯 NEW IMPLEMENTATION
        public async Task<BookingResponseDto> BookSeatsTransactionAsync(BookSeatInputDto input)
        {
            // 1. Begin Database Transaction (Ensures atomicity: All seats booked OR none are)
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 2. Lock/Retrieve current seat statuses for the schedule
                var seatsToUpdate = await _context.SeatStatuses
                    .Where(s => s.BusScheduleId == input.ScheduleId &&
                                input.SeatBookings.Select(b => b.SeatNumber).Contains(s.SeatNumber))
                    // IMPORTANT: Use .Where().ToList() before the following check to force execution
                    .ToListAsync();

                // 3. Validation: Check if all requested seats exist and are AVAILABLE (Status = 1)
                var availableSeats = seatsToUpdate.Where(s => s.Status == (int)SeatStatusCode.Available).ToList();

                if (availableSeats.Count != input.SeatBookings.Count)
                {
                    await transaction.RollbackAsync();
                    return new BookingResponseDto
                    {
                        BookingId = Guid.Empty,
                        Status = "Failure",
                        Message = "One or more selected seats are no longer available or do not exist."
                    };
                }

                // 4. Update Seat Statuses
                foreach (var seat in availableSeats)
                {
                    // 🎯 CRITICAL: Use the generic BOOKED status (3) as requested
                    seat.Status = (int)SeatStatusCode.Booked;
                    // NOTE: If you need to store M/F, you'd need to add a Gender field to SeatStatus.cs
                }

                // 5. Save Changes (Updates the SeatStatuses table)
                await _context.SaveChangesAsync();

                // 6. Finalize Booking (In a real app, you would create a Booking record here)
                Guid bookingId = Guid.NewGuid(); // Placeholder booking ID

                // 7. Commit Transaction
                await transaction.CommitAsync();

                return new BookingResponseDto
                {
                    BookingId = bookingId,
                    Status = "Success",
                    Message = $"Booking confirmed for {input.SeatBookings.Count} seats."
                };
            }
            catch (Exception ex)
            {
                // 8. Rollback on Error
                await transaction.RollbackAsync();
                // Log the exception (recommended)
                // _logger.LogError(ex, "Failed to complete seat booking transaction.");

                return new BookingResponseDto
                {
                    BookingId = Guid.Empty,
                    Status = "Failure",
                    Message = "An unexpected error occurred during the booking process."
                };
            }
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
                .Include(s => s.SeatStatuses) // <--- CRITICAL ADDITION: Include SeatStatuses to count them
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
                    
                    // 🎯 FIX: Calculate SeatsLeft dynamically for the search result
                    SeatsLeft = s.SeatStatuses.Count(ss => ss.Status == (int)SeatStatusCode.Available), 
                    
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
            var availableSeatCount = scheduleWithSeats.SeatStatuses.Count(ss => ss.Status == (int)SeatStatusCode.Available); // 🎯 NEW: Calculate available seats

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
                // 🎯 FIX: Calculate SeatsLeft dynamically
                SeatsLeft = availableSeatCount,
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