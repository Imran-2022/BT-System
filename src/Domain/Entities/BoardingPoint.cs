using System;

namespace BusTicketReservationSystem.Domain.Entities
{
    public class BoardingPoint
    {
        public Guid PointId { get; set; }             // Unique identifier for the boarding point
        public Guid RouteId { get; set; }             // Associated route identifier
        public string LocationName { get; set; } = default!;  // Name of the location
        public TimeSpan DepartureTimeOffset { get; set; }     // Time offset from route start (e.g., 00:00:00 for start, 01:30:00 for 1.5 hours later)
        public bool IsDroppingPoint { get; set; }     // true if this point is a dropping point; false if boarding/pickup

        // Navigation property to the associated route
        public Route Route { get; set; } = default!;
    }
}
