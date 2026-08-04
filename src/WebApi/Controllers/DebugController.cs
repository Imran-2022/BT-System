using BusTicketReservationSystem.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
    }
}
