using Microsoft.EntityFrameworkCore;
using BusTicketReservationSystem.Domain.Entities;

namespace BusTicketReservationSystem.Infrastructure.Data
{
    public class BusTicketDbContext : DbContext
    {
        public BusTicketDbContext(DbContextOptions<BusTicketDbContext> options) : base(options) { }
        
        // DbSets for all entities
        // the tables in the underlying database.
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
                .HasOne(b => b.Layout) // A Bus has One Layout
                .WithMany(l => l.Buses) // A Layout has Many Buses
                .HasForeignKey(b => b.BusSeatLayoutId); //Bus to BusSeatLayout (One-to-Many): One layout can be used by many buses.

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
            Guid routeDRbId = Guid.Parse("10000000-0000-0000-0000-000000000004"); // Dhaka-Rangpur

            // Helper function to generate sequential GUIDs for buses and schedules
            Func<int, Guid> NextGuid = (counter) => Guid.Parse(string.Format("30000000-0000-0000-0000-{0:D12}", counter));

            // 2. Define journey dates for Aug-Nov 2026
            var journeyDates = new List<DateTime>();
            DateTime startDate = new DateTime(2026, 8, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime endDate = new DateTime(2026, 11, 30, 0, 0, 0, DateTimeKind.Utc);
            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                journeyDates.Add(date);
            }

            // 3. Seed Seat Layouts (updated to larger configurations)
            // 4-column layout: 10 rows (A-J) -> 4 * 10 = 40 seats
            var seatConfig4 = string.Join(";",
                Enumerable.Range(0, 10)
                    .Select(i => string.Join(",", Enumerable.Range(1, 4).Select(n => $"{(char)('A' + i)}{n}"))));

            // 3-column layout: 12 rows (A-L) -> 3 * 12 = 36 seats
            var seatConfig3 = string.Join(";",
                Enumerable.Range(0, 12)
                    .Select(i => string.Join(",", Enumerable.Range(1, 3).Select(n => $"{(char)('A' + i)}{n}"))));

            modelBuilder.Entity<BusSeatLayout>().HasData(
                new BusSeatLayout { BusSeatLayoutId = layout2x2Id, LayoutName = "2x2 Standard", SeatsPerRowCount = 4, TotalSeats = 40, SeatConfiguration = seatConfig4 },
                new BusSeatLayout { BusSeatLayoutId = layout2x1Id, LayoutName = "2x1 AC Business", SeatsPerRowCount = 3, TotalSeats = 36, SeatConfiguration = seatConfig3 }
            );

            // 4. Seed Routes
            modelBuilder.Entity<Route>().HasData(
                new Route { RouteId = routeDRId, Origin = "Dhaka", Destination = "Rajshahi" },
                new Route { RouteId = routeDDId, Origin = "Dhaka", Destination = "Dinajpur" },
                new Route { RouteId = routeDRaId, Origin = "Dinajpur", Destination = "Rangpur" },
                new Route { RouteId = routeDRbId, Origin = "Dhaka", Destination = "Rangpur" }
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

            // Dhaka - Rangpur Points
            AddPoint(routeDRbId, "Dhaka: Sayedabad", new TimeSpan(0, 0, 0), false);
            AddPoint(routeDRbId, "Dhaka: Gabtali", new TimeSpan(0, 45, 0), false);
            AddPoint(routeDRbId, "Rangpur: Central Station", new TimeSpan(7, 30, 0), true);
            AddPoint(routeDRbId, "Rangpur: Stadium", new TimeSpan(8, 0, 0), true);

            modelBuilder.Entity<BoardingPoint>().HasData(points);

            /// 6. Seed Buses
            var allBuses = new List<Bus>();
            var allBusIds = new List<Guid>();

            for (int i = 1; i <= 12; i++) // Create 12 unique buses
            {
                Guid busId = NextGuid(i);
                allBusIds.Add(busId);
                bool isAC = i % 2 != 0; // Odd buses are AC
                Guid layoutId = isAC ? layout2x1Id : layout2x2Id;
                string busType = isAC ? "AC" : "Non AC";
                string companyName = isAC ? "Green Line" : "National Travels";
                string busName = isAC ? $"GL AC Bus {i:D2}" : $"NT Non-AC Bus {i:D2}";
                decimal basePrice = isAC ? 1400.00m : 900.00m;

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

            var allRouteIds = new[] { routeDRId, routeDDId, routeDRaId, routeDRbId };
            var startTimes = new[] { new TimeSpan(7, 0, 0), new TimeSpan(13, 0, 0), new TimeSpan(19, 0, 0) };

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
        }
    }
}