using System;
using System.ComponentModel.DataAnnotations.Schema;
// Assuming your BaseEntity and BusSchedule are in these namespaces
// using BusTicketReservationSystem.Domain.Common;
// using BusTicketReservationSystem.Domain.Entities; 

namespace BusTicketReservationSystem.Domain.Entities
{
    public class SeatStatus
    {
        public Guid SeatStatusId { get; set; } // Primary Key
        
        public Guid BusScheduleId { get; set; } // Foreign Key
        public BusSchedule BusSchedule { get; set; } = default!;

        public string SeatNumber { get; set; } = default!; 

        public int Status { get; set; } 

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }
    }

    // Optional enum for clarity in C#
    public enum SeatStatusCode
    {
        Available = 1,
        Blocked = 2,
        Booked = 3,
        SoldMale = 4, 
        SoldFemale = 5,
    }
}