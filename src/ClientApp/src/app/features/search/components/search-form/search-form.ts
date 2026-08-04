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
  
  // Autocomplete suggestion state
  public fromSuggestions: string[] = [];
  public toSuggestions: string[] = [];
  public showFromSuggestions = false;
  public showToSuggestions = false;

  public minJourneyDate = '2026-08-01';
  public maxJourneyDate = '2026-11-30';

  /** Default search criteria for quick testing */
  public query: SearchQuery = {
    from: 'Dhaka',
    to: 'Rajshahi',
    // Default to today so the search form starts with the current date.
    journeyDate: new Date().toISOString().substring(0, 10)
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
      this.showFromSuggestions = false;
      return;
    }

    this.searchService.getLocations(q).subscribe(list => {
      this.fromSuggestions = list;
      this.showFromSuggestions = list.length > 0;
    }, () => {
      this.fromSuggestions = [];
      this.showFromSuggestions = false;
    });
  }

  public onToInput(value: string): void {
    const q = value?.trim() ?? '';
    if (q.length === 0) {
      this.toSuggestions = [];
      this.showToSuggestions = false;
      return;
    }

    this.searchService.getLocations(q).subscribe(list => {
      this.toSuggestions = list;
      this.showToSuggestions = list.length > 0;
    }, () => {
      this.toSuggestions = [];
      this.showToSuggestions = false;
    });
  }

  public selectFrom(value: string) {
    this.query.from = value;
    this.fromSuggestions = [];
    this.showFromSuggestions = false;
  }

  public selectTo(value: string) {
    this.query.to = value;
    this.toSuggestions = [];
    this.showToSuggestions = false;
  }

  /** Handles the form submission and fetches available buses */
  public onSubmit(): void {
    if (!this.query.journeyDate || this.query.journeyDate < this.minJourneyDate || this.query.journeyDate > this.maxJourneyDate) {
      this.errorMessage = `Please choose a journey date between ${this.minJourneyDate} and ${this.maxJourneyDate}.`;
      this.searchResults = [];
      this.hasSearchRun = true;
      return;
    }

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