using System;
using System.ComponentModel.DataAnnotations.Schema;

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

        //  decimal(18, 2) is a standard database type that specifies:
        // 18 (Precision): The total number of digits the number can have (both before and after the decimal point).
        // 2 (Scale): The number of digits to the right of the decimal point.

        public decimal Price { get; set; }
        // Passenger details linked to the seat status upon booking
        public string? PassengerName { get; set; }
        public string? MobileNumber { get; set; }
        public Guid? TicketId { get; set; } // Link to the booking ticket
    }

    // Optional enum for clarity in C#
    public enum SeatStatusCode
    {
        Available = 1,
        Blocked = 2,
        Booked = 3,
    }
}