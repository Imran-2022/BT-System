using System;
using System.Collections.Generic;

namespace BusTicketReservationSystem.Domain.Entities
{
    public class BusSchedule
    {
        public Guid BusScheduleId { get; set; }
        public Guid RouteId { get; set; }
        public Guid BusId { get; set; }
        public DateTime JourneyDate { get; set; } // Date of travel
        public TimeSpan StartTime { get; set; }

        // Navigation properties (Foreign Keys)
        public Route Route { get; set; } = default!;
        public Bus Bus { get; set; } = default!;
        // --- NEW NAVIGATION PROPERTY ---
        public ICollection<SeatStatus> SeatStatuses { get; set; } = new List<SeatStatus>();
    }
}