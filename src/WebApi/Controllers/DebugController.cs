using BusTicketReservationSystem.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace BusTicketReservationSystem.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DebugController : ControllerBase
    {
        private readonly BusTicketDbContext _db;
        public DebugController(BusTicketDbContext db)
        {
            _db = db;
        }

        [HttpGet("stats")]
        public async Task<IActionResult> Stats()
        {
            try
            {
                var routes = await _db.Routes.CountAsync();
                var schedules = await _db.BusSchedules.CountAsync();
                var buses = await _db.Buses.CountAsync();
                var seatLayouts = await _db.BusSeatLayouts.CountAsync();
                var boardingPoints = await _db.BoardingPoints.CountAsync();
                var tickets = await _db.Tickets.CountAsync();
                var seatStatuses = await _db.SeatStatuses.CountAsync();

                return Ok(new
                {
                    DatabaseConnected = await _db.Database.CanConnectAsync(),
                    routes,
                    schedules,
                    buses,
                    seatLayouts,
                    boardingPoints,
                    tickets,
                    seatStatuses
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, type = ex.GetType().FullName, stack = ex.StackTrace });
            }
        }

        // POST: /api/Debug/force-reseed
        // WARNING: Destructive. Drops seeded schedules, buses, tickets and seat statuses,
        // then re-runs the DatabaseSeeder. Intended for development only.
        [HttpPost("force-reseed")]
        public async Task<IActionResult> ForceReseed()
        {
            try
            {
                // Delete in dependency order
                _db.SeatStatuses.RemoveRange(_db.SeatStatuses);
                _db.Tickets.RemoveRange(_db.Tickets);
                _db.BusSchedules.RemoveRange(_db.BusSchedules);
                _db.Buses.RemoveRange(_db.Buses);
                _db.BoardingPoints.RemoveRange(_db.BoardingPoints);
                _db.Routes.RemoveRange(_db.Routes);

                await _db.SaveChangesAsync();

                // Re-run seeder (uses service provider to create scope)
                await DatabaseSeeder.SeedAsync(HttpContext.RequestServices);

                return Ok(new { message = "Force reseed completed." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, type = ex.GetType().FullName });
            }
        }
    }
}
