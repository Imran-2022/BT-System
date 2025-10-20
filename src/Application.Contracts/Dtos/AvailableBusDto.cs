using System;
using System.Collections.Generic;

namespace BusTicketReservationSystem.Application.Contracts.Dtos
{
    public class AvailableBusDto
    {
        public Guid BusScheduleId { get; set; }
        public string CompanyName { get; set; }
        public string BusName { get; set; } 
        public string BusType { get; set; }
        public TimeSpan StartTime { get; set; }
        public string BoardingPoint { get; set; }
        public TimeSpan ArrivalTime { get; set; }
        public string DroppingPoint { get; set; }
        public int SeatsLeft { get; set; } 
        public decimal Price { get; set; }
        public string CancellationPolicy { get; set; }
    }
}