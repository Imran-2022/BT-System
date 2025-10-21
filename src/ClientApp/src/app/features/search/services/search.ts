import { Injectable, inject } from '@angular/core';
import { ApiService } from '../../../core/services/api';
import { AvailableBus, SearchQuery,BookSeatInputDto,BookingResponseDto } from '../models/available-bus.model';
import { Observable } from 'rxjs';
import { HttpParams } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class SearchService {
  private apiService = inject(ApiService);

  /**
   * Fetches available buses based on search criteria.
   * @returns An Observable array of available buses.
   */
  searchBuses(query: SearchQuery): Observable<AvailableBus[]> {
    let params = new HttpParams();
    params = params.append('from', query.from);
    params = params.append('to', query.to);
    
    // The date must be in YYYY-MM-DD format for the C# API binding
    params = params.append('journeyDate', query.journeyDate); 

    return this.apiService.get<AvailableBus[]>('search', params);
  }

  // ------------------------------------------------------------------
  // NEW METHOD: Fetch details for a single bus schedule by ID
  // ------------------------------------------------------------------
  /**
   * Fetches detailed information for a specific bus schedule.
   * @param id The GUID of the bus schedule.
   * @returns An Observable of a single AvailableBus object.
   */
  getBusDetails(id: string): Observable<AvailableBus> {
    // The endpoint is GET /api/Search/{id}
    return this.apiService.get<AvailableBus>(`search/${id}`);
  }
  // ------------------------------------------------------------------
// NEW METHOD: Book selected seats
// ------------------------------------------------------------------
  /**
   * Sends the booking request to the backend API.
   * @param input The booking payload (DTO).
   * @returns An Observable of the booking response.
   */
  bookSeats(input: BookSeatInputDto): Observable<BookingResponseDto> {
      // Endpoint should be something like POST /api/Bookings/BookSeat
      return this.apiService.post<BookingResponseDto>('Bookings/BookSeat', input);
      
      // NOTE: Your backend (C# Task BookSeatAsync) should map to this POST request.
  }
}