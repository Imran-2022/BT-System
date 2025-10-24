using Xunit;
using Moq;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using BusTicketReservationSystem.Application.Contracts.Dtos;
using BusTicketReservationSystem.Application.Contracts.Repositories;
using BusTicketReservationSystem.Application.Services;

// NOTE: You may need to adjust the namespace based on your actual project structure if the compiler complains.
// Example: namespace BusTicketReservationSystem.Application.Tests 
public class BookingServiceTests
{
    private readonly Mock<IBusScheduleRepository> _mockBusScheduleRepo;
    private readonly Mock<IBookingRepository> _mockBookingRepo;
    private readonly BookingService _bookingService;

    public BookingServiceTests()
    {
        // ARRANGE SETUP: Initialize both mock repositories needed
        _mockBusScheduleRepo = new Mock<IBusScheduleRepository>();
        _mockBookingRepo = new Mock<IBookingRepository>();
        // 🎯 NEW DEFAULT SETUP ADDED HERE:
        // Set up the GetBookedSeatNumbersAsync to return an empty list by default.
        // This makes the existing "successful" tests pass, as the new validation
        // will find no booked seats.
        _mockBusScheduleRepo.Setup(r => r.GetBookedSeatNumbersAsync(It.IsAny<Guid>()))
                            .ReturnsAsync(new List<string>());
        // ARRANGE SETUP: Initialize the BookingService, injecting both Mocks
        _bookingService = new BookingService(_mockBusScheduleRepo.Object, _mockBookingRepo.Object);
    }

    [Fact]
    public async Task BookSeatAsync_ReturnsFailure_WhenNoSeatsSelected()
    {
        // ARRANGE: Create an input DTO with an empty seat list
        var input = new BookSeatInputDto
        {
            ScheduleId = Guid.NewGuid(),
            SeatBookings = new List<SeatBookingItemDto>() // Empty list
        };

        // ACT
        var result = await _bookingService.BookSeatAsync(input);

        // ASSERT: Verify the result is a failure and has the correct message
        Assert.Equal("Failure", result.Status);
        Assert.Contains("No seats selected", result.Message);

        // Verify the service stopped the call and did NOT touch the booking repository
        _mockBookingRepo.Verify(r => r.BookSeatsTransactionAsync(It.IsAny<BookSeatInputDto>()), Times.Never);
    }

    [Fact]
    public async Task BookSeatAsync_DelegatesToRepository_OnValidInput()
    {
        // ARRANGE: 
        var input = new BookSeatInputDto
        {
            ScheduleId = Guid.NewGuid(),
            SeatBookings = new List<SeatBookingItemDto>
            {
                new SeatBookingItemDto { SeatNumber = "A1", PassengerName = "Test", Price = 500.00M }
            }
        };
        var successResponse = new BookingResponseDto
        {
            BookingId = Guid.NewGuid(),
            Status = "Success"
        };

        // Tell the Mock repository what to return on a successful transaction
        _mockBookingRepo.Setup(r => r.BookSeatsTransactionAsync(input))
                        .ReturnsAsync(successResponse);

        // ACT
        var result = await _bookingService.BookSeatAsync(input);

        // ASSERT: 
        Assert.Equal("Success", result.Status);
        Assert.NotEqual(Guid.Empty, result.BookingId);

        // 🎯 IMPORTANT: Add a verification for the availability check
        // This ensures that even the successful path performs the check.
        _mockBusScheduleRepo.Verify(r => r.GetBookedSeatNumbersAsync(input.ScheduleId), Times.Once);
    }
    [Fact]
    public async Task BookSeatAsync_ReturnsFailure_WhenSeatIsAlreadyBooked()
    {
        // ARRANGE: Input with a seat we know is taken
        var scheduleId = Guid.NewGuid();
        var bookedSeatNumber = "B5";

        var input = new BookSeatInputDto
        {
            ScheduleId = scheduleId,
            SeatBookings = new List<SeatBookingItemDto>
        {
            new SeatBookingItemDto { SeatNumber = bookedSeatNumber, PassengerName = "Test Passenger", Price = 500.00M }
        }
        };

        // 1. Tell the Mock Bus Schedule Repository that B5 is already booked.
        var currentBookedSeats = new List<string> { bookedSeatNumber, "A1", "C2" };

        _mockBusScheduleRepo.Setup(r => r.GetBookedSeatNumbersAsync(scheduleId))
                            .ReturnsAsync(currentBookedSeats);

        // ACT: Execute the method under test
        var result = await _bookingService.BookSeatAsync(input);

        // ASSERT: Verify the outcome is a failure
        Assert.Equal("Failure", result.Status);
        Assert.Contains($"Seat {bookedSeatNumber} is already booked", result.Message);

        // CRUCIAL ASSERT: Verify the service stopped the process and did NOT attempt a transaction
        _mockBookingRepo.Verify(r => r.BookSeatsTransactionAsync(It.IsAny<BookSeatInputDto>()), Times.Never);

        // Verify the availability check WAS performed exactly once
        _mockBusScheduleRepo.Verify(r => r.GetBookedSeatNumbersAsync(scheduleId), Times.Once);
    }
}