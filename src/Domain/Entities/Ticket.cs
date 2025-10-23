// src/Domain/Entities/Ticket.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusTicketReservationSystem.Domain.Entities
{
    public class Ticket
    {
        public Guid TicketId { get; set; }
        public Guid BusScheduleId { get; set; } // Foreign Key to BusSchedule
        public DateTime BookingDate { get; set; }
        
        // Finalized route info
        public string BoardingPoint { get; set; } = default!;
        public string DroppingPoint { get; set; } = default!;
        
        // Consolidated Passenger Info (For lookup convenience)
        public string MobileNumber { get; set; } = default!;
        
        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalPrice { get; set; }
        
        // Navigation Properties
        public ICollection<SeatStatus> BookedSeats { get; set; } = new List<SeatStatus>();
        
        // Relationship definition for EF Core
        public BusSchedule BusSchedule { get; set; } = default!; 
    }
}