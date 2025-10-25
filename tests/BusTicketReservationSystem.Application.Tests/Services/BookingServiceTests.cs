using Xunit;
using Moq;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using BusTicketReservationSystem.Application.Contracts.Dtos;
using BusTicketReservationSystem.Application.Contracts.Repositories;
using BusTicketReservationSystem.Application.Services;

public class BookingServiceTests
{
    private readonly Mock<IBusScheduleRepository> _mockBusScheduleRepo;
    private readonly Mock<IBookingRepository> _mockBookingRepo;
    private readonly BookingService _bookingService;

    public BookingServiceTests()
    {
        // Initialize mock repositories
        _mockBusScheduleRepo = new Mock<IBusScheduleRepository>();
        _mockBookingRepo = new Mock<IBookingRepository>();

        // Default setup: No seats are booked
        _mockBusScheduleRepo.Setup(r => r.GetBookedSeatNumbersAsync(It.IsAny<Guid>()))
                            .ReturnsAsync(new List<string>());

        // Initialize service with mocked dependencies
        _bookingService = new BookingService(_mockBusScheduleRepo.Object, _mockBookingRepo.Object);
    }

    [Fact]
    public async Task BookSeatAsync_ReturnsFailure_WhenNoSeatsSelected()
    {
        // Input DTO with no seats selected
        var input = new BookSeatInputDto
        {
            ScheduleId = Guid.NewGuid(),
            SeatBookings = new List<SeatBookingItemDto>()
        };

        // Execute
        var result = await _bookingService.BookSeatAsync(input);

        // Verify failure due to no seats selected
        Assert.Equal("Failure", result.Status);
        Assert.Contains("No seats selected", result.Message);

        // Ensure booking repository is not called
        _mockBookingRepo.Verify(r => r.BookSeatsTransactionAsync(It.IsAny<BookSeatInputDto>()), Times.Never);
    }

    [Fact]
    public async Task BookSeatAsync_DelegatesToRepository_OnValidInput()
    {
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

        // Mock successful booking transaction
        _mockBookingRepo.Setup(r => r.BookSeatsTransactionAsync(input))
                        .ReturnsAsync(successResponse);

        var result = await _bookingService.BookSeatAsync(input);

        // Verify success and valid BookingId
        Assert.Equal("Success", result.Status);
        Assert.NotEqual(Guid.Empty, result.BookingId);

        // Verify seat availability check is performed
        _mockBusScheduleRepo.Verify(r => r.GetBookedSeatNumbersAsync(input.ScheduleId), Times.Once);
    }

    [Fact]
    public async Task BookSeatAsync_ReturnsFailure_WhenSeatIsAlreadyBooked()
    {
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

        // Mock a seat that is already booked
        var currentBookedSeats = new List<string> { bookedSeatNumber, "A1", "C2" };
        _mockBusScheduleRepo.Setup(r => r.GetBookedSeatNumbersAsync(scheduleId))
                            .ReturnsAsync(currentBookedSeats);

        var result = await _bookingService.BookSeatAsync(input);

        // Verify failure due to seat already booked
        Assert.Equal("Failure", result.Status);
        Assert.Contains($"Seat {bookedSeatNumber} is already booked", result.Message);

        // Ensure no booking transaction is attempted
        _mockBookingRepo.Verify(r => r.BookSeatsTransactionAsync(It.IsAny<BookSeatInputDto>()), Times.Never);

        // Verify seat availability check is performed
        _mockBusScheduleRepo.Verify(r => r.GetBookedSeatNumbersAsync(scheduleId), Times.Once);
    }
}
