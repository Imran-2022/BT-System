// src/Domain/Entities/BusSeatLayout.cs (NEW FILE)
using System;
using System.Collections.Generic;

namespace BusTicketReservationSystem.Domain.Entities
{
    public class BusSeatLayout
    {
        public Guid BusSeatLayoutId { get; set; }
        public string LayoutName { get; set; } = default!; // e.g., "2x2 Standard", "2x1 AC"
        public int SeatsPerRowCount { get; set; } = 4; // 2+2 layout
        public string SeatConfiguration { get; set; } = default!; // e.g., "A1,A2,A3,A4;B1,B2,B3,B4;..."
        public int TotalSeats { get; set; }
        
        // Navigation
        public ICollection<Bus> Buses { get; set; } = new List<Bus>();
    }
}