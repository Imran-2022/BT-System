export interface AvailableBus {
  busScheduleId: string;
  companyName: string;
  busName: string;
  busType: string;
  startTime: string; // TimeSpan from C# is string
  boardingPoint: string;
  arrivalTime: string;
  droppingPoint: string;
  seatsLeft: number;
  price: number;
  cancellationPolicy: string;
  seatLayout: SeatStatus[]; 

}

export interface SearchQuery {
    from: string;
    to: string;
    journeyDate: string; // YYYY-MM-DD
}

// --- NEW INTERFACE ---
export interface SeatStatus {
    seatNumber: string;
    status: number; // 1: Available, 2: Blocked, etc.
    price: number;
}