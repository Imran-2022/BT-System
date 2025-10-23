// src/Infrastructure/Repositories/BookingRepository.cs

using BusTicketReservationSystem.Application.Contracts.Repositories;
using BusTicketReservationSystem.Application.Contracts.Dtos;
using BusTicketReservationSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using BusTicketReservationSystem.Domain.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace BusTicketReservationSystem.Infrastructure.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly BusTicketDbContext _context;

        public BookingRepository(BusTicketDbContext context)
        {
            _context = context;
        }
        
        // 🎯 IMPLEMENTATION: Transactional booking logic
        public async Task<BookingResponseDto> BookSeatsTransactionAsync(BookSeatInputDto input)
        {
            // 1. Begin Database Transaction (ACID property: Atomicity)
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 2. Lock/Retrieve current seat statuses for the schedule and requested seats
                // Use AsEnumerable() to ensure filtering happens in memory after retrieving required seats
                var seatNumbersToBook = input.SeatBookings.Select(b => b.SeatNumber).ToList();
                
                // IMPORTANT: Fetch the seats that match the schedule and the list of seat numbers
                var seatsToUpdate = await _context.SeatStatuses
                    .Where(s => s.BusScheduleId == input.ScheduleId &&
                                seatNumbersToBook.Contains(s.SeatNumber))
                    .ToListAsync();

                // 3. Validation: Check if ALL requested seats exist and are AVAILABLE (Status = 1)
                var availableSeats = seatsToUpdate
                    .Where(s => s.Status == (int)SeatStatusCode.Available)
                    .ToList();
                
                // Check 1: Did we find exactly the number of seats requested? (Prevents non-existent seat booking)
                if (seatsToUpdate.Count != input.SeatBookings.Count)
                {
                    await transaction.RollbackAsync();
                    return new BookingResponseDto
                    {
                        BookingId = Guid.Empty,
                        Status = "Failure",
                        Message = "One or more selected seats do not exist for this schedule."
                    };
                }

                // Check 2: Are all found seats actually available? (Prevents double booking)
                if (availableSeats.Count != input.SeatBookings.Count)
                {
                    await transaction.RollbackAsync();
                    return new BookingResponseDto
                    {
                        BookingId = Guid.Empty,
                        Status = "Failure",
                        Message = "One or more selected seats are no longer available (already booked)."
                    };
                }

                // 4. Create the main Ticket/Booking Record
                var newTicket = new Ticket
                {
                    TicketId = Guid.NewGuid(),
                    BusScheduleId = input.ScheduleId,
                    BookingDate = DateTime.UtcNow,
                    BoardingPoint = await GetLocationName(input.BoardingPointId),
                    DroppingPoint = await GetLocationName(input.DroppingPointId),
                    MobileNumber = input.MobileNumber,
                    TotalPrice = input.SeatBookings.Sum(b => b.Price)
                };
                _context.Tickets.Add(newTicket);
                
                // 5. Update Seat Statuses and Link to Ticket
                foreach (var seat in availableSeats)
                {
                    var bookingDetail = input.SeatBookings.First(b => b.SeatNumber == seat.SeatNumber);
                    
                    seat.Status = (int)SeatStatusCode.Booked; // Status 3
                    // 🎯 NEW: Store Passenger and Ticket details on the seat
                    seat.PassengerName = bookingDetail.PassengerName;
                    seat.MobileNumber = input.MobileNumber; // Redundant but useful for reporting/lookup
                    seat.TicketId = newTicket.TicketId;
                }

                // 6. Save Changes (Updates SeatsStatuses and inserts the Ticket)
                await _context.SaveChangesAsync();

                // 7. Commit Transaction
                await transaction.CommitAsync();

                return new BookingResponseDto
                {
                    BookingId = newTicket.TicketId,
                    Status = "Success",
                    Message = $"Booking confirmed for {input.SeatBookings.Count} seats. Reference ID: {newTicket.TicketId}"
                };
            }
            catch (Exception ex)
            {
                // 8. Rollback on Error
                await transaction.RollbackAsync();
                // You would log ex here
                
                return new BookingResponseDto
                {
                    BookingId = Guid.Empty,
                    Status = "Failure",
                    Message = "An unexpected error occurred during the booking process. Try again later."
                };
            }
        }
        
        // Helper to retrieve the location name from the Point ID
        private async Task<string> GetLocationName(Guid pointId)
        {
            var point = await _context.BoardingPoints.AsNoTracking().FirstOrDefaultAsync(p => p.PointId == pointId);
            return point?.LocationName ?? "Unknown Location";
        }
    }
}