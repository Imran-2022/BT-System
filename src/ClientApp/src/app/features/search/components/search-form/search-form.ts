import { Component, inject, OnInit } from '@angular/core';
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
  
  // Autocomplete suggestion lists
  public fromSuggestions: string[] = [];
  public toSuggestions: string[] = [];

  /** Default search criteria for quick testing */
  public query: SearchQuery = {
    from: 'Dhaka',
    to: 'Rajshahi',
    // Default to tomorrow so the pre-filled search date stays in the supported range.
    journeyDate: new Date(Date.now() + 24 * 60 * 60 * 1000).toISOString().substring(0, 10)
  };

  public searchResults: AvailableBus[] | null = null;
  public hasSearchRun = false;
  public isLoading: boolean = false;
  public errorMessage: string | null = null;

  ngOnInit(): void {
    // Prime suggestions (empty query returns all known locations)
    this.searchService.getLocations('').subscribe(list => {
      this.fromSuggestions = list;
      this.toSuggestions = list;
    }, err => {
      // ignore suggestion errors
      console.warn('Failed to load location suggestions', err);
    });
  }

  public onFromInput(value: string): void {
    const q = value?.trim() ?? '';
    if (q.length === 0) {
      this.fromSuggestions = [];
      return;
    }
    this.searchService.getLocations(q).subscribe(list => this.fromSuggestions = list, () => this.fromSuggestions = []);
  }

  public onToInput(value: string): void {
    const q = value?.trim() ?? '';
    if (q.length === 0) {
      this.toSuggestions = [];
      return;
    }
    this.searchService.getLocations(q).subscribe(list => this.toSuggestions = list, () => this.toSuggestions = []);
  }

  public selectFrom(value: string) { this.query.from = value; this.fromSuggestions = []; }
  public selectTo(value: string) { this.query.to = value; this.toSuggestions = []; }

  /** Handles the form submission and fetches available buses */
  public onSubmit(): void {
    this.isLoading = true;
    this.errorMessage = null;
    this.searchResults = null;
    this.hasSearchRun = true;

    const startTime = Date.now();

    this.searchService.searchBuses(this.query).subscribe({
      next: (buses) => {
        const elapsed = Date.now() - startTime;
        const delay = Math.max(1000 - elapsed, 0);
        setTimeout(() => {
          this.searchResults = buses;
          this.isLoading = false;
        }, delay);
      },

      error: (err) => {
        const elapsed = Date.now() - startTime;
        const delay = Math.max(1000 - elapsed, 0);

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