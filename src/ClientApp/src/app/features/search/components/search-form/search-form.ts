import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SearchService } from '../../services/search';
import { AvailableBus, SearchQuery } from '../../models/available-bus.model';
import { SearchResultsComponent } from '../search-results/search-results';

@Component({
  selector: 'app-search-form',
  standalone: true,
  imports: [CommonModule, FormsModule, SearchResultsComponent],
  templateUrl: './search-form.html',
})
export class SearchFormComponent {
  private searchService = inject(SearchService);

  /** Default search criteria for quick testing */
  public query: SearchQuery = {
    from: 'Dhaka',
    to: 'Rajshahi',
    // future date (today + 1 day)
    // journeyDate: new Date(new Date().getTime() + (24 * 60 * 60 * 1000)).toISOString().substring(0, 10)
    journeyDate: '2025-11-02' // Fixed date (YYYY-MM-DD)
  };

  public searchResults: AvailableBus[] | null = null;
  public isLoading: boolean = false;
  public errorMessage: string | null = null;

  /** Handles the form submission and fetches available buses */
  public onSubmit(): void {
    this.isLoading = true;
    this.errorMessage = null;
    this.searchResults = null;

    const startTime = Date.now();

    this.searchService.searchBuses(this.query).subscribe({
      next: (buses) => {
        const elapsed = Date.now() - startTime;
        const delay = Math.max(3000 - elapsed, 0);
        setTimeout(() => {
          this.searchResults = buses;
          this.isLoading = false;
        }, delay);
      },

      error: (err) => {
        const elapsed = Date.now() - startTime;
        const delay = Math.max(3000 - elapsed, 0);

        setTimeout(() => {
          console.error('Search API Error:', err);
          const message = err?.statusText || 'Unable to fetch buses. Please check the API connection.';
          this.errorMessage = `Error fetching buses: ${message}`;
          this.isLoading = false;
          this.searchResults = [];
        }, delay);
      }
    });
  }
}