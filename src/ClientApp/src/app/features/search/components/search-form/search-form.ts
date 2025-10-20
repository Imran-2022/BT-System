import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SearchService } from '../../services/search';
import { AvailableBus, SearchQuery } from '../../models/available-bus.model';
import { SearchResultsComponent } from '../search-results/search-results'; // Import results component

@Component({
  selector: 'app-search-form',
  standalone: true,
  // Ensure all modules/components used in the template are imported here
  imports: [CommonModule, FormsModule, SearchResultsComponent],
  templateUrl: './search-form.html',
  styleUrls: ['./search-form.css']
})
export class SearchFormComponent {
  private searchService = inject(SearchService);
  
  // Set default values matching the dummy data route for easy testing
  public query: SearchQuery = {
    from: 'Dhaka',
    to: 'Rajshahi',
    // Set a default future date (e.g., today + 1 day)
    journeyDate: new Date(new Date().getTime() + (24 * 60 * 60 * 1000)).toISOString().substring(0, 10)
  };

  public searchResults: AvailableBus[] | null = null;
  public isLoading: boolean = false;
  public errorMessage: string | null = null;

  public onSubmit(): void {
    this.isLoading = true;
    this.errorMessage = null;
    this.searchResults = null; // Clear old results

    this.searchService.searchBuses(this.query).subscribe({
      next: (buses) => {
        this.searchResults = buses;
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Search API Error:', err);
        // Display a user-friendly error message for the user
        this.errorMessage = `Error fetching buses: ${err.statusText || 'Check the .NET API server and ensure the route is correct.'}`;
        this.isLoading = false;
        this.searchResults = []; 
      }
    });
  }
}