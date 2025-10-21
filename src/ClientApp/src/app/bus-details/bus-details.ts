import { Component, OnInit, inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { SearchService } from '../features/search/services/search';
import {
  AvailableBus,
  SeatStatus,
  BookSeatInputDto,      // 👈 NEW: Import Booking Input DTO
  BookingResponseDto     // 👈 NEW: Import Booking Response DTO
} from '../features/search/models/available-bus.model';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { lastValueFrom } from 'rxjs'; // 👈 NEW: Utility for converting Observable to Promise

// --- Seat Status Codes ---
export const SeatStatusCodes = {
  AVAILABLE: 1,
  BLOCKED: 2,
  BOOKED: 3,
  SELECTED: 99
};


// Interface for a seat, including the client-side selection state
interface SeatGridItem extends SeatStatus {
  isSelected: boolean;
}

@Component({
  selector: 'app-bus-details',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './bus-details.html',
  styleUrls: ['./bus-details.css']
})
export class BusDetailsComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private searchService = inject(SearchService);

  scheduleId: string | null = null;
  busDetails: AvailableBus | null = null;
  isLoading = true;
  error: any = null;

  // Define seatRows to explicitly allow SeatGridItem OR null (for the Aisle)
  seatRows: (SeatGridItem | null)[][] = [];
  selectedSeats: SeatGridItem[] = [];
  seatStatusCodes = SeatStatusCodes;

  // --- FORM STATE (NEW) ---
  boardingPoint: string = '';
  droppingPoint: string = '';
  mobileNumber: string = '';
  isBooking = false; // Flag to disable button during booking

  constructor() { }

  ngOnInit(): void {
    this.scheduleId = this.route.snapshot.paramMap.get('id');

    if (this.scheduleId) {
      this.fetchBusDetails(this.scheduleId);
    } else {
      this.error = "No schedule ID provided in the URL.";
      this.isLoading = false;
    }
  }

  fetchBusDetails(id: string): void {
    // Reset any existing errors before fetching
    this.error = null;
    this.isLoading = true;

    this.searchService.getBusDetails(id).subscribe({
      next: (data) => {
        this.busDetails = data;
        this.isLoading = false;
        if (data.seatLayout) {
          this.processSeatData(data.seatLayout);
        }
        // Set default values for the form (if necessary, often better left blank for user input)
        this.boardingPoint = data.boardingPoint;
      },
      error: (err) => {
        this.error = 'Failed to load bus details.';
        console.error('API Error:', err);
        this.isLoading = false;
      }
    });
  }

  // Transforms the flat list of seats into a 2D grid
  private processSeatData(seatLayout: SeatStatus[]): void {
    // Before processing, clear previous selections.
  this.selectedSeats = [];
  this.seatRows = [];

  // Ignore sold male/female seats (status 4 and 5)
  seatLayout = seatLayout.filter(seat => seat.status !== 4 && seat.status !== 5);

  const rows = ['A', 'B', 'C', 'D', 'E', 'F', 'G', 'H'];
  seatLayout.sort((a, b) => a.seatNumber.localeCompare(b.seatNumber));

    rows.forEach(rowLetter => {
      const rowSeats = seatLayout
        .filter(seat => seat.seatNumber.startsWith(rowLetter))
        .map(seat => ({ ...seat, isSelected: false } as SeatGridItem));

      if (rowSeats.length === 4) {
        // Structured Row: [Left1, Left2, Aisle Gap, Right3, Right4]
        const structuredRow: (SeatGridItem | null)[] = [
          rowSeats[0],
          rowSeats[1],
          null, // Aisle Placeholder
          rowSeats[2],
          rowSeats[3]
        ];
        this.seatRows.push(structuredRow);
      }
    });
  }

  // Handles click events on seats
  toggleSeat(seat: SeatGridItem): void {
    if (seat.status !== SeatStatusCodes.AVAILABLE) {
      return;
    }

    seat.isSelected = !seat.isSelected;

    if (seat.isSelected) {
      this.selectedSeats.push(seat);
    } else {
      this.selectedSeats = this.selectedSeats.filter(s => s.seatNumber !== seat.seatNumber);
    }
  }

  // Helper for template to apply Tailwind classes based on seat status
 getSeatClass(seat: SeatGridItem): string {
  if (seat.isSelected) {
    return 'bg-green-500 border-green-700 hover:bg-green-600 cursor-pointer';
  }

  switch (seat.status) {
    case SeatStatusCodes.AVAILABLE:
      return 'bg-white border-gray-400 hover:bg-gray-100 cursor-pointer';
    case SeatStatusCodes.BLOCKED:
      return 'bg-gray-400 border-gray-500 cursor-not-allowed';
    case SeatStatusCodes.BOOKED:
      return 'bg-purple-400 border-purple-500 cursor-not-allowed';
    default:
      return 'bg-gray-200 border-gray-300 cursor-not-allowed';
  }
}


  // Calculates the total price of selected seats
  getTotalPrice(): number {
    return this.selectedSeats.reduce((sum, seat) => sum + seat.price, 0);
  }

  // --- NEW: Async Booking Logic ---
  async proceedToBooking(): Promise<void> {
    if (this.isBooking) return; // Prevent multiple clicks

    // 1. Client-Side Validation (using console.warn instead of alert for better UX)
    if (this.selectedSeats.length === 0) {
      console.warn("Validation Error: Please select at least one seat.");
      return;
    }
    if (!this.mobileNumber || !this.boardingPoint || !this.droppingPoint) {
      console.warn("Validation Error: Please complete all passenger details (Mobile, Boarding/Dropping).");
      return;
    }
    if (!this.scheduleId) {
      this.error = "Error: Trip details are missing. Cannot proceed.";
      return;
    }

    this.isBooking = true;
    this.error = null;

    // 2. Prepare Payload (BookSeatInputDto)
    const bookingPayload: BookSeatInputDto = {
      scheduleId: this.scheduleId,
      boardingPoint: this.boardingPoint,
      droppingPoint: this.droppingPoint,
      mobileNumber: this.mobileNumber,
      seatBookings: this.selectedSeats.map(seat => ({
        seatNumber: seat.seatNumber,
        price: seat.price
      }))
    };

    try {
      // 3. API Call: Convert Observable to Promise for async/await simplicity
      const booking$ = this.searchService.bookSeats(bookingPayload);
      const response = await lastValueFrom(booking$);

      // 4. Handle Success
      console.log("Booking successful:", response);
      this.error = `Booking successful! Reference ID: ${response.bookingId}. Your seats are confirmed.`; // Use the error display for success message temporarily

      // 5. Refresh Data
      this.resetStateAfterBooking();

    } catch (err: any) {
      // 6. Handle Error
      this.error = `Booking failed. ${err.error?.message || 'Server error or network connection failed.'}`;
      console.error('Booking Error:', err);

    } finally {
      this.isBooking = false;
    }
  }

  private resetStateAfterBooking(): void {
    // Clear form and refresh seat map
    this.selectedSeats = [];
    this.mobileNumber = '';
    this.droppingPoint = '';

    if (this.scheduleId) {
      this.fetchBusDetails(this.scheduleId); // Re-fetch to show updated seat status
    }
  }
}
