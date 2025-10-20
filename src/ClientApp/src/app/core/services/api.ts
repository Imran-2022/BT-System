import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  // CRITICAL: Must match the HTTP port of your running .NET API
  private readonly baseUrl = 'http://localhost:5106/api';

  constructor(private http: HttpClient) { }

  /**
   * Performs a GET request to the API with optional query parameters.
   * @param path The endpoint path (e.g., 'search').
   * @param params Query parameters.
   */
  get<T>(path: string, params: HttpParams = new HttpParams()): Observable<T> {
    // Example: http://localhost:5106/api/search?from=Dhaka...
    return this.http.get<T>(`${this.baseUrl}/${path}`, { params });
  }
}