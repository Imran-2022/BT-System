using Xunit;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using BusTicketReservationSystem.Application.Contracts.Dtos;
using BusTicketReservationSystem.Application.Contracts.Repositories;
using BusTicketReservationSystem.Application.Services;

public class SearchServiceTests
{
    private readonly Mock<IBusScheduleRepository> _mockBusScheduleRepo;
    private readonly SearchService _searchService;

    public SearchServiceTests()
    {
        // Initialize mock repository
        _mockBusScheduleRepo = new Mock<IBusScheduleRepository>();

        // Initialize the service with mocked dependency
        _searchService = new SearchService(_mockBusScheduleRepo.Object);
    }

    [Fact]
    public async Task SearchAvailableBusesAsync_ReturnsSortedBuses_ByStartTime()
    {
        // ARRANGE: Set up input and unsorted mock data
        var from = "Dhaka";
        var to = "Rajshahi";
        var journeyDate = DateTime.Today.AddDays(7);

        var bus1 = new AvailableBusDto { StartTime = new TimeSpan(10, 0, 0) };
        var bus2 = new AvailableBusDto { StartTime = new TimeSpan(6, 0, 0) };
        var bus3 = new AvailableBusDto { StartTime = new TimeSpan(14, 0, 0) };
        var unsortedList = new List<AvailableBusDto> { bus1, bus3, bus2 };

        _mockBusScheduleRepo.Setup(r => r.FindAvailableBusesAsync(from, to, journeyDate))
                            .ReturnsAsync(unsortedList);

        // ACT: Execute the method under test
        var result = await _searchService.SearchAvailableBusesAsync(from, to, journeyDate);

        // ASSERT: Verify the list is not null, has correct count, and is sorted
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Equal(bus2.StartTime, result[0].StartTime);
        Assert.Equal(bus1.StartTime, result[1].StartTime);
        Assert.Equal(bus3.StartTime, result[2].StartTime);

        // Verify the repository call occurred exactly once
        _mockBusScheduleRepo.Verify(r => r.FindAvailableBusesAsync(from, to, journeyDate), Times.Once);
    }

    [Fact]
    public async Task SearchAvailableBusesAsync_ReturnsEmptyList_WhenJourneyDateIsPast()
    {
        // ARRANGE: Use a past date
        var pastDate = DateTime.Today.AddDays(-1);

        // ACT: Execute the method
        var result = await _searchService.SearchAvailableBusesAsync("A", "B", pastDate);

        // ASSERT: Verify empty result and that repository was not called
        Assert.Empty(result);
        _mockBusScheduleRepo.Verify(r => r.FindAvailableBusesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()), Times.Never);
    }
}
