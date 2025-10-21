using System;
using System.Collections.Generic;

namespace BusTicketReservationSystem.Application.Contracts.Dtos
{
    public class AvailableBusDto
    {
        public Guid BusScheduleId { get; set; }
        public string CompanyName { get; set; } = default!;
        public string BusName { get; set; }  = default!;
        public string BusType { get; set; } = default!;
        public TimeSpan StartTime { get; set; } = default!;
        public string BoardingPoint { get; set; } = default!;
        public TimeSpan ArrivalTime { get; set; }
        public string DroppingPoint { get; set; } = default!;
        public int SeatsLeft { get; set; } 
        public decimal Price { get; set; }
        public string CancellationPolicy { get; set; } = default!;
    }
}