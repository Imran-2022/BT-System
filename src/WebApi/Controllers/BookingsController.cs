// src/WebApi/Controllers/BookingsController.cs

using BusTicketReservationSystem.Application.Contracts.Dtos;
using BusTicketReservationSystem.Application.Contracts.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    // 🎯 FIX: Changed ISearchService to IBookingService
    private readonly IBookingService _bookingService; 

    // Inject the new service
    public BookingsController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    // Full Route: POST /api/Bookings/BookSeat
    [HttpPost("BookSeat")]
    [ProducesResponseType(typeof(BookingResponseDto), 200)]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    public async Task<IActionResult> BookSeat([FromBody] BookSeatInputDto bookingDetails)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        // 🎯 FIX: Call the IBookingService.BookSeatAsync
        var response = await _bookingService.BookSeatAsync(bookingDetails);

        if (response.Status == "Failure")
        {
            // Return a 400 BadRequest if booking failed due to business rules
            return BadRequest(new { message = response.Message });
        }

        return Ok(response);
    }
}