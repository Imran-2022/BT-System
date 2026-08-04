using BusTicketReservationSystem.Application.Contracts.Dtos;

namespace BusTicketReservationSystem.Application.Contracts.Services
{
    public interface IBookingService
    {
        // Retrieves the seat plan for a specific bus schedule.
        Task<AvailableBusDto?> GetSeatPlanAsync(Guid busScheduleId);

        // Books a seat based on the provided input details and returns the booking response.
        Task<BookingResponseDto> BookSeatAsync(BookSeatInputDto input);
    }
}


// The ? Nullability Operator (Handling Absence of Data)
// Why ? after AvailableBusDto? The ? makes the return type nullable.

// What does it mean? It means the method can return an instance of AvailableBusDto or it can return null.

// Infrastructure Project - repositories. and Application Project.
// Defines the Contract application.contrasts(Defines the Contract (The "what" is provided). This project is the blueprint for the service layer.)

// Application  - Provides the Implementation (The "how" the business logic is executed). This project holds the actual code that performs the tasks defined by the contract.