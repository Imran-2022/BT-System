import { Component, Input } from '@angular/core';
import { CommonModule, DecimalPipe } from '@angular/common';
import { AvailableBus } from '../../models/available-bus.model';
import { RouterLink } from "@angular/router";

@Component({
  selector: 'app-search-results',
  standalone: true,
  imports: [CommonModule, DecimalPipe, RouterLink], // DecimalPipe is needed for price formatting
  templateUrl: './search-results.html',
})
export class SearchResultsComponent {
  // Input property to receive the bus data from SearchFormComponent
  @Input() buses: AvailableBus[] | null = null;
}