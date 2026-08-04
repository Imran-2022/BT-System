
using BusTicketReservationSystem.Application.Contracts.Dtos;
using BusTicketReservationSystem.Application.Contracts.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
// Marks the class as an ASP.NET Core Web API controller, enabling automatic features like routing and model validation.
[Route("api/[controller]")]
public class BookingsController : ControllerBase // inheritance. 
{
    private readonly IBookingService _bookingService;

    // Inject the booking service
    public BookingsController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    // The controller doesn't handle business logic or database access itself.
    // It uses Dependency Injection (DI) to receive an instance of the IBookingService (the business layer). This ensures the controller remains lean, focusing only on HTTP concerns.

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