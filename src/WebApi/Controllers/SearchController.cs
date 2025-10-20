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
            // WebApi (Presentation) layer validation for required parameters
            if (string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(to))
            {
                return BadRequest(new { message = "Both 'from' and 'to' cities are required." });
            }

            // Call the Application Service (use case execution)
            var buses = await _searchService.SearchAvailableBusesAsync(from, to, journeyDate);

            if (buses == null || buses.Count == 0)
            {
                return NotFound(new { message = $"No buses found from {from} to {to} on {journeyDate.ToShortDateString()}." });
            }

            return Ok(buses);
        }
    }
}