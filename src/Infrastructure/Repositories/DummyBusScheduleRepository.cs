// using BusTicketReservationSystem.Application.Contracts.Dtos;
// using BusTicketReservationSystem.Application.Contracts.Repositories;
// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;

// namespace BusTicketReservationSystem.Infrastructure.Repositories
// {
//     public class DummyBusScheduleRepository : IBusScheduleRepository
//     {
//         private static readonly Guid ScheduleId1 = new Guid("4b1e9f2a-7c8d-4f0e-9b2c-3d4a5e6f7b8c");
//         private static readonly Guid ScheduleId2 = new Guid("7c8d1e9f-4f0e-9b2c-3d4a-5e6f7b8c4b1e");

//         private readonly List<AvailableBusDto> _allSchedules = new List<AvailableBusDto>
//         {
//             new AvailableBusDto {
//                 BusScheduleId = ScheduleId1,
//                 CompanyName = "National Travels",
//                 BusName = "99-DHA-CHA",
//                 BusType = "NON AC",
//                 CancellationPolicy = "Cancellation policy applicable.",
//                 StartTime = new TimeSpan(6, 0, 0),
//                 BoardingPoint = "Kallyanpur",
//                 ArrivalTime = new TimeSpan(13, 30, 0),
//                 DroppingPoint = "Chapai Nawabganj",
//                 SeatsLeft = 36, // Seats Left calculated dynamically (here, hardcoded) [cite: 44]
//                 // [cite_start]SeatsLeft = 36, 
//                 Price = 700.00m
//             },
//             new AvailableBusDto {
//                 BusScheduleId = ScheduleId2,
//                 CompanyName = "Hanif Enterprise",
//                 BusName = "68-RAJ-CHAPA",
//                 BusType = "AC Deluxe",
//                 CancellationPolicy = "Strict cancellation policy.",
//                 StartTime = new TimeSpan(7, 30, 0),
//                 BoardingPoint = "Mohakhali",
//                 ArrivalTime = new TimeSpan(15, 0, 0),
//                 DroppingPoint = "Rajshahi Counter",
//                 SeatsLeft = 40,
//                 Price = 1200.00m
//             }
//         };

//         public Task<List<AvailableBusDto>> FindAvailableBusesAsync(string from, string to, DateTime journeyDate)
//         {
//             //[cite_start]// Simulate successful search ONLY for the required route: Dhaka to Rajshahi [cite: 131]
//             if (from.Equals("Dhaka", StringComparison.OrdinalIgnoreCase) && 
//                 to.Equals("Rajshahi", StringComparison.OrdinalIgnoreCase))
//             {
//                 // NOTE: Ignoring journeyDate for this dummy implementation.
//                 return Task.FromResult(_allSchedules.ToList());
//             }

//             // Return empty list for any other route
//             return Task.FromResult(new List<AvailableBusDto>());
//         }
//     }
// }