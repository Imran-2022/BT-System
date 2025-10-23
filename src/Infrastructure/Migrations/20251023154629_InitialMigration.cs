using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BusTicketReservationSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BusSeatLayouts",
                columns: table => new
                {
                    BusSeatLayoutId = table.Column<Guid>(type: "uuid", nullable: false),
                    LayoutName = table.Column<string>(type: "text", nullable: false),
                    SeatsPerRowCount = table.Column<int>(type: "integer", nullable: false),
                    SeatConfiguration = table.Column<string>(type: "text", nullable: false),
                    TotalSeats = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusSeatLayouts", x => x.BusSeatLayoutId);
                });

            migrationBuilder.CreateTable(
                name: "Routes",
                columns: table => new
                {
                    RouteId = table.Column<Guid>(type: "uuid", nullable: false),
                    Origin = table.Column<string>(type: "text", nullable: false),
                    Destination = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Routes", x => x.RouteId);
                });

            migrationBuilder.CreateTable(
                name: "Buses",
                columns: table => new
                {
                    BusId = table.Column<Guid>(type: "uuid", nullable: false),
                    BusSeatLayoutId = table.Column<Guid>(type: "uuid", nullable: false),
                    CompanyName = table.Column<string>(type: "text", nullable: false),
                    BusName = table.Column<string>(type: "text", nullable: false),
                    BusType = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Buses", x => x.BusId);
                    table.ForeignKey(
                        name: "FK_Buses_BusSeatLayouts_BusSeatLayoutId",
                        column: x => x.BusSeatLayoutId,
                        principalTable: "BusSeatLayouts",
                        principalColumn: "BusSeatLayoutId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BoardingPoints",
                columns: table => new
                {
                    PointId = table.Column<Guid>(type: "uuid", nullable: false),
                    RouteId = table.Column<Guid>(type: "uuid", nullable: false),
                    LocationName = table.Column<string>(type: "text", nullable: false),
                    DepartureTimeOffset = table.Column<TimeSpan>(type: "interval", nullable: false),
                    IsDroppingPoint = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoardingPoints", x => x.PointId);
                    table.ForeignKey(
                        name: "FK_BoardingPoints_Routes_RouteId",
                        column: x => x.RouteId,
                        principalTable: "Routes",
                        principalColumn: "RouteId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BusSchedules",
                columns: table => new
                {
                    BusScheduleId = table.Column<Guid>(type: "uuid", nullable: false),
                    RouteId = table.Column<Guid>(type: "uuid", nullable: false),
                    BusId = table.Column<Guid>(type: "uuid", nullable: false),
                    JourneyDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "interval", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusSchedules", x => x.BusScheduleId);
                    table.ForeignKey(
                        name: "FK_BusSchedules_Buses_BusId",
                        column: x => x.BusId,
                        principalTable: "Buses",
                        principalColumn: "BusId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BusSchedules_Routes_RouteId",
                        column: x => x.RouteId,
                        principalTable: "Routes",
                        principalColumn: "RouteId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tickets",
                columns: table => new
                {
                    TicketId = table.Column<Guid>(type: "uuid", nullable: false),
                    BusScheduleId = table.Column<Guid>(type: "uuid", nullable: false),
                    BookingDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    BoardingPoint = table.Column<string>(type: "text", nullable: false),
                    DroppingPoint = table.Column<string>(type: "text", nullable: false),
                    MobileNumber = table.Column<string>(type: "text", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.TicketId);
                    table.ForeignKey(
                        name: "FK_Tickets_BusSchedules_BusScheduleId",
                        column: x => x.BusScheduleId,
                        principalTable: "BusSchedules",
                        principalColumn: "BusScheduleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SeatStatuses",
                columns: table => new
                {
                    SeatStatusId = table.Column<Guid>(type: "uuid", nullable: false),
                    BusScheduleId = table.Column<Guid>(type: "uuid", nullable: false),
                    SeatNumber = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    PassengerName = table.Column<string>(type: "text", nullable: true),
                    MobileNumber = table.Column<string>(type: "text", nullable: true),
                    TicketId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeatStatuses", x => x.SeatStatusId);
                    table.ForeignKey(
                        name: "FK_SeatStatuses_BusSchedules_BusScheduleId",
                        column: x => x.BusScheduleId,
                        principalTable: "BusSchedules",
                        principalColumn: "BusScheduleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SeatStatuses_Tickets_TicketId",
                        column: x => x.TicketId,
                        principalTable: "Tickets",
                        principalColumn: "TicketId");
                });

            migrationBuilder.InsertData(
                table: "BusSeatLayouts",
                columns: new[] { "BusSeatLayoutId", "LayoutName", "SeatConfiguration", "SeatsPerRowCount", "TotalSeats" },
                values: new object[,]
                {
                    { new Guid("a0000000-0000-0000-0000-000000000001"), "2x2 Standard", "A1,A2,A3,A4;B1,B2,B3,B4;C1,C2,C3,C4;D1,D2,D3,D4;E1,E2,E3,E4;F1,F2,F3,F4;G1,G2,G3,G4;H1,H2,H3,H4", 4, 32 },
                    { new Guid("a0000000-0000-0000-0000-000000000002"), "2x1 AC Business", "A1,A2,A3;B1,B2,B3;C1,C2,C3;D1,D2,D3;E1,E2,E3;F1,F2,F3;G1,G2,G3;H1,H2,H3", 3, 24 }
                });

            migrationBuilder.InsertData(
                table: "Routes",
                columns: new[] { "RouteId", "Destination", "Origin" },
                values: new object[,]
                {
                    { new Guid("10000000-0000-0000-0000-000000000001"), "Rajshahi", "Dhaka" },
                    { new Guid("10000000-0000-0000-0000-000000000002"), "Dinajpur", "Dhaka" },
                    { new Guid("10000000-0000-0000-0000-000000000003"), "Rangpur", "Dinajpur" }
                });

            migrationBuilder.InsertData(
                table: "BoardingPoints",
                columns: new[] { "PointId", "DepartureTimeOffset", "IsDroppingPoint", "LocationName", "RouteId" },
                values: new object[,]
                {
                    { new Guid("40000000-0000-0000-0000-000000000001"), new TimeSpan(0, 0, 0, 0, 0), false, "Dhaka: Kallyanpur", new Guid("10000000-0000-0000-0000-000000000001") },
                    { new Guid("40000000-0000-0000-0000-000000000002"), new TimeSpan(0, 0, 30, 0, 0), false, "Dhaka: Gabtali", new Guid("10000000-0000-0000-0000-000000000001") },
                    { new Guid("40000000-0000-0000-0000-000000000003"), new TimeSpan(0, 4, 30, 0, 0), true, "Rajshahi: Court", new Guid("10000000-0000-0000-0000-000000000001") },
                    { new Guid("40000000-0000-0000-0000-000000000004"), new TimeSpan(0, 5, 0, 0, 0), true, "Rajshahi: Bus Terminal", new Guid("10000000-0000-0000-0000-000000000001") },
                    { new Guid("40000000-0000-0000-0000-000000000005"), new TimeSpan(0, 0, 0, 0, 0), false, "Dhaka: Mirpur-1", new Guid("10000000-0000-0000-0000-000000000002") },
                    { new Guid("40000000-0000-0000-0000-000000000006"), new TimeSpan(0, 0, 45, 0, 0), false, "Dhaka: Gabtali", new Guid("10000000-0000-0000-0000-000000000002") },
                    { new Guid("40000000-0000-0000-0000-000000000007"), new TimeSpan(0, 7, 0, 0, 0), true, "Dinajpur: Kantajew", new Guid("10000000-0000-0000-0000-000000000002") },
                    { new Guid("40000000-0000-0000-0000-000000000008"), new TimeSpan(0, 0, 0, 0, 0), false, "Dinajpur: Sadar", new Guid("10000000-0000-0000-0000-000000000003") },
                    { new Guid("40000000-0000-0000-0000-000000000009"), new TimeSpan(0, 2, 0, 0, 0), true, "Rangpur: Medical Moor", new Guid("10000000-0000-0000-0000-000000000003") }
                });

            migrationBuilder.InsertData(
                table: "Buses",
                columns: new[] { "BusId", "BusName", "BusSeatLayoutId", "BusType", "CompanyName" },
                values: new object[,]
                {
                    { new Guid("20000000-0000-0000-0000-000000000001"), "99 Non-AC", new Guid("a0000000-0000-0000-0000-000000000001"), "Non AC", "National Travels" },
                    { new Guid("20000000-0000-0000-0000-000000000002"), "68 Business", new Guid("a0000000-0000-0000-0000-000000000001"), "Non AC", "Hanif Enterprise" },
                    { new Guid("20000000-0000-0000-0000-000000000003"), "GL AC", new Guid("a0000000-0000-0000-0000-000000000002"), "AC", "Green Line" }
                });

            migrationBuilder.InsertData(
                table: "BusSchedules",
                columns: new[] { "BusScheduleId", "BusId", "JourneyDate", "RouteId", "StartTime" },
                values: new object[,]
                {
                    { new Guid("40000000-0000-0000-0000-000000000001"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 29, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("10000000-0000-0000-0000-000000000001"), new TimeSpan(0, 6, 0, 0, 0) },
                    { new Guid("40000000-0000-0000-0000-000000000002"), new Guid("20000000-0000-0000-0000-000000000002"), new DateTime(2025, 10, 29, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("10000000-0000-0000-0000-000000000001"), new TimeSpan(0, 12, 0, 0, 0) },
                    { new Guid("40000000-0000-0000-0000-000000000003"), new Guid("20000000-0000-0000-0000-000000000003"), new DateTime(2025, 10, 29, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("10000000-0000-0000-0000-000000000001"), new TimeSpan(0, 18, 0, 0, 0) },
                    { new Guid("40000000-0000-0000-0000-000000000004"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 29, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("10000000-0000-0000-0000-000000000002"), new TimeSpan(0, 6, 0, 0, 0) },
                    { new Guid("40000000-0000-0000-0000-000000000005"), new Guid("20000000-0000-0000-0000-000000000002"), new DateTime(2025, 10, 29, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("10000000-0000-0000-0000-000000000002"), new TimeSpan(0, 12, 0, 0, 0) },
                    { new Guid("40000000-0000-0000-0000-000000000006"), new Guid("20000000-0000-0000-0000-000000000003"), new DateTime(2025, 10, 29, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("10000000-0000-0000-0000-000000000002"), new TimeSpan(0, 18, 0, 0, 0) },
                    { new Guid("40000000-0000-0000-0000-000000000007"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 29, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("10000000-0000-0000-0000-000000000003"), new TimeSpan(0, 6, 0, 0, 0) },
                    { new Guid("40000000-0000-0000-0000-000000000008"), new Guid("20000000-0000-0000-0000-000000000002"), new DateTime(2025, 10, 29, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("10000000-0000-0000-0000-000000000003"), new TimeSpan(0, 12, 0, 0, 0) },
                    { new Guid("40000000-0000-0000-0000-000000000009"), new Guid("20000000-0000-0000-0000-000000000003"), new DateTime(2025, 10, 29, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("10000000-0000-0000-0000-000000000003"), new TimeSpan(0, 18, 0, 0, 0) },
                    { new Guid("40000000-0000-0000-0000-000000000010"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 30, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("10000000-0000-0000-0000-000000000001"), new TimeSpan(0, 6, 0, 0, 0) },
                    { new Guid("40000000-0000-0000-0000-000000000011"), new Guid("20000000-0000-0000-0000-000000000002"), new DateTime(2025, 10, 30, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("10000000-0000-0000-0000-000000000001"), new TimeSpan(0, 12, 0, 0, 0) },
                    { new Guid("40000000-0000-0000-0000-000000000012"), new Guid("20000000-0000-0000-0000-000000000003"), new DateTime(2025, 10, 30, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("10000000-0000-0000-0000-000000000001"), new TimeSpan(0, 18, 0, 0, 0) },
                    { new Guid("40000000-0000-0000-0000-000000000013"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 30, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("10000000-0000-0000-0000-000000000002"), new TimeSpan(0, 6, 0, 0, 0) },
                    { new Guid("40000000-0000-0000-0000-000000000014"), new Guid("20000000-0000-0000-0000-000000000002"), new DateTime(2025, 10, 30, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("10000000-0000-0000-0000-000000000002"), new TimeSpan(0, 12, 0, 0, 0) },
                    { new Guid("40000000-0000-0000-0000-000000000015"), new Guid("20000000-0000-0000-0000-000000000003"), new DateTime(2025, 10, 30, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("10000000-0000-0000-0000-000000000002"), new TimeSpan(0, 18, 0, 0, 0) },
                    { new Guid("40000000-0000-0000-0000-000000000016"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 30, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("10000000-0000-0000-0000-000000000003"), new TimeSpan(0, 6, 0, 0, 0) },
                    { new Guid("40000000-0000-0000-0000-000000000017"), new Guid("20000000-0000-0000-0000-000000000002"), new DateTime(2025, 10, 30, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("10000000-0000-0000-0000-000000000003"), new TimeSpan(0, 12, 0, 0, 0) },
                    { new Guid("40000000-0000-0000-0000-000000000018"), new Guid("20000000-0000-0000-0000-000000000003"), new DateTime(2025, 10, 30, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("10000000-0000-0000-0000-000000000003"), new TimeSpan(0, 18, 0, 0, 0) },
                    { new Guid("40000000-0000-0000-0000-000000000019"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 31, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("10000000-0000-0000-0000-000000000001"), new TimeSpan(0, 6, 0, 0, 0) },
                    { new Guid("40000000-0000-0000-0000-000000000020"), new Guid("20000000-0000-0000-0000-000000000002"), new DateTime(2025, 10, 31, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("10000000-0000-0000-0000-000000000001"), new TimeSpan(0, 12, 0, 0, 0) },
                    { new Guid("40000000-0000-0000-0000-000000000021"), new Guid("20000000-0000-0000-0000-000000000003"), new DateTime(2025, 10, 31, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("10000000-0000-0000-0000-000000000001"), new TimeSpan(0, 18, 0, 0, 0) },
                    { new Guid("40000000-0000-0000-0000-000000000022"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 31, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("10000000-0000-0000-0000-000000000002"), new TimeSpan(0, 6, 0, 0, 0) },
                    { new Guid("40000000-0000-0000-0000-000000000023"), new Guid("20000000-0000-0000-0000-000000000002"), new DateTime(2025, 10, 31, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("10000000-0000-0000-0000-000000000002"), new TimeSpan(0, 12, 0, 0, 0) },
                    { new Guid("40000000-0000-0000-0000-000000000024"), new Guid("20000000-0000-0000-0000-000000000003"), new DateTime(2025, 10, 31, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("10000000-0000-0000-0000-000000000002"), new TimeSpan(0, 18, 0, 0, 0) },
                    { new Guid("40000000-0000-0000-0000-000000000025"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 31, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("10000000-0000-0000-0000-000000000003"), new TimeSpan(0, 6, 0, 0, 0) },
                    { new Guid("40000000-0000-0000-0000-000000000026"), new Guid("20000000-0000-0000-0000-000000000002"), new DateTime(2025, 10, 31, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("10000000-0000-0000-0000-000000000003"), new TimeSpan(0, 12, 0, 0, 0) },
                    { new Guid("40000000-0000-0000-0000-000000000027"), new Guid("20000000-0000-0000-0000-000000000003"), new DateTime(2025, 10, 31, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("10000000-0000-0000-0000-000000000003"), new TimeSpan(0, 18, 0, 0, 0) },
                    { new Guid("40000000-0000-0000-0000-000000000028"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTime(2025, 11, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("10000000-0000-0000-0000-000000000001"), new TimeSpan(0, 6, 0, 0, 0) },
                    { new Guid("40000000-0000-0000-0000-000000000029"), new Guid("20000000-0000-0000-0000-000000000002"), new DateTime(2025, 11, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("10000000-0000-0000-0000-000000000001"), new TimeSpan(0, 12, 0, 0, 0) },
                    { new Guid("40000000-0000-0000-0000-000000000030"), new Guid("20000000-0000-0000-0000-000000000003"), new DateTime(2025, 11, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("10000000-0000-0000-0000-000000000001"), new TimeSpan(0, 18, 0, 0, 0) },
                    { new Guid("40000000-0000-0000-0000-000000000031"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTime(2025, 11, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("10000000-0000-0000-0000-000000000002"), new TimeSpan(0, 6, 0, 0, 0) },
                    { new Guid("40000000-0000-0000-0000-000000000032"), new Guid("20000000-0000-0000-0000-000000000002"), new DateTime(2025, 11, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("10000000-0000-0000-0000-000000000002"), new TimeSpan(0, 12, 0, 0, 0) },
                    { new Guid("40000000-0000-0000-0000-000000000033"), new Guid("20000000-0000-0000-0000-000000000003"), new DateTime(2025, 11, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("10000000-0000-0000-0000-000000000002"), new TimeSpan(0, 18, 0, 0, 0) },
                    { new Guid("40000000-0000-0000-0000-000000000034"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTime(2025, 11, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("10000000-0000-0000-0000-000000000003"), new TimeSpan(0, 6, 0, 0, 0) },
                    { new Guid("40000000-0000-0000-0000-000000000035"), new Guid("20000000-0000-0000-0000-000000000002"), new DateTime(2025, 11, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("10000000-0000-0000-0000-000000000003"), new TimeSpan(0, 12, 0, 0, 0) },
                    { new Guid("40000000-0000-0000-0000-000000000036"), new Guid("20000000-0000-0000-0000-000000000003"), new DateTime(2025, 11, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("10000000-0000-0000-0000-000000000003"), new TimeSpan(0, 18, 0, 0, 0) },
                    { new Guid("40000000-0000-0000-0000-000000000037"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTime(2025, 11, 2, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("10000000-0000-0000-0000-000000000001"), new TimeSpan(0, 6, 0, 0, 0) },
                    { new Guid("40000000-0000-0000-0000-000000000038"), new Guid("20000000-0000-0000-0000-000000000002"), new DateTime(2025, 11, 2, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("10000000-0000-0000-0000-000000000001"), new TimeSpan(0, 12, 0, 0, 0) },
                    { new Guid("40000000-0000-0000-0000-000000000039"), new Guid("20000000-0000-0000-0000-000000000003"), new DateTime(2025, 11, 2, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("10000000-0000-0000-0000-000000000001"), new TimeSpan(0, 18, 0, 0, 0) },
                    { new Guid("40000000-0000-0000-0000-000000000040"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTime(2025, 11, 2, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("10000000-0000-0000-0000-000000000002"), new TimeSpan(0, 6, 0, 0, 0) },
                    { new Guid("40000000-0000-0000-0000-000000000041"), new Guid("20000000-0000-0000-0000-000000000002"), new DateTime(2025, 11, 2, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("10000000-0000-0000-0000-000000000002"), new TimeSpan(0, 12, 0, 0, 0) },
                    { new Guid("40000000-0000-0000-0000-000000000042"), new Guid("20000000-0000-0000-0000-000000000003"), new DateTime(2025, 11, 2, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("10000000-0000-0000-0000-000000000002"), new TimeSpan(0, 18, 0, 0, 0) },
                    { new Guid("40000000-0000-0000-0000-000000000043"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTime(2025, 11, 2, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("10000000-0000-0000-0000-000000000003"), new TimeSpan(0, 6, 0, 0, 0) },
                    { new Guid("40000000-0000-0000-0000-000000000044"), new Guid("20000000-0000-0000-0000-000000000002"), new DateTime(2025, 11, 2, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("10000000-0000-0000-0000-000000000003"), new TimeSpan(0, 12, 0, 0, 0) },
                    { new Guid("40000000-0000-0000-0000-000000000045"), new Guid("20000000-0000-0000-0000-000000000003"), new DateTime(2025, 11, 2, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("10000000-0000-0000-0000-000000000003"), new TimeSpan(0, 18, 0, 0, 0) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_BoardingPoints_RouteId",
                table: "BoardingPoints",
                column: "RouteId");

            migrationBuilder.CreateIndex(
                name: "IX_Buses_BusSeatLayoutId",
                table: "Buses",
                column: "BusSeatLayoutId");

            migrationBuilder.CreateIndex(
                name: "IX_BusSchedules_BusId",
                table: "BusSchedules",
                column: "BusId");

            migrationBuilder.CreateIndex(
                name: "IX_BusSchedules_RouteId",
                table: "BusSchedules",
                column: "RouteId");

            migrationBuilder.CreateIndex(
                name: "IX_SeatStatuses_BusScheduleId",
                table: "SeatStatuses",
                column: "BusScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_SeatStatuses_TicketId",
                table: "SeatStatuses",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_BusScheduleId",
                table: "Tickets",
                column: "BusScheduleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BoardingPoints");

            migrationBuilder.DropTable(
                name: "SeatStatuses");

            migrationBuilder.DropTable(
                name: "Tickets");

            migrationBuilder.DropTable(
                name: "BusSchedules");

            migrationBuilder.DropTable(
                name: "Buses");

            migrationBuilder.DropTable(
                name: "Routes");

            migrationBuilder.DropTable(
                name: "BusSeatLayouts");
        }
    }
}
