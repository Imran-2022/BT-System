/**
 * Represents a bus boarding or dropping point.
 */
export interface PointOption {
    pointId: string;       // GUID of the point
    locationName: string;
    time: string;          // TimeSpan from C# backend, e.g., "06:00:00"
}

/**
 * Represents the status of a single seat.
 */
export interface SeatStatus {
    seatNumber: string;
    status: number;        // 1: Available, 2: Blocked, 3: Booked
    price: number;
}

/**
 * Represents an available bus schedule.
 */
export interface AvailableBus {
    busScheduleId: string;
    companyName: string;
    busName: string;
    busType: string;
    startTime: string;      // TimeSpan from C#
    arrivalTime: string;    // TimeSpan from C#
    seatsLeft: number;
    price: number;
    cancellationPolicy: string;

    boardingPoints: PointOption[];
    droppingPoints: PointOption[];
    seatLayout: SeatStatus[];
}

/**
 * Represents search criteria for buses.
 */
export interface SearchQuery {
    from: string;
    to: string;
    journeyDate: string;    // Format: YYYY-MM-DD
}

/**
 * Represents a seat booking item for a passenger.
 */
export interface SeatBookingItemDto {
    seatNumber: string;
    price: number;
    passengerName: string;  // Name of the passenger
}

/**
 * Payload for booking seats via the API.
 */
export interface BookSeatInputDto {
    scheduleId: string;
    boardingPointId: string;   // Use GUIDs
    droppingPointId: string;
    mobileNumber: string;
    seatBookings: SeatBookingItemDto[];
}

/**
 * Response from the booking API.
 */
export interface BookingResponseDto {
    bookingId: string;
    status: string;   // "Success" or "Failure"
    message: string;
}