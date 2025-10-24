import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })

export class ApiService {
  /** Base URL of the .NET API */
  private readonly baseUrl = 'http://localhost:5106/api';
  constructor(private http: HttpClient) { }

  /**
   * Sends a GET request.
   * @param path API endpoint (e.g., 'search')
   * @param params Optional query parameters
   * @returns Observable of the response
   */

  get<T>(path: string, params: HttpParams = new HttpParams()): Observable<T> {
    return this.http.get<T>(`${this.baseUrl}/${path}`, { params });
  }

  /**
   * Sends a POST request.
   * @param path API endpoint (e.g., 'Bookings/BookSeat')
   * @param body Payload to send (e.g., DTO)
   * @returns Observable of the response
   */

  post<T>(path: string, body: any): Observable<T> {
    return this.http.post<T>(`${this.baseUrl}/${path}`, body);
  }
}