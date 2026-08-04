using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BusTicketReservationSystem.Infrastructure.Data;

namespace BusTicketReservationSystem.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly BusTicketDbContext _db;
        private readonly ILogger<HealthController> _logger;

        public HealthController(BusTicketDbContext db, ILogger<HealthController> logger)
        {
            _db = db;
            _logger = logger;
        }

        // GET: /api/health
        // Returns 200 if the API can reach the database, otherwise 503.
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var canConnect = await _db.Database.CanConnectAsync();
                if (canConnect)
                {
                    return Ok(new { status = "ok", database = "connected" });
                }
                else
                {
                    _logger.LogWarning("Health check: database cannot be reached (CanConnect returned false).");
                    return StatusCode(503, new { status = "unavailable", database = "cannot connect" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Health check failed while attempting to contact the database.");
                return StatusCode(503, new { status = "unavailable", message = "database error" });
            }
        }
    }
}
