// src/Application/Services/BookingService.cs (NEW FILE)
using BusTicketReservationSystem.Application.Contracts.Dtos;
using BusTicketReservationSystem.Application.Contracts.Repositories;
using BusTicketReservationSystem.Application.Contracts.Services;
using System;
using System.Threading.Tasks;

namespace BusTicketReservationSystem.Application.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBusScheduleRepository _busScheduleRepository; // Used for fetching details
        private readonly IBookingRepository _bookingRepository; // Used for transaction

        public BookingService(IBusScheduleRepository busScheduleRepository, IBookingRepository bookingRepository)
        {
            _busScheduleRepository = busScheduleRepository;
            _bookingRepository = bookingRepository;
        }

        // 🎯 Implements GetSeatPlanAsync (uses the same underlying repository method)
        public Task<AvailableBusDto?> GetSeatPlanAsync(Guid busScheduleId)
        {
            // Renamed for better DDD: Schedule details are fetched here, including the seat plan.
            return _busScheduleRepository.GetBusScheduleAndSeatDetailsByIdAsync(busScheduleId);
        }

        // 🎯 Implements BookSeatAsync
        public async Task<BookingResponseDto> BookSeatAsync(BookSeatInputDto input)
        {
            // Application Rule: Check if any seats were selected
            if (input.SeatBookings == null || input.SeatBookings.Count == 0)
            {
                return new BookingResponseDto 
                { 
                    BookingId = Guid.Empty, 
                    Status = "Failure", 
                    Message = "No seats selected for booking." 
                };
            }

            // Delegate the booking transaction to the dedicated repository
            return await _bookingRepository.BookSeatsTransactionAsync(input);
        }
    }
}