using System;

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
        public Route Route { get; set; }
        public Bus Bus { get; set; }
    }
}