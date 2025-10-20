import { Component, Input } from '@angular/core';
import { CommonModule, DecimalPipe } from '@angular/common';
import { AvailableBus } from '../../models/available-bus.model';

@Component({
  selector: 'app-search-results',
  standalone: true,
  imports: [CommonModule, DecimalPipe], // DecimalPipe is needed for price formatting
  templateUrl: './search-results.html',
  styleUrls: ['./search-results.css']
})
export class SearchResultsComponent {
  // Input property to receive the bus data from SearchFormComponent
  @Input() buses: AvailableBus[] | null = null;
}