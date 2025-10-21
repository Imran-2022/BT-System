using Microsoft.EntityFrameworkCore;
using BusTicketReservationSystem.Domain.Entities;
using System;

namespace BusTicketReservationSystem.Infrastructure.Data
{
    public class BusTicketDbContext : DbContext
    {
        public BusTicketDbContext(DbContextOptions<BusTicketDbContext> options) : base(options) { }

        public DbSet<BusSchedule> BusSchedules { get; set; }
        public DbSet<Route> Routes { get; set; }
        public DbSet<Bus> Buses { get; set; }
// --- NEW DbSet ---
        public DbSet<SeatStatus> SeatStatuses { get; set; } // <--- ADD THIS LINE
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Define keys
            modelBuilder.Entity<Route>().HasKey(r => r.RouteId);
            modelBuilder.Entity<Bus>().HasKey(b => b.BusId);
            modelBuilder.Entity<BusSchedule>().HasKey(s => s.BusScheduleId);
            
            // --- SEEDING DATA WITH STATIC UTC DATE ---
            
            // 1. Define static GUIDs
            Guid routeDhakaRajshahiId = Guid.Parse("10000000-0000-0000-0000-000000000001");
            Guid busNationalTravelsId = Guid.Parse("20000000-0000-0000-0000-000000000001");
            Guid busHanifEnterpriseId = Guid.Parse("20000000-0000-0000-0000-000000000002");
            Guid scheduleNationalId = Guid.Parse("30000000-0000-0000-0000-000000000001");
            Guid scheduleHanifId = Guid.Parse("30000000-0000-0000-0000-000000000002");

            // 2. Seed Routes
            modelBuilder.Entity<Route>().HasData(
                new Route { RouteId = routeDhakaRajshahiId, Origin = "Dhaka", Destination = "Rajshahi" }
            );

            // 3. Seed Buses
            modelBuilder.Entity<Bus>().HasData(
                new Bus { BusId = busNationalTravelsId, CompanyName = "National Travels", BusName = "99 DHA-CHA", BusType = "Non AC", TotalSeats = 40 },
                new Bus { BusId = busHanifEnterpriseId, CompanyName = "Hanif Enterprise", BusName = "68 RAJ CHAPA", BusType = "Non AC", TotalSeats = 40 }
            );

            // 4. Seed Schedules
            // CRITICAL FIX: Initialize DateTime with DateTimeKind.Utc
            DateTime staticJourneyDate = new DateTime(2025, 10, 23, 0, 0, 0, DateTimeKind.Utc);
            
            modelBuilder.Entity<BusSchedule>().HasData(
                new BusSchedule { BusScheduleId = scheduleNationalId, RouteId = routeDhakaRajshahiId, BusId = busNationalTravelsId, JourneyDate = staticJourneyDate, StartTime = new TimeSpan(6, 0, 0) },
                new BusSchedule { BusScheduleId = scheduleHanifId, RouteId = routeDhakaRajshahiId, BusId = busHanifEnterpriseId, JourneyDate = staticJourneyDate, StartTime = new TimeSpan(7, 30, 0) }
            );
        }
    }
}