
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

using BusTicketReservationSystem.Application.Contracts.Repositories;
using BusTicketReservationSystem.Application.Contracts.Services;
using BusTicketReservationSystem.Application.Services;
using BusTicketReservationSystem.Infrastructure.Data;
using BusTicketReservationSystem.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// 1. DATABASE CONTEXT SETUP
builder.Services.AddDbContext<BusTicketDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("BusTicketReservationSystem.Infrastructure")
    )
);

// 2. DEPENDENCY INJECTION
// Booking
builder.Services.AddScoped<IBookingService, BookingService>();
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
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        // For development, allowed all origins, methods, and headers.
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

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
app.UseCors();
app.UseRouting();
app.UseAuthorization();

app.MapControllers();

// Simple test endpoint
app.MapGet("/", () => "BT-System - Api is working Fine !");
app.Run();