// src/Domain/Entities/Bus.cs
using System;

namespace BusTicketReservationSystem.Domain.Entities
{
    public class Bus
    {
        public Guid BusId { get; set; }
        public Guid BusSeatLayoutId { get; set; } // 🎯 NEW: Link to the fixed layout
        public string CompanyName { get; set; } = default!;
        public string BusName { get; set; } = default!;
        public string BusType { get; set; } = default!; // AC/Non-AC

        // Navigation properties
        public BusSeatLayout Layout { get; set; } = default!; // 🎯 NEW
    }
}