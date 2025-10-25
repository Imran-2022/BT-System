using System;

namespace BusTicketReservationSystem.Domain.Entities
{
    public class Bus
    {
        public Guid BusId { get; set; }                     // Unique identifier for the bus
        public Guid BusSeatLayoutId { get; set; }           // Reference to the fixed seat layout
        public string CompanyName { get; set; } = default!; // Bus operator/company
        public string BusName { get; set; } = default!;    // Bus display name or identifier
        public string BusType { get; set; } = default!;    // AC / Non-AC
        public decimal BasePrice { get; set; }             // Base fare for the bus

        // Navigation property to the seat layout
        public BusSeatLayout Layout { get; set; } = default!;
    }
}
