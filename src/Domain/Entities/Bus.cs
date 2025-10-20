using System;

namespace BusTicketReservationSystem.Domain.Entities
{
    public class Bus
    {
        public Guid BusId { get; set; }
        public string CompanyName { get; set; }
        public string BusName { get; set; }
        public string BusType { get; set; } // AC/Non-AC
        public int TotalSeats { get; set; } = 40;
    }
}