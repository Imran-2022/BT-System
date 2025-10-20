using System;

namespace BusTicketReservationSystem.Domain.Entities
{
    public class Route
    {
        public Guid RouteId { get; set; }
        public string Origin { get; set; } // Dhaka
        public string Destination { get; set; } // Rajshahi
        // Collection navigation property (used in search logic)
        public ICollection<BusSchedule> BusSchedules { get; set; } = new List<BusSchedule>();
    }
}