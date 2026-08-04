using BusTicketReservationSystem.Application.Contracts.Dtos;
using BusTicketReservationSystem.Application.Contracts.Repositories;
using BusTicketReservationSystem.Application.Contracts.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BusTicketReservationSystem.Application.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBusScheduleRepository _busScheduleRepository;
        private readonly IBookingRepository _bookingRepository;

        // _busScheduleRepository: Used only for looking up current data (like checking which seats are already booked).
        // _bookingRepository: Used only for saving the final booking transaction.

        public BookingService(IBusScheduleRepository busScheduleRepository, IBookingRepository bookingRepository)
        {
            _busScheduleRepository = busScheduleRepository;
            _bookingRepository = bookingRepository;
        }

        public Task<AvailableBusDto?> GetSeatPlanAsync(Guid busScheduleId)
        {
            // Fetch schedule details including seat plan
            return _busScheduleRepository.GetBusScheduleAndSeatDetailsByIdAsync(busScheduleId);
        }

        public async Task<BookingResponseDto> BookSeatAsync(BookSeatInputDto input)
        {
            // Fail if no seats were selected
            if (input.SeatBookings == null || input.SeatBookings.Count == 0)
            {
                return new BookingResponseDto
                {
                    BookingId = Guid.Empty,
                    Status = "Failure",
                    Message = "No seats selected for booking."
                };
            }

            // Check for already booked seats
            var bookedSeats = await _busScheduleRepository.GetBookedSeatNumbersAsync(input.ScheduleId);

            if (bookedSeats.Count == 0)
            {
                // If there are no booked seats returned and the schedule is a fallback/demo schedule,
                // allow booking on known demo seat layout data.
                var fallbackSchedule = DemoFallback.GetScheduleDetails(input.ScheduleId);
                if (fallbackSchedule != null)
                {
                    var validSeatNumbers = fallbackSchedule.SeatLayout.Select(s => s.SeatNumber).ToHashSet(StringComparer.OrdinalIgnoreCase);
                    if (!input.SeatBookings.All(seat => validSeatNumbers.Contains(seat.SeatNumber)))
                    {
                        return new BookingResponseDto
                        {
                            Status = "Failure",
                            Message = "One or more selected seats do not exist for this schedule."
                        };
                    }

                    return new BookingResponseDto
                    {
                        BookingId = Guid.NewGuid(),
                        Status = "Success",
                        Message = $"Demo booking confirmed for {input.SeatBookings.Count} seats."
                    };
                }
            }

            foreach (var requestedSeat in input.SeatBookings)
            {
                if (bookedSeats.Contains(requestedSeat.SeatNumber))
                {
                    return new BookingResponseDto
                    {
                        Status = "Failure",
                        Message = $"Seat {requestedSeat.SeatNumber} is already booked."
                    };
                }
            }

            // Proceed with booking transaction
            return await _bookingRepository.BookSeatsTransactionAsync(input);
        }
    }
}