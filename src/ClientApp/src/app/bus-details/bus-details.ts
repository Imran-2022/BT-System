import { Component, OnInit, inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { SearchService } from '../features/search/services/search';
import {
    AvailableBus,
    SeatStatus,
    BookSeatInputDto,
} from '../features/search/models/available-bus.model';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { lastValueFrom } from 'rxjs';
import { BusInfoCard } from "./components/bus-info-card/bus-info-card";
import { BusSeatSelector } from './components/bus-seat-selector/bus-seat-selector';

// =============================
// Seat Status Codes for UI rendering and logic
// =============================
export const SeatStatusCodes = {
    AVAILABLE: 1,
    BLOCKED: 2,
    BOOKED: 3,
    SELECTED: 99
};

// =============================
// Interfaces for Seats
// =============================

// Represents a seat in the UI with selection state
interface SeatGridItem extends SeatStatus {
    isSelected: boolean;
}

// Represents a selected seat with passenger info for booking
interface SelectedSeatDetails extends SeatGridItem {
    passengerName: string;
}

@Component({
    selector: 'app-bus-details',
    standalone: true,
    imports: [CommonModule, FormsModule, BusInfoCard,BusSeatSelector],
    templateUrl: './bus-details.html',
})
export class BusDetailsComponent implements OnInit {
    public currentDate: Date = new Date(); // Displayed in UI
    private route = inject(ActivatedRoute); // ActivatedRoute for scheduleId
    private searchService = inject(SearchService); // Service for API calls

    // =============================
    // State variables
    // =============================
    scheduleId: string | null = null;
    busDetails: AvailableBus | null = null;
    isLoading = true;
    error: any = null;

    // Seat data
    seatRows: (SeatGridItem | null)[][] = [];
    selectedSeats: SelectedSeatDetails[] = [];
    seatStatusCodes = SeatStatusCodes;

    // Booking form data
    boardingPointId: string = '';
    droppingPointId: string = '';
    mobileNumber: string = '';
    isBooking = false;
    isBooked = false; // Controls booking confirmation overlay

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

    /**
     * Fetches bus details and initializes seat grid and points.
     */
    fetchBusDetails(id: string): void {
        this.error = null;
        this.isLoading = true;
        const startTime = Date.now();

        this.searchService.getBusDetails(id).subscribe({
            next: async (data) => {
                this.busDetails = data;

                if (data.seatLayout) {
                    this.processSeatData(data.seatLayout);
                }

                // Set default boarding and dropping points
                if (data.boardingPoints.length && !this.boardingPointId) {
                    this.boardingPointId = data.boardingPoints[0].pointId;
                }
                if (data.droppingPoints.length && !this.droppingPointId) {
                    this.droppingPointId = data.droppingPoints[0].pointId;
                }

                // Simulate minimum loading time
                const elapsed = Date.now() - startTime;
                if (elapsed < 3000) {
                    await new Promise(resolve => setTimeout(resolve, 3000 - elapsed));
                }

                this.isLoading = false;
            },
            error: async (err) => {
                this.error = 'Failed to load bus details.';
                const elapsed = Date.now() - startTime;
                if (elapsed < 3000) {
                    await new Promise(resolve => setTimeout(resolve, 3000 - elapsed));
                }
                this.isLoading = false;
            }
        });
    }

    /**
     * Converts flat seat list into structured 2D grid for rendering.
     */
    private processSeatData(seatLayout: SeatStatus[]): void {
        this.selectedSeats = []; // Reset selected seats
        this.seatRows = [];
        const rows = ['A', 'B', 'C', 'D', 'E', 'F', 'G', 'H'];

        seatLayout.sort((a, b) => a.seatNumber.localeCompare(b.seatNumber, undefined, { numeric: true, sensitivity: 'base' }));

        rows.forEach(rowLetter => {
            const rowSeats = seatLayout
                .filter(seat => seat.seatNumber.startsWith(rowLetter))
                .map(seat => ({ ...seat, isSelected: false } as SeatGridItem));

            if (rowSeats.length === 0) return;

            let structuredRow: (SeatGridItem | null)[] = [];

            // =============================
            // Layout handling: This is highly custom
            // If buses have variable layouts, consider making this dynamic
            // =============================
            
            if (rowSeats.length === 4) {
                // 2x2 Layout with aisle placeholder
                structuredRow = [rowSeats[0], rowSeats[1], null, rowSeats[2], rowSeats[3]];
            } else if (rowSeats.length === 3) {
                // 2x1 Layout example
                structuredRow = [rowSeats[0], null, rowSeats[1], rowSeats[2]];
            } else {
                return; // Ignore unexpected row layouts
            }

            this.seatRows.push(structuredRow);
        });
    }

    /**
     * Toggle selection for available seats.
     */
    toggleSeat(seat: SeatGridItem): void {
        if (seat.status !== SeatStatusCodes.AVAILABLE) return;

        const existing = this.selectedSeats.find(s => s.seatNumber === seat.seatNumber);
        if (existing) {
            seat.isSelected = false;
            this.selectedSeats = this.selectedSeats.filter(s => s.seatNumber !== seat.seatNumber);
        } else {
            seat.isSelected = true;
            this.selectedSeats.push({ ...seat, passengerName: '' } as SelectedSeatDetails);
        }
    }

    updatePassengerName(seatNumber: string, name: string): void {
        const seat = this.selectedSeats.find(s => s.seatNumber === seatNumber);
        if (seat) seat.passengerName = name;
    }

    /**
     * Validates the booking form before submission.
     */
    isBookingFormValid(): boolean {
        const hasSeats = this.selectedSeats.length > 0;
        const allNamesEntered = this.selectedSeats.every(s => s.passengerName?.trim().length > 0);
        const mobileValid = /^[0-9]{11}$/.test(this.mobileNumber || '');
        const hasPointsAndMobile = !!this.boardingPointId && !!this.droppingPointId && mobileValid;

        return hasSeats && allNamesEntered && hasPointsAndMobile;
    }

    /**
     * Returns the Tailwind CSS class for a seat.
     */
    getSeatClass(seat: SeatGridItem): string {
        if (seat.isSelected) return 'bg-green-500 border-green-700 hover:bg-green-600 cursor-pointer text-white';
        switch (seat.status) {
            case SeatStatusCodes.AVAILABLE: return 'bg-white border-gray-400 hover:bg-gray-100 cursor-pointer';
            case SeatStatusCodes.BLOCKED: return 'bg-gray-400 border-gray-500 cursor-not-allowed text-white';
            case SeatStatusCodes.BOOKED: return 'bg-purple-400 border-purple-500 cursor-not-allowed text-white';
            default: return 'bg-gray-200 border-gray-300 cursor-not-allowed';
        }
    }

    getTotalPrice(): number {
        return this.selectedSeats.reduce((sum, seat) => sum + seat.price, 0);
    }

    /**
     * Processes the booking request.
     */
    async proceedToBooking(): Promise<void> {
        if (this.isBooking) return;

        if (!this.isBookingFormValid()) {
            this.error = "Validation Error: Please ensure all fields are filled, including names for all seats and a valid 11-digit mobile number.";
            return;
        }

        if (!this.scheduleId) {
            this.error = "Error: Trip details are missing. Cannot proceed.";
            return;
        }

        this.isBooking = true;
        this.error = null;

        const bookingPayload: BookSeatInputDto = {
            scheduleId: this.scheduleId,
            boardingPointId: this.boardingPointId,
            droppingPointId: this.droppingPointId,
            mobileNumber: this.mobileNumber,
            seatBookings: this.selectedSeats.map(seat => ({
                seatNumber: seat.seatNumber,
                price: seat.price,
                passengerName: seat.passengerName
            }))
        };

        try {
            await lastValueFrom(this.searchService.bookSeats(bookingPayload));
            this.isBooked = true;
            this.resetFormAfterBookingSuccess();

        } catch (err: any) {
            this.error = `Booking failed. ${err.error?.message || 'Server error or network connection failed.'}`;
            console.error('Booking Error:', err);

        } finally {
            this.isBooking = false;
        }
    }

    /**
     * Clears form after successful booking but keeps seat map static for confirmation.
     */
    private resetFormAfterBookingSuccess(): void {
        this.selectedSeats = [];
        this.mobileNumber = '';
    }

    /**
     * Resets the entire component state for a new booking attempt.
     */
    resetBooking(): void {
        this.isBooked = false;
        this.error = null;
        this.selectedSeats = [];
        this.mobileNumber = '';

        if (this.busDetails?.boardingPoints.length) {
            this.boardingPointId = this.busDetails.boardingPoints[0].pointId;
        }
        if (this.busDetails?.droppingPoints.length) {
            this.droppingPointId = this.busDetails.droppingPoints[0].pointId;
        }

        // Re-fetch data to refresh seat map
        if (this.scheduleId) {
            this.fetchBusDetails(this.scheduleId);
        }
    }
    // =============================
    // TODO / Future Improvements:
    // 1. Make seat row detection dynamic instead of using a fixed `rows` array. 
    //    This will allow handling buses with any row labels automatically.
    // 2. Improve seat layout processing to handle variable row lengths and configurations
    //    instead of only 3 or 4 seats per row. This will prevent seats from being skipped
    //    for buses with custom or irregular layouts.
    // 3. Consider generating aisle positions dynamically based on seat count or bus layout metadata.
    // =============================

}
