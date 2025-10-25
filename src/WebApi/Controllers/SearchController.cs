using BusTicketReservationSystem.Application.Contracts.Dtos;
using BusTicketReservationSystem.Application.Contracts.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusTicketReservationSystem.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly ISearchService _searchService;

        public SearchController(ISearchService searchService)
        {
            _searchService = searchService;
        }

        // GET: /api/Search?from=CityA&to=CityB&journeyDate=2025-10-25
        // Searches for available bus schedules based on From, To, and Journey Date
        [HttpGet]
        public async Task<ActionResult<List<AvailableBusDto>>> Search(
            [FromQuery] string from,
            [FromQuery] string to,
            [FromQuery] DateTime journeyDate)
        {
            // Validate required query parameters
            if (string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(to))
            {
                return BadRequest(new { message = "Both 'from' and 'to' cities are required." });
            }

            var buses = await _searchService.SearchAvailableBusesAsync(from, to, journeyDate);

            // Return 404 if no buses found
            if (buses == null || buses.Count == 0)
            {
                return NotFound(new { message = $"No buses found from {from} to {to} on {journeyDate.ToShortDateString()}." });
            }

            return Ok(buses);
        }

        // GET: /api/Search/{id}
        // Retrieves single bus schedule details for seat viewing
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(AvailableBusDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Get(Guid id)
        {
            // Validate the schedule ID
            if (id == Guid.Empty)
            {
                return BadRequest(new { message = "Schedule ID is required." });
            }
            var schedule = await _searchService.GetScheduleAndSeatDetailsAsync(id);
            
            // Return 404 if schedule not found
            if (schedule == null)
            {
                return NotFound(new { message = $"Bus schedule with ID {id} not found." });
            }

            return Ok(schedule);
        }
    }
}