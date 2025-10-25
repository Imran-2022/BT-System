using Microsoft.EntityFrameworkCore;
using BusTicketReservationSystem.Domain.Entities;

namespace BusTicketReservationSystem.Infrastructure.Data
{
    public class BusTicketDbContext : DbContext
    {
        public BusTicketDbContext(DbContextOptions<BusTicketDbContext> options) : base(options) { }

        // DbSets for all entities
        public DbSet<BusSchedule> BusSchedules { get; set; }
        public DbSet<Route> Routes { get; set; }
        public DbSet<Bus> Buses { get; set; }
        public DbSet<SeatStatus> SeatStatuses { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<BusSeatLayout> BusSeatLayouts { get; set; }
        public DbSet<BoardingPoint> BoardingPoints { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Keys and Relationships
            modelBuilder.Entity<Route>().HasKey(r => r.RouteId);
            modelBuilder.Entity<Bus>().HasKey(b => b.BusId);
            modelBuilder.Entity<BusSchedule>().HasKey(s => s.BusScheduleId);
            modelBuilder.Entity<BusSeatLayout>().HasKey(l => l.BusSeatLayoutId);
            modelBuilder.Entity<BoardingPoint>().HasKey(p => p.PointId);
            modelBuilder.Entity<Ticket>().HasKey(t => t.TicketId);

            modelBuilder.Entity<Bus>()
                .HasOne(b => b.Layout)
                .WithMany(l => l.Buses)
                .HasForeignKey(b => b.BusSeatLayoutId);

            modelBuilder.Entity<BoardingPoint>()
                .HasOne(p => p.Route)
                .WithMany(r => r.BoardingPoints)
                .HasForeignKey(p => p.RouteId);

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.BusSchedule)
                .WithMany(s => s.Tickets)
                .HasForeignKey(t => t.BusScheduleId)
                .IsRequired();

            modelBuilder.Entity<SeatStatus>()
                .HasOne<Ticket>()
                .WithMany(t => t.BookedSeats)
                .HasForeignKey(s => s.TicketId)
                .IsRequired(false);

             // SEEDING DATA

            // 1. Predefined GUIDs for layouts and routes
            Guid layout2x2Id = Guid.Parse("A0000000-0000-0000-0000-000000000001");
            Guid layout2x1Id = Guid.Parse("A0000000-0000-0000-0000-000000000002");

            Guid routeDRId = Guid.Parse("10000000-0000-0000-0000-000000000001"); // Dhaka-Rajshahi
            Guid routeDDId = Guid.Parse("10000000-0000-0000-0000-000000000002"); // Dhaka-Dinajpur
            Guid routeDRaId = Guid.Parse("10000000-0000-0000-0000-000000000003"); // Dinajpur-Rangpur

            // Helper function to generate sequential GUIDs for buses and schedules
            Func<int, Guid> NextGuid = (counter) => Guid.Parse(string.Format("30000000-0000-0000-0000-{0:D12}", counter));

            // 2. Define journey dates
            var journeyDates = new List<DateTime>();
            for (int i = 0; i < 5; i++)
            {
                journeyDates.Add(new DateTime(2025, 10, 29, 0, 0, 0, DateTimeKind.Utc).AddDays(i));
            }

            // 3. Seed Seat Layouts
            string standardLayout = string.Join(";", new[] { "A1,A2,A3,A4", "B1,B2,B3,B4", "C1,C2,C3,C4", "D1,D2,D3,D4", "E1,E2,E3,E4", "F1,F2,F3,F4", "G1,G2,G3,G4", "H1,H2,H3,H4" });
            string acLayout = string.Join(";", new[] { "A1,A2,A3", "B1,B2,B3", "C1,C2,C3", "D1,D2,D3", "E1,E2,E3", "F1,F2,F3", "G1,G2,G3", "H1,H2,H3" });

            modelBuilder.Entity<BusSeatLayout>().HasData(
                new BusSeatLayout { BusSeatLayoutId = layout2x2Id, LayoutName = "2x2 Standard", SeatsPerRowCount = 4, TotalSeats = 32, SeatConfiguration = standardLayout },
                new BusSeatLayout { BusSeatLayoutId = layout2x1Id, LayoutName = "2x1 AC Business", SeatsPerRowCount = 3, TotalSeats = 24, SeatConfiguration = acLayout }
            );

            // 4. Seed Routes
            modelBuilder.Entity<Route>().HasData(
                new Route { RouteId = routeDRId, Origin = "Dhaka", Destination = "Rajshahi" },
                new Route { RouteId = routeDDId, Origin = "Dhaka", Destination = "Dinajpur" },
                new Route { RouteId = routeDRaId, Origin = "Dinajpur", Destination = "Rangpur" }
            );

            // 5. Seed Boarding/Dropping Points
            var points = new List<BoardingPoint>();
            int pointCounter = 1;

            Action<Guid, string, TimeSpan, bool> AddPoint = (routeId, location, time, isDrop) =>
            {
                points.Add(new BoardingPoint { PointId = Guid.Parse(string.Format("40000000-0000-0000-0000-{0:D12}", pointCounter++)), RouteId = routeId, LocationName = location, DepartureTimeOffset = time, IsDroppingPoint = isDrop });
            };

            // Dhaka - Rajshahi Points
            AddPoint(routeDRId, "Dhaka: Kallyanpur", new TimeSpan(0, 0, 0), false);
            AddPoint(routeDRId, "Dhaka: Gabtali", new TimeSpan(0, 30, 0), false);
            AddPoint(routeDRId, "Rajshahi: Court", new TimeSpan(4, 30, 0), true);
            AddPoint(routeDRId, "Rajshahi: Bus Terminal", new TimeSpan(5, 0, 0), true);

            // Dhaka - Dinajpur Points
            AddPoint(routeDDId, "Dhaka: Mirpur-1", new TimeSpan(0, 0, 0), false);
            AddPoint(routeDDId, "Dhaka: Gabtali", new TimeSpan(0, 45, 0), false);
            AddPoint(routeDDId, "Dinajpur: Kantajew", new TimeSpan(7, 0, 0), true);

            // Dinajpur - Rangpur Points
            AddPoint(routeDRaId, "Dinajpur: Sadar", new TimeSpan(0, 0, 0), false);
            AddPoint(routeDRaId, "Rangpur: Medical Moor", new TimeSpan(2, 0, 0), true);

            modelBuilder.Entity<BoardingPoint>().HasData(points);

            /// 6. Seed Buses
            var allBuses = new List<Bus>();
            var allBusIds = new List<Guid>();

            for (int i = 1; i <= 10; i++) // Create 10 unique buses
            {
                Guid busId = NextGuid(i);
                allBusIds.Add(busId);
                bool isAC = i % 2 != 0; // Bus 1, 3, 5, 7, 9 are AC (using Green Line's 2x1 layout)
                Guid layoutId = isAC ? layout2x1Id : layout2x2Id;
                string busType = isAC ? "AC" : "Non AC";
                string companyName = isAC ? "Green Line" : "National Travels";
                string busName = isAC ? $"GL AC Bus {i:D2}" : $"Bus {i:D2} Non-AC";
                decimal basePrice = isAC ? 1200.00m : 700.00m;

                allBuses.Add(new Bus
                {
                    BusId = busId,
                    BusSeatLayoutId = layoutId,
                    CompanyName = companyName,
                    BusName = busName,
                    BusType = busType,
                    BasePrice = basePrice 
                });
            }

            modelBuilder.Entity<Bus>().HasData(allBuses);

            // 7. Seed Schedules 
            var schedules = new List<BusSchedule>();
            int scheduleCounter = 1;
            int busIndex = 0; // Index to cycle through the 10 unique buses

            var allRouteIds = new[] { routeDRId, routeDDId, routeDRaId };
            var startTimes = new[] { new TimeSpan(6, 0, 0), new TimeSpan(12, 0, 0), new TimeSpan(18, 0, 0) };

            foreach (var date in journeyDates)
            {
                foreach (var routeId in allRouteIds)
                {
                    for (int i = 0; i < 3; i++) // 3 schedules per route per day
                    {
                        Guid currentBusId = allBusIds[busIndex % allBusIds.Count];
                        busIndex++; // Move to the next unique bus

                        schedules.Add(new BusSchedule
                        {
                            BusScheduleId = NextGuid(scheduleCounter++ + 100), 
                            RouteId = routeId,
                            BusId = currentBusId, 
                            JourneyDate = date,
                            StartTime = startTimes[i]
                        });
                    }
                }
            }

            modelBuilder.Entity<BusSchedule>().HasData(schedules);
            
            // 8. Seed Seat Statuses
            var seatStatuses = new List<SeatStatus>();
            int seatStatusCounter = 1;

            foreach (var schedule in schedules)
            {
                var bus = allBuses.First(b => b.BusId == schedule.BusId);
                decimal basePrice = bus.BasePrice;
                var layout = (bus.BusSeatLayoutId == layout2x1Id ? acLayout : standardLayout).Split(';');

                foreach (var row in layout)
                {
                    foreach (var seatNumber in row.Split(',').Where(s => !string.IsNullOrWhiteSpace(s)))
                    {
                        seatStatuses.Add(new SeatStatus
                        {
                            // Ensure unique ID generation is maintained
                            SeatStatusId = Guid.Parse(string.Format("50000000-0000-0000-0000-{0:D12}", seatStatusCounter++)),
                            BusScheduleId = schedule.BusScheduleId,
                            SeatNumber = seatNumber.Trim(),
                            Status = 1, // Available
                            Price = basePrice, // correct price from the Bus
                            TicketId = null
                        });
                    }
                }
            }

            modelBuilder.Entity<SeatStatus>().HasData(seatStatuses);
        }
    }
}