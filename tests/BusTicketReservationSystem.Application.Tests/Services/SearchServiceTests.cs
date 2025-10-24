using Xunit;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using BusTicketReservationSystem.Application.Contracts.Dtos;
using BusTicketReservationSystem.Application.Contracts.Repositories;
using BusTicketReservationSystem.Application.Services;

// NOTE: You may need to adjust the namespace based on your actual project structure if the compiler complains.
// Example: namespace BusTicketReservationSystem.Application.Tests 
public class SearchServiceTests
{
    private readonly Mock<IBusScheduleRepository> _mockBusScheduleRepo;
    private readonly SearchService _searchService;

    public SearchServiceTests()
    {
        // ARRANGE SETUP: Initialize the Mock Repository
        _mockBusScheduleRepo = new Mock<IBusScheduleRepository>();
        
        // ARRANGE SETUP: Initialize the actual SearchService, injecting the Mock
        _searchService = new SearchService(_mockBusScheduleRepo.Object);
    }

    [Fact]
    public async Task SearchAvailableBusesAsync_ReturnsSortedBuses_ByStartTime()
    {
        // ARRANGE: Set up data for a successful, sorted search
        var from = "Dhaka";
        var to = "Rajshahi";
        var journeyDate = DateTime.Today.AddDays(7);
        
        // Create unsorted mock data (6:00 AM, 10:00 AM, 2:00 PM)
        var bus1 = new AvailableBusDto { StartTime = new TimeSpan(10, 0, 0) }; 
        var bus2 = new AvailableBusDto { StartTime = new TimeSpan(6, 0, 0) }; 
        var bus3 = new AvailableBusDto { StartTime = new TimeSpan(14, 0, 0) }; 
        var unsortedList = new List<AvailableBusDto> { bus1, bus3, bus2 };

        // Tell the Mock what to return when the Service calls it
        _mockBusScheduleRepo.Setup(r => r.FindAvailableBusesAsync(from, to, journeyDate))
                            .ReturnsAsync(unsortedList);

        // ACT: Execute the method under test
        var result = await _searchService.SearchAvailableBusesAsync(from, to, journeyDate);

        // ASSERT: Verify the outcome
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        
        // Check if the list is sorted correctly (6:00, 10:00, 14:00)
        Assert.Equal(bus2.StartTime, result[0].StartTime);
        Assert.Equal(bus1.StartTime, result[1].StartTime);
        Assert.Equal(bus3.StartTime, result[2].StartTime);
        
        // Verify the mock was called exactly once
        _mockBusScheduleRepo.Verify(r => r.FindAvailableBusesAsync(from, to, journeyDate), Times.Once);
    }

    [Fact]
    public async Task SearchAvailableBusesAsync_ReturnsEmptyList_WhenJourneyDateIsPast()
    {
        // ARRANGE: Use a past date
        var pastDate = DateTime.Today.AddDays(-1);
        
        // ACT: Execute the method
        var result = await _searchService.SearchAvailableBusesAsync("A", "B", pastDate);

        // ASSERT: Verify the service's internal validation worked
        Assert.Empty(result);
        
        // Verify the service stopped the call and did NOT touch the repository
        _mockBusScheduleRepo.Verify(r => r.FindAvailableBusesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()), Times.Never);
    }
}