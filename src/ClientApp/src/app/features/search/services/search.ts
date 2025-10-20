import { Injectable, inject } from '@angular/core';
import { ApiService } from '../../../core/services/api';
import { AvailableBus, SearchQuery } from '../models/available-bus.model';
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
}