using BusTicketReservationSystem.Application.Contracts.Dtos;
using BusTicketReservationSystem.Application.Contracts.Repositories;
using BusTicketReservationSystem.Application.Contracts.Services;
using System;
using System.Threading.Tasks;

namespace BusTicketReservationSystem.Application.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBusScheduleRepository _busScheduleRepository;
        private readonly IBookingRepository _bookingRepository;

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