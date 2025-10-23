using System;
using System.Collections.Generic;

namespace BusTicketReservationSystem.Domain.Entities
{
    public class Route
    {
        public Guid RouteId { get; set; }
        public string Origin { get; set; } = default!; // Dhaka
        public string Destination { get; set; } = default!; // Rajshahi
        
        // Collection navigation property (used in search logic)
        public ICollection<BusSchedule> BusSchedules { get; set; } = new List<BusSchedule>();
        
        // 🎯 FIX: ADDED MISSING NAVIGATION PROPERTY
        public ICollection<BoardingPoint> BoardingPoints { get; set; } = new List<BoardingPoint>();
    }
}