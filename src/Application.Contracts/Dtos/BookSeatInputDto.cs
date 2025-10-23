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

        [Required]
        public string PassengerName { get; set; } = default!;
        
    }

    // DTO for the entire booking request
    public class BookSeatInputDto
    {
        [Required]
        public Guid ScheduleId { get; set; }
        
        // 🎯 CHANGE: Use Point IDs for definitive booking (optional for now, but recommended)
        [Required]
        public Guid BoardingPointId { get; set; }
        [Required]
        public Guid DroppingPointId { get; set; }

        [Required]
        [Phone]
        public string MobileNumber { get; set; } = default!;

        [Required]
        public List<SeatBookingItemDto> SeatBookings { get; set; } = new List<SeatBookingItemDto>();
    }

    // DTO for the successful booking response
    public class BookingResponseDto
    {
        public Guid BookingId { get; set; }
        public string Message { get; set; } = default!;
        public string Status { get; set; } = default!; // "Success" or "Failure"
    }
}