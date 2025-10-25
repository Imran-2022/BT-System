using BusTicketReservationSystem.Application.Contracts.Repositories;
using BusTicketReservationSystem.Application.Contracts.Dtos;
using BusTicketReservationSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using BusTicketReservationSystem.Domain.Entities;

namespace BusTicketReservationSystem.Infrastructure.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly BusTicketDbContext _context;

        public BookingRepository(BusTicketDbContext context)
        {
            _context = context;
        }
        
        // Books seats transactionally for a given schedule
        public async Task<BookingResponseDto> BookSeatsTransactionAsync(BookSeatInputDto input)
        {
            // Begin database transaction for ACID compliance
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Extract seat numbers from the input
                var seatNumbersToBook = input.SeatBookings.Select(b => b.SeatNumber).ToList();
                
                // Retrieve seats for the schedule that match the requested seat numbers
                var seatsToUpdate = await _context.SeatStatuses
                    .Where(s => s.BusScheduleId == input.ScheduleId &&
                                seatNumbersToBook.Contains(s.SeatNumber))
                    .ToListAsync();

                // Filter available seats (Status = Available)
                var availableSeats = seatsToUpdate
                    .Where(s => s.Status == (int)SeatStatusCode.Available)
                    .ToList();
                
                // Validation: Check if all requested seats exist
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

                // Validation: Check if all requested seats are available
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

                // Create the ticket/booking record
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
                
                // Update seat statuses and link to ticket
                foreach (var seat in availableSeats)
                {
                    var bookingDetail = input.SeatBookings.First(b => b.SeatNumber == seat.SeatNumber);
                    seat.Status = (int)SeatStatusCode.Booked; 
                    seat.PassengerName = bookingDetail.PassengerName;
                    seat.MobileNumber = input.MobileNumber; 
                    seat.TicketId = newTicket.TicketId;
                }

                // Save changes to the database
                await _context.SaveChangesAsync();

                // Commit the transaction
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
                // Rollback transaction on error
                await transaction.RollbackAsync();
                return new BookingResponseDto
                {
                    BookingId = Guid.Empty,
                    Status = "Failure",
                    Message = "An unexpected error occurred during the booking process. Try again later."
                };
            }
        }
        
        // Helper method to get location name by Point ID
        private async Task<string> GetLocationName(Guid pointId)
        {
            var point = await _context.BoardingPoints.AsNoTracking().FirstOrDefaultAsync(p => p.PointId == pointId);
            return point?.LocationName ?? "Unknown Location";
        }
    }
}