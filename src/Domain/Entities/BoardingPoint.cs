// src/Domain/Entities/BoardingPoint.cs (NEW FILE)
using System;

namespace BusTicketReservationSystem.Domain.Entities
{
    public class BoardingPoint
    {
        public Guid PointId { get; set; }
        public Guid RouteId { get; set; }
        public string LocationName { get; set; } = default!;
        public TimeSpan DepartureTimeOffset { get; set; } // e.g., 00:00:00 (Start Time) or 01:30:00 (1.5 hours later)
        public bool IsDroppingPoint { get; set; } // true for dropping, false for boarding/pickup
        
        // Navigation
        public Route Route { get; set; } = default!;
    }
}