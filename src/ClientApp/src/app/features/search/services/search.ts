import { Injectable, inject } from '@angular/core';
import { ApiService } from '../../../core/services/api';
import { AvailableBus, SearchQuery, BookSeatInputDto, BookingResponseDto } from '../models/available-bus.model';
import { Observable } from 'rxjs';
import { HttpParams } from '@angular/common/http';

@Injectable({ providedIn: 'root' })

export class SearchService {
  private apiService = inject(ApiService);

  /**
   * Fetch available buses based on search criteria.
   * @param query Search parameters including from, to, and journeyDate.
   * @returns Observable of an array of available buses.
   */
  searchBuses(query: SearchQuery): Observable<AvailableBus[]> {
    let params = new HttpParams()
      .append('from', query.from)
      .append('to', query.to)
      .append('journeyDate', query.journeyDate); // Format: YYYY-MM-DD

    return this.apiService.get<AvailableBus[]>('search', params);
  }

  /**
  * Fetch detailed information for a specific bus schedule by ID.
  * @param id GUID of the bus schedule.
  * @returns Observable of a single AvailableBus object.
  */
  getBusDetails(id: string): Observable<AvailableBus> {
    return this.apiService.get<AvailableBus>(`search/${id}`);
  }

  /**
   * Book selected seats.
   * @param input Booking payload (DTO).
   * @returns Observable of the booking response.
   */
  bookSeats(input: BookSeatInputDto): Observable<BookingResponseDto> {
    return this.apiService.post<BookingResponseDto>('Bookings/BookSeat', input);
  }
}