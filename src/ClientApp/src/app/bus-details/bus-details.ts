import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { SearchService } from '../features/search/services/search';
import { AvailableBus } from '../features/search/models/available-bus.model';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-bus-details',
  imports: [CommonModule],
  templateUrl: './bus-details.html',
  styleUrl: './bus-details.css'
})

export class BusDetailsComponent implements OnInit {
  scheduleId: string | null = null;
  busDetails: AvailableBus | null = null;
  isLoading = true;
  error: any = null;

  constructor(
    private route: ActivatedRoute,
    private searchService: SearchService
  ) { }

  ngOnInit(): void {
    // 1. Get the ID from the URL parameter
    this.scheduleId = this.route.snapshot.paramMap.get('id');

    if (this.scheduleId) {
      this.fetchBusDetails(this.scheduleId);
    } else {
      this.error = "No schedule ID provided in the URL.";
      this.isLoading = false;
    }
  }

  fetchBusDetails(id: string): void {
    this.searchService.getBusDetails(id).subscribe({
      next: (data) => {
        this.busDetails = data;
        this.isLoading = false;
      },
      error: (err) => {
        this.error = 'Failed to load bus details.';
        console.error('API Error:', err);
        this.isLoading = false;
      }
    });
  }
}