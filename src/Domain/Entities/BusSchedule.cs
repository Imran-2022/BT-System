using System;
using System.Collections.Generic;

namespace BusTicketReservationSystem.Domain.Entities
{
    public class BusSchedule
    {
        public Guid BusScheduleId { get; set; }    // Unique identifier for the bus schedule
        public Guid RouteId { get; set; }          // Associated route
        public Guid BusId { get; set; }            // Assigned bus
        public DateTime JourneyDate { get; set; }  // Date of travel
        public TimeSpan StartTime { get; set; }    // Scheduled start time

        // Navigation properties
        public Route Route { get; set; } = default!;
        public Bus Bus { get; set; } = default!;

        public ICollection<SeatStatus> SeatStatuses { get; set; } = new List<SeatStatus>(); // Seat availability for this schedule
        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();               // Booked tickets
    }
}
