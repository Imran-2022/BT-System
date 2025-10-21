using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusTicketReservationSystem.Application.Contracts.Dtos
{
    public class SeatStatusDto
    {
        public string SeatNumber { get; set; } = default!;
        // 1: Available, 2: Blocked, 3: Booked (Generic), 4: Sold Male, 5: Sold Female
        public int Status { get; set; } 
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }
    }
}