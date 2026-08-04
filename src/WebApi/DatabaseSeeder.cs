using BusTicketReservationSystem.Domain.Entities;
using BusTicketReservationSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using RouteEntity = BusTicketReservationSystem.Domain.Entities.Route;

namespace BusTicketReservationSystem.WebApi;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BusTicketDbContext>();

        if (await context.BusSchedules.AnyAsync())
        {
            return;
        }

        var layout2x2Id = Guid.Parse("A0000000-0000-0000-0000-000000000001");
        var layout2x1Id = Guid.Parse("A0000000-0000-0000-0000-000000000002");

        var routeDRId = Guid.Parse("10000000-0000-0000-0000-000000000001");
        var routeDDId = Guid.Parse("10000000-0000-0000-0000-000000000002");
        var routeDRaId = Guid.Parse("10000000-0000-0000-0000-000000000003");
        var routeDRbId = Guid.Parse("10000000-0000-0000-0000-000000000004");

        var layouts = new[]
        {
            new BusSeatLayout { BusSeatLayoutId = layout2x2Id, LayoutName = "2x2 Standard", SeatsPerRowCount = 4, TotalSeats = 32, SeatConfiguration = string.Join(";", new[] { "A1,A2,A3,A4", "B1,B2,B3,B4", "C1,C2,C3,C4", "D1,D2,D3,D4", "E1,E2,E3,E4", "F1,F2,F3,F4", "G1,G2,G3,G4", "H1,H2,H3,H4" }) },
            new BusSeatLayout { BusSeatLayoutId = layout2x1Id, LayoutName = "2x1 AC Business", SeatsPerRowCount = 3, TotalSeats = 24, SeatConfiguration = string.Join(";", new[] { "A1,A2,A3", "B1,B2,B3", "C1,C2,C3", "D1,D2,D3", "E1,E2,E3", "F1,F2,F3", "G1,G2,G3", "H1,H2,H3" }) }
        };

        var routes = new RouteEntity[]
        {
            new RouteEntity { RouteId = routeDRId, Origin = "Dhaka", Destination = "Rajshahi" },
            new RouteEntity { RouteId = routeDDId, Origin = "Dhaka", Destination = "Dinajpur" },
            new RouteEntity { RouteId = routeDRaId, Origin = "Dinajpur", Destination = "Rangpur" },
            new RouteEntity { RouteId = routeDRbId, Origin = "Dhaka", Destination = "Rangpur" }
        };

        var points = new List<BoardingPoint>();
        int pointCounter = 1;

        void AddPoint(Guid routeId, string location, TimeSpan time, bool isDrop)
        {
            points.Add(new BoardingPoint
            {
                PointId = Guid.Parse($"40000000-0000-0000-0000-{pointCounter:D12}"),
                RouteId = routeId,
                LocationName = location,
                DepartureTimeOffset = time,
                IsDroppingPoint = isDrop
            });
            pointCounter++;
        }

        AddPoint(routeDRId, "Dhaka: Kallyanpur", new TimeSpan(0, 0, 0), false);
        AddPoint(routeDRId, "Dhaka: Gabtali", new TimeSpan(0, 30, 0), false);
        AddPoint(routeDRId, "Rajshahi: Court", new TimeSpan(4, 30, 0), true);
        AddPoint(routeDRId, "Rajshahi: Bus Terminal", new TimeSpan(5, 0, 0), true);

        AddPoint(routeDDId, "Dhaka: Mirpur-1", new TimeSpan(0, 0, 0), false);
        AddPoint(routeDDId, "Dhaka: Gabtali", new TimeSpan(0, 45, 0), false);
        AddPoint(routeDDId, "Dinajpur: Kantajew", new TimeSpan(7, 0, 0), true);

        AddPoint(routeDRaId, "Dinajpur: Sadar", new TimeSpan(0, 0, 0), false);
        AddPoint(routeDRaId, "Rangpur: Medical Moor", new TimeSpan(2, 0, 0), true);

        AddPoint(routeDRbId, "Dhaka: Sayedabad", new TimeSpan(0, 0, 0), false);
        AddPoint(routeDRbId, "Dhaka: Gabtali", new TimeSpan(0, 45, 0), false);
        AddPoint(routeDRbId, "Rangpur: Central Station", new TimeSpan(7, 30, 0), true);
        AddPoint(routeDRbId, "Rangpur: Stadium", new TimeSpan(8, 0, 0), true);

        var buses = new List<Bus>();
        for (int i = 1; i <= 12; i++)
        {
            bool isAC = i % 2 != 0;
            buses.Add(new Bus
            {
                BusId = Guid.Parse($"30000000-0000-0000-0000-{i:D12}"),
                BusSeatLayoutId = isAC ? layout2x1Id : layout2x2Id,
                CompanyName = isAC ? "Green Line" : "National Travels",
                BusName = isAC ? $"GL AC Bus {i:D2}" : $"NT Non-AC Bus {i:D2}",
                BusType = isAC ? "AC" : "Non AC",
                BasePrice = isAC ? 1400.00m : 900.00m
            });
        }

        var journeyDates = new List<DateTime>();
        for (var date = new DateTime(2026, 8, 1, 0, 0, 0, DateTimeKind.Utc); date <= new DateTime(2026, 11, 30, 0, 0, 0, DateTimeKind.Utc); date = date.AddDays(1))
        {
            journeyDates.Add(date);
        }

        var routeIds = new[] { routeDRId, routeDDId, routeDRaId, routeDRbId };
        var startTimes = new[] { new TimeSpan(7, 0, 0), new TimeSpan(13, 0, 0), new TimeSpan(19, 0, 0) };

        var schedules = new List<BusSchedule>();
        int scheduleCounter = 1;
        int busIndex = 0;

        foreach (var date in journeyDates)
        {
            foreach (var routeId in routeIds)
            {
                for (int i = 0; i < 3; i++)
                {
                    schedules.Add(new BusSchedule
                    {
                        BusScheduleId = Guid.Parse($"30000000-0000-0000-0000-{scheduleCounter + 100:D12}"),
                        RouteId = routeId,
                        BusId = buses[busIndex % buses.Count].BusId,
                        JourneyDate = date,
                        StartTime = startTimes[i]
                    });

                    scheduleCounter++;
                    busIndex++;
                }
            }
        }

        var routeStops = new Dictionary<Guid, (string Boarding, string Dropping)>
        {
            [routeDRId] = ("Dhaka: Kallyanpur", "Rajshahi: Court"),
            [routeDDId] = ("Dhaka: Mirpur-1", "Dinajpur: Kantajew"),
            [routeDRaId] = ("Dinajpur: Sadar", "Rangpur: Medical Moor"),
            [routeDRbId] = ("Dhaka: Sayedabad", "Rangpur: Central Station")
        };

        var passengerNames = new[]
        {
            "Ayesha Khan", "Imran Rahman", "Fatima Sultana", "Rohit Das",
            "Selina Jahan", "Tariq Hasan", "Nadia Akter", "Rahim Uddin"
        };

        var tickets = new List<Ticket>();
        var seatStatuses = new List<SeatStatus>();
        int seatCounter = 1;
        int ticketCounter = 1;

        foreach (var schedule in schedules)
        {
            var bus = buses.First(b => b.BusId == schedule.BusId);
            var layoutRows = (bus.BusSeatLayoutId == layout2x1Id ? layouts[1].SeatConfiguration : layouts[0].SeatConfiguration).Split(';');
            var stops = routeStops[schedule.RouteId];
            var bookingDate = schedule.JourneyDate.AddDays(-3);

            foreach (var row in layoutRows)
            {
                foreach (var seatCode in row.Split(',').Where(x => !string.IsNullOrWhiteSpace(x)))
                {
                    Guid? ticketId = null;
                    string? passengerName = null;
                    string? mobileNumber = null;
                    int status = 1;

                    if (seatCounter % 16 == 0)
                    {
                        ticketId = Guid.Parse($"60000000-0000-0000-0000-{ticketCounter:D12}");
                        passengerName = passengerNames[(ticketCounter - 1) % passengerNames.Length];
                        mobileNumber = $"017{10000000 + ticketCounter:D8}";
                        status = 3;

                        tickets.Add(new Ticket
                        {
                            TicketId = ticketId.Value,
                            BusScheduleId = schedule.BusScheduleId,
                            BookingDate = bookingDate,
                            BoardingPoint = stops.Boarding,
                            DroppingPoint = stops.Dropping,
                            MobileNumber = mobileNumber,
                            TotalPrice = bus.BasePrice
                        });

                        ticketCounter++;
                    }

                    seatStatuses.Add(new SeatStatus
                    {
                        SeatStatusId = Guid.Parse($"50000000-0000-0000-0000-{seatCounter:D12}"),
                        BusScheduleId = schedule.BusScheduleId,
                        SeatNumber = seatCode.Trim(),
                        Status = status,
                        Price = bus.BasePrice,
                        PassengerName = passengerName,
                        MobileNumber = mobileNumber,
                        TicketId = ticketId
                    });

                    seatCounter++;
                }
            }
        }

        await context.BusSeatLayouts.AddRangeAsync(layouts);
        await context.Routes.AddRangeAsync(routes);
        await context.BoardingPoints.AddRangeAsync(points);
        await context.Buses.AddRangeAsync(buses);
        await context.BusSchedules.AddRangeAsync(schedules);
        await context.Tickets.AddRangeAsync(tickets);
        await context.SeatStatuses.AddRangeAsync(seatStatuses);

        await context.SaveChangesAsync();
    }
}
