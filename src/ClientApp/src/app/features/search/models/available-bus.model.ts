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
}

export interface SearchQuery {
    from: string;
    to: string;
    journeyDate: string; // YYYY-MM-DD
}