
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using BusTicketReservationSystem.Application.Contracts.Repositories;
using BusTicketReservationSystem.Application.Contracts.Services;
using BusTicketReservationSystem.Application.Services;
using BusTicketReservationSystem.Infrastructure.Data;
using BusTicketReservationSystem.Infrastructure.Repositories;
using BusTicketReservationSystem.WebApi;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureAppConfiguration((hostingContext, config) =>
{
    config.Sources.Clear();
    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: false);
    config.AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: false);
    config.AddEnvironmentVariables();
    if (args is not null)
    {
        config.AddCommandLine(args);
    }
});

// 1. DATABASE CONTEXT SETUP
builder.Services.AddDbContext<BusTicketDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("BusTicketReservationSystem.Infrastructure")
    )
    .ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning))
);

// 2. DEPENDENCY INJECTION
// Booking
builder.Services.AddScoped<IBookingService, BookingService>();
// AddScoped: Means a new instance of the service/repository is created for every incoming web request, ensuring clean separation and state management.
// What it does: This is the core of the layered architecture. It registers all Services and Repositories so that when a controller or service asks for an interface (e.g., IBookingService), the system knows to provide the concrete implementation (BookingService).
builder.Services.AddScoped<IBookingRepository, BookingRepository>();

// Search
builder.Services.AddScoped<ISearchService, SearchService>();

// Bus Schedule
builder.Services.AddScoped<IBusScheduleRepository, BusScheduleRepository>();

// 3. CONTROLLERS & SWAGGER
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 4. CORS POLICY (Allow Angular Access)
var allowedOrigins = builder.Configuration["Cors:AllowedOrigins"]
    ?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
    ?? Array.Empty<string>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowedOrigins", policy =>
    {
        if (allowedOrigins.Length > 0)
        {
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        }
        else
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        }
    });
});

var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrWhiteSpace(port))
{
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
}

var app = builder.Build();

try
{
    await DatabaseSeeder.SeedAsync(app.Services);
}
catch (Exception ex)
{
    var logger = app.Services.GetService<ILogger<Program>>();
    if (logger != null)
    {
        logger.LogError(ex, "Database seeding failed at startup; continuing without seeding.");
    }
}

// 5. MIDDLEWARE

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

//  CORS Policy
app.UseCors("AllowedOrigins");
app.UseRouting();
app.UseAuthorization();

app.MapControllers();

// app.UseRouting() and app.MapControllers(): Directs incoming URLs (like /api/search) to the correct controller methods.

// Simple test endpoint
app.MapGet("/", () => "BT-System - Api is working Fine !");
app.Run();