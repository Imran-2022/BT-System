using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BusTicketReservationSystem.Application.Contracts.Dtos
{
    // DTO for a single seat being booked in the request
    public class SeatBookingItemDto
    {
        [Required]
        public string SeatNumber { get; set; } = default!;

        [Required]
        public decimal Price { get; set; }
        
        // NOTE: The frontend removed gender, so we remove it here to match the payload.
        // public char Gender { get; set; } 
    }

    // DTO for the entire booking request
    public class BookSeatInputDto
    {
        [Required]
        public Guid ScheduleId { get; set; }

        [Required]
        public string BoardingPoint { get; set; } = default!;

        [Required]
        public string DroppingPoint { get; set; } = default!;

        [Required]
        [Phone]
        public string MobileNumber { get; set; } = default!;

        [Required]
        public List<SeatBookingItemDto> SeatBookings { get; set; } = new List<SeatBookingItemDto>();

        // IMPORTANT: The frontend did not send journeyDate, but your booking logic often requires it.
        // For simplicity, we are omitting it to match your current frontend payload. 
        // If your database requires this for validation, you'd need to add it back to the frontend and here.
    }

    // DTO for the successful booking response
    public class BookingResponseDto
    {
        public Guid BookingId { get; set; }
        public string Message { get; set; } = default!;
        public string Status { get; set; } = default!; // "Success" or "Failure"
    }
}