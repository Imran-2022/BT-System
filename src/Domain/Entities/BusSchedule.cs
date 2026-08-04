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
        // SeatStatuses: The folder containing all the seat availability information for this one bus trip.
        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();               // Booked tickets
        // Once a seat is successfully booked, a Ticket entity is created. This collection is a list of all the actual booked tickets for this journey. You would use this to see how many people are confirmed to be traveling.
    }
}
