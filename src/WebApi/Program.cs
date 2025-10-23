using BusTicketReservationSystem.Application.Contracts.Repositories;
using BusTicketReservationSystem.Application.Contracts.Services;
using BusTicketReservationSystem.Application.Services;
using BusTicketReservationSystem.Infrastructure.Data;
using BusTicketReservationSystem.Infrastructure.Repositories;
// Remove this using since DummyBusScheduleRepository is going away:
// using BusTicketReservationSystem.Infrastructure.Repositories; 

// --- 1. ADD NEW USINGS FOR POSTGRESQL AND INFRASTRUCTURE ---
using Microsoft.EntityFrameworkCore; 
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// --- 1. Dependency Injection (DI) Setup ---

// 1. ADD DB CONTEXT REGISTRATION
builder.Services.AddDbContext<BusTicketDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
    // CRITICAL: Specify the assembly where EF Core should look for migration files
    b => b.MigrationsAssembly("BusTicketReservationSystem.Infrastructure")));


// 🎯 NEW: Register the Booking Service
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>(); 

// Register Application Service (Search Service remains)
builder.Services.AddScoped<ISearchService, SearchService>();

// Register the REAL Repository using EF Core/PostgreSQL
builder.Services.AddScoped<IBusScheduleRepository, BusScheduleRepository>();
// Register Application Service

builder.Services.AddScoped<ISearchService, SearchService>();

// CRITICAL CHANGE: SWAP REPOSITORIES
// Register Infrastructure Service (TEMPORARY DUMMY IMPLEMENTATION)
// builder.Services.AddScoped<IBusScheduleRepository, DummyBusScheduleRepository>(); // <-- DELETE OR COMMENT OUT

// Register the REAL Repository using EF Core/PostgreSQL
builder.Services.AddScoped<IBusScheduleRepository, BusScheduleRepository>(); // <-- NEW LINE

// Add Controllers service
builder.Services.AddControllers();
// ... (rest of the services setup)....................

// Add Swagger documentation services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- 2. CORS Policy Setup (Allow Angular Access) ---
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        // For development, allow all origins, methods, and headers.
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});


var app = builder.Build();

// --- 3. HTTP Request Pipeline (Middleware) ---

// app.UseHttpsRedirection(); // Keep this commented or remove it if not using HTTPS

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

app.MapGet("/", () => "Api is working Fine !");

// 4. Use CORS Policy
app.UseCors(); 

app.UseRouting();
app.UseAuthorization();

// 5. Map API Controllers
app.MapControllers(); 

// Remove app.UseStaticFiles(), app.UseSpaStaticFiles(), and the entire app.UseSpa() block.

app.Run();





// using BusTicketReservationSystem.Application.Contracts.Repositories;
// using BusTicketReservationSystem.Application.Contracts.Services;
// using BusTicketReservationSystem.Application.Services;
// using BusTicketReservationSystem.Infrastructure.Repositories; 
// using Microsoft.AspNetCore.Mvc;

// var builder = WebApplication.CreateBuilder(args);

// // --- 1. Dependency Injection (DI) Setup ---

// // Register Application Service
// builder.Services.AddScoped<ISearchService, SearchService>();

// // Register Infrastructure Service (TEMPORARY DUMMY IMPLEMENTATION)
// builder.Services.AddScoped<IBusScheduleRepository, DummyBusScheduleRepository>();

// // Add Controllers service
// builder.Services.AddControllers();

// // Add Swagger documentation services
// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();

// // --- 2. CORS Policy Setup (Allow Angular Access) ---
// builder.Services.AddCors(options =>
// {
//     options.AddDefaultPolicy(policy =>
//     {
//         // For development, allow all origins, methods, and headers.
//         policy.AllowAnyOrigin()
//               .AllowAnyMethod()
//               .AllowAnyHeader();
//     });
// });


// var app = builder.Build();

// // --- 3. HTTP Request Pipeline (Middleware) ---

// // app.UseHttpsRedirection(); // Keep this commented or remove it if not using HTTPS

// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }
// else
// {
//     app.UseExceptionHandler("/Error");
//     app.UseHsts();
// }

// app.MapGet("/", () => "Api is working Fine !");

// // 4. Use CORS Policy
// app.UseCors(); 

// app.UseRouting();
// app.UseAuthorization();

// // 5. Map API Controllers
// app.MapControllers(); 

// // Remove app.UseStaticFiles(), app.UseSpaStaticFiles(), and the entire app.UseSpa() block.

// app.Run();