// src/Application.Contracts/Dtos/AvailableBusDto.cs
using System;
using System.Collections.Generic;

namespace BusTicketReservationSystem.Application.Contracts.Dtos
{
    public class PointOptionDto
    {
        public Guid PointId { get; set; }
        public string LocationName { get; set; } = default!;
        public TimeSpan Time { get; set; } // Actual time at this point
    }
    
    public class AvailableBusDto
    {
        public Guid BusScheduleId { get; set; }
        public string CompanyName { get; set; } = default!;
        public string BusName { get; set; }  = default!;
        public string BusType { get; set; } = default!;
        public TimeSpan StartTime { get; set; } = default!;
        public TimeSpan ArrivalTime { get; set; }
        public int SeatsLeft { get; set; }
        public decimal Price { get; set; }
        public string CancellationPolicy { get; set; } = default!;
        // ... existing properties ...
        public Guid LayoutId { get; set; } // The ID of the layout
        public string SeatConfiguration { get; set; } = default!; // The string defining the layout
        // 🎯 CHANGES: Boarding/Dropping points are now lists of options
        public List<PointOptionDto> BoardingPoints { get; set; } = new List<PointOptionDto>();
        public List<PointOptionDto> DroppingPoints { get; set; } = new List<PointOptionDto>();
        
        // Seat layout for Search results (will be empty)
        public List<SeatStatusDto> SeatLayout { get; set; } = new List<SeatStatusDto>();
    }
}