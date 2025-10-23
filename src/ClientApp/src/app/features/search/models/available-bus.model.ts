// src/ClientApp/src/app/features/search/models/available-bus.model.ts

export interface PointOption {
    pointId: string; // The GUID for the point
    locationName: string;
    time: string; // TimeSpan from C# is string (e.g., "06:00:00")
}

export interface SeatStatus {
    seatNumber: string;
    status: number; // 1: Available, 2: Blocked, 3: Booked
    price: number;
}

export interface AvailableBus {
    busScheduleId: string;
    companyName: string;
    busName: string;
    busType: string;
    startTime: string; // TimeSpan from C#
    arrivalTime: string; // TimeSpan from C#
    seatsLeft: number;
    price: number;
    cancellationPolicy: string;
    
    // 🎯 FIX: REMOVED OLD simple properties and ADDED NEW ARRAY PROPERTIES
    // The properties the error messages were referring to:
    // boardingPoint: string; 
    // droppingPoint: string; 
    
    // 🎯 CORRECTED PROPERTIES (Matching the DTO from the Backend)
    boardingPoints: PointOption[];
    droppingPoints: PointOption[];

    seatLayout: SeatStatus[];
}

export interface SearchQuery {
    from: string;
    to: string;
    journeyDate: string;
}

export interface SeatBookingItemDto {
    seatNumber: string;
    price: number;
    // 🎯 NEW: Passenger Name
    passengerName: string; 
}

/**
 * The Data Transfer Object for the BookSeat API call.
 */
export interface BookSeatInputDto {
    scheduleId: string;
    // 🎯 CHANGE: Use Point IDs instead of names
    boardingPointId: string; 
    droppingPointId: string; 
    
    mobileNumber: string;
    seatBookings: SeatBookingItemDto[];
}

export interface BookingResponseDto {
    bookingId: string;
    status: string; // "Success" or "Failure"
    message: string;
}