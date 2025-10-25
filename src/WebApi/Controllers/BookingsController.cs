
using BusTicketReservationSystem.Application.Contracts.Dtos;
using BusTicketReservationSystem.Application.Contracts.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;

    // Inject the booking service
    public BookingsController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    // POST: /api/Bookings/BookSeat
    // Handles seat booking requests
    [HttpPost("BookSeat")]
    [ProducesResponseType(typeof(BookingResponseDto), 200)]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    public async Task<IActionResult> BookSeat([FromBody] BookSeatInputDto bookingDetails)
    {
        // Validate incoming request
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var response = await _bookingService.BookSeatAsync(bookingDetails);

        // Return 400 if booking failed due to business rules
        if (response.Status == "Failure")
        {
            // Return 200 if booking succeeded
            return BadRequest(new { message = response.Message });
        }
        // Return 200 if booking succeeded
        return Ok(response);
    }
}