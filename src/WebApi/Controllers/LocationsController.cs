using BusTicketReservationSystem.Application.Services;
using BusTicketReservationSystem.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BusTicketReservationSystem.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LocationsController : ControllerBase
    {
        private readonly BusTicketDbContext _db;
        private readonly ILogger<LocationsController> _logger;

        public LocationsController(BusTicketDbContext db, ILogger<LocationsController> logger)
        {
            _db = db;
            _logger = logger;
        }

        // GET: /api/Locations?q=dhaka
        // Returns distinct origin and destination city names that match the query (case-insensitive)
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string q = "")
        {
            q = q?.Trim() ?? string.Empty;

            try
            {
                if (_db.Database.CanConnect())
                {
                    var origins = _db.Routes.AsQueryable();

                    if (!string.IsNullOrEmpty(q))
                    {
                        origins = origins.Where(r => EF.Functions.ILike(r.Origin, $"%{q}%") || EF.Functions.ILike(r.Destination, $"%{q}%"));
                    }

                    var results = await origins
                        .SelectMany(r => new[] { r.Origin, r.Destination })
                        .Distinct()
                        .OrderBy(x => x)
                        .ToListAsync();

                    if (results.Any())
                    {
                        return Ok(results);
                    }
                }

                return Ok(DemoFallback.GetLocations(q));
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to fetch locations with query '{Query}'", q);
                return Ok(DemoFallback.GetLocations(q));
            }
        }
    }
}
