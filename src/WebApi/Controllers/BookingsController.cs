using BusTicketReservationSystem.Application.Contracts.Dtos;
using BusTicketReservationSystem.Application.Contracts.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

// 1. Define the base route: /api/Bookings
[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly ISearchService _searchService;

    // Inject the service you need (ISearchService contains BookSeatsAsync)
    public BookingsController(ISearchService searchService)
    {
        _searchService = searchService;
    }

    // 2. Define the action route: /BookSeat
    // Full Route: POST /api/Bookings/BookSeat (Matches frontend URL)
    [HttpPost("BookSeat")]
    [ProducesResponseType(typeof(BookingResponseDto), 200)]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    public async Task<IActionResult> BookSeat([FromBody] BookSeatInputDto bookingDetails) // Renamed action to BookSeat for clarity
    {
        // Model validation (checks [Required] attributes in the DTO)
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        // Call the Application Service
        var response = await _searchService.BookSeatsAsync(bookingDetails);

        if (response.Status == "Failure")
        {
            // Return a 400 BadRequest if booking failed due to business rules
            return BadRequest(new { message = response.Message });
        }

        return Ok(response);
    }
}