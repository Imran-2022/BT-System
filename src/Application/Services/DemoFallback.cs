using System;
using System.Collections.Generic;
using System.Linq;
using BusTicketReservationSystem.Application.Contracts.Dtos;

namespace BusTicketReservationSystem.Application.Services
{
    public static class DemoFallback
    {
        private static readonly (Guid RouteId, string Origin, string Destination)[] Routes = new[] {
            (Guid.Parse("10000000-0000-0000-0000-000000000001"), "Dhaka", "Rajshahi"),
            (Guid.Parse("10000000-0000-0000-0000-000000000002"), "Dhaka", "Dinajpur"),
            (Guid.Parse("10000000-0000-0000-0000-000000000003"), "Dinajpur", "Rangpur"),
            (Guid.Parse("10000000-0000-0000-0000-000000000004"), "Dhaka", "Rangpur")
        };

        private static readonly TimeSpan[] StartTimes = new[]
        {
            new TimeSpan(7, 0, 0),
            new TimeSpan(13, 0, 0),
            new TimeSpan(19, 0, 0)
        };

        public static List<string> GetLocations(string q = "")
        {
            var list = Routes.SelectMany(r => new[] { r.Origin, r.Destination })
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (string.IsNullOrWhiteSpace(q))
            {
                return list;
            }

            q = q.Trim();
            return list.Where(x => x.Contains(q, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public static List<AvailableBusDto> SearchBuses(string from, string to, DateTime journeyDate)
        {
            if (string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(to))
            {
                return new List<AvailableBusDto>();
            }

            var route = Routes.FirstOrDefault(r => string.Equals(r.Origin, from.Trim(), StringComparison.OrdinalIgnoreCase)
                && string.Equals(r.Destination, to.Trim(), StringComparison.OrdinalIgnoreCase));

            if (route.Equals(default))
            {
                return new List<AvailableBusDto>();
            }

            if (journeyDate.Date < new DateTime(2026, 8, 1) || journeyDate.Date > new DateTime(2026, 11, 30))
            {
                return new List<AvailableBusDto>();
            }

            var results = new List<AvailableBusDto>();
            var counter = 1;
            foreach (var time in StartTimes)
            {
                results.Add(new AvailableBusDto
                {
                    BusScheduleId = Guid.NewGuid(),
                    CompanyName = counter % 2 == 0 ? "Green Line" : "National Travels",
                    BusName = counter % 2 == 0 ? $"GL AC Bus {counter:D2}" : $"NT Non-AC Bus {counter:D2}",
                    BusType = counter % 2 == 0 ? "AC" : "Non AC",
                    StartTime = time,
                    ArrivalTime = time.Add(TimeSpan.FromHours(5)),
                    SeatsLeft = 20,
                    Price = counter % 2 == 0 ? 1400m : 900m,
                    CancellationPolicy = "Flexible",
                    LayoutId = Guid.Empty,
                    SeatConfiguration = "A1,A2;B1,B2;C1,C2",
                    BoardingPoints = new List<PointOptionDto>
                    {
                        new PointOptionDto { PointId = Guid.NewGuid(), LocationName = route.Origin, Time = time }
                    },
                    DroppingPoints = new List<PointOptionDto>
                    {
                        new PointOptionDto { PointId = Guid.NewGuid(), LocationName = route.Destination, Time = time.Add(TimeSpan.FromHours(5)) }
                    },
                    SeatLayout = new List<SeatStatusDto>()
                });
                counter++;
            }

            return results;
        }
    }
}
