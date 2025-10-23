// src/WebApi/Controllers/SearchController.cs

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

        /// <summary>
        /// Searches for available bus schedules by From, To, and Journey Date.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<AvailableBusDto>>> Search(
            [FromQuery] string from,
            [FromQuery] string to,
            [FromQuery] DateTime journeyDate)
        {
            // ... (Search logic remains the same)

            if (string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(to))
            {
                return BadRequest(new { message = "Both 'from' and 'to' cities are required." });
            }

            var buses = await _searchService.SearchAvailableBusesAsync(from, to, journeyDate);

            if (buses == null || buses.Count == 0)
            {
                return NotFound(new { message = $"No buses found from {from} to {to} on {journeyDate.ToShortDateString()}." });
            }

            return Ok(buses);
        }

        // ------------------------------------------------------------------
        // NEW ENDPOINT: Get single bus schedule details by ID (for View Seats)
        // Route: GET api/Search/{id}
        // ------------------------------------------------------------------
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(AvailableBusDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Get(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest(new { message = "Schedule ID is required." });
            }

            // 🎯 FIX: Call the renamed method GetScheduleAndSeatDetailsAsync
            var schedule = await _searchService.GetScheduleAndSeatDetailsAsync(id);

            if (schedule == null)
            {
                return NotFound(new { message = $"Bus schedule with ID {id} not found." });
            }

            return Ok(schedule);
        }
    }
}