using System;

namespace BusTicketReservationSystem.Domain.Entities
{
    public class Bus
    {
        public Guid BusId { get; set; }
        public string CompanyName { get; set; } = default!;
        public string BusName { get; set; } = default!;
        public string BusType { get; set; } = default!; // AC/Non-AC
        public int TotalSeats { get; set; } = 40;
    }
}