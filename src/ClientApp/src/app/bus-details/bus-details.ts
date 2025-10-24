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

// Seat status codes for rendering and logic
export const SeatStatusCodes = {
    AVAILABLE: 1,
    BLOCKED: 2,
    BOOKED: 3,
    SELECTED: 99
};

// Represents a seat in the UI with selection state
interface SeatGridItem extends SeatStatus {
    isSelected: boolean;
}

// Represents a selected seat along with passenger info for booking
interface SelectedSeatDetails extends SeatGridItem {
    passengerName: string;
}

@Component({
    selector: 'app-bus-details',
    standalone: true,
    imports: [CommonModule, FormsModule],
    templateUrl: './bus-details.html',
    styleUrls: ['./bus-details.css']
})
export class BusDetailsComponent implements OnInit {
    public currentDate: Date = new Date();
    private route = inject(ActivatedRoute);
    private searchService = inject(SearchService);
    // Current trip ID and bus details
    scheduleId: string | null = null;
    busDetails: AvailableBus | null = null;

    // Loading and error states
    isLoading = true;
    error: any = null;

    // Seats organized for UI grid
    seatRows: (SeatGridItem | null)[][] = [];
    selectedSeats: SelectedSeatDetails[] = [];
    seatStatusCodes = SeatStatusCodes;

    // Booking form state
    boardingPointId: string = '';
    droppingPointId: string = '';
    mobileNumber: string = '';
    isBooking = false;

    constructor() { }

    ngOnInit(): void {
        // Get schedule ID from route and fetch bus details
        this.scheduleId = this.route.snapshot.paramMap.get('id');
        if (this.scheduleId) {
            this.fetchBusDetails(this.scheduleId);
        } else {
            this.error = "No schedule ID provided in the URL.";
            this.isLoading = false;
        }
    }

    // Fetch bus details including seats and boarding/dropping points
    fetchBusDetails(id: string): void {
        this.error = null;
        this.isLoading = true;
        const startTime = Date.now();
        this.searchService.getBusDetails(id).subscribe({
            next: async (data) => {
                // console.log("response checking : ", data);
                this.busDetails = data;
                if (data.seatLayout) {
                    this.processSeatData(data.seatLayout);
                }
                // Set default boarding and dropping points
                if (data.boardingPoints.length) {
                    this.boardingPointId = data.boardingPoints[0].pointId;
                }
                if (data.droppingPoints.length) {
                    this.droppingPointId = data.droppingPoints[0].pointId;
                }
                // Ensure minimum 3 seconds loading
                const elapsed = Date.now() - startTime;
                if (elapsed < 3000) {
                    await new Promise(resolve => setTimeout(resolve, 3000 - elapsed));
                }

                this.isLoading = false;
            },
            error: async (err) => {
                this.error = 'Failed to load bus details.';
                // console.error('API Error:', err);
                
                // Ensure minimum 3 seconds loading
                const elapsed = Date.now() - startTime;
                if (elapsed < 3000) {
                    await new Promise(resolve => setTimeout(resolve, 3000 - elapsed));
                }
                this.isLoading = false;
            }
        });
    }

    // Transform flat seat list into 2D grid for rendering
    private processSeatData(seatLayout: SeatStatus[]): void {
        this.selectedSeats = [];
        this.seatRows = [];
        const rows = ['A', 'B', 'C', 'D', 'E', 'F', 'G', 'H'];

        // Sort seats by row and number
        seatLayout.sort((a, b) => a.seatNumber.localeCompare(b.seatNumber, undefined, { numeric: true, sensitivity: 'base' }));

        rows.forEach(rowLetter => {
            const rowSeats = seatLayout
                .filter(seat => seat.seatNumber.startsWith(rowLetter))
                .map(seat => ({ ...seat, isSelected: false } as SeatGridItem));

            if (rowSeats.length === 0) {
                return;
            }
            let structuredRow: (SeatGridItem | null)[] = [];

            // Handle common layouts: 2x2 or 2x1 seats
            if (rowSeats.length === 4) {
                // 2x2 Layout: [Seat, Seat, Aisle, Seat, Seat]
                structuredRow = [
                    rowSeats[0],
                    rowSeats[1],
                    null, // Aisle Placeholder
                    rowSeats[2],
                    rowSeats[3]
                ];
            } else if (rowSeats.length === 3) {
                // 2x1 Layout 
                // layout: [Seat, Aisle, Seat, Seat]
                structuredRow = [
                    rowSeats[0],
                    null, // Aisle Placeholder
                    rowSeats[1],
                    rowSeats[2]
                ];
            } else {
                return;  // Ignore unexpected layouts
            }
            this.seatRows.push(structuredRow);
        });
    }

    // Toggle seat selection
    toggleSeat(seat: SeatGridItem): void {
        if (seat.status !== SeatStatusCodes.AVAILABLE) {
            return;
        }
        const existingSelection = this.selectedSeats.find(s => s.seatNumber === seat.seatNumber);
        if (existingSelection) {
            // Deselect
            seat.isSelected = false;
            this.selectedSeats = this.selectedSeats.filter(s => s.seatNumber !== seat.seatNumber);
        } else {
            // Select and add to list with empty passenger name
            seat.isSelected = true;
            // CHANGE: Add 'passengerName: '' to the selection
            this.selectedSeats.push({ ...seat, passengerName: '' } as SelectedSeatDetails);
        }
    }

    // Update passenger name for a selected seat
    updatePassengerName(seatNumber: string, name: string): void {
        const seat = this.selectedSeats.find(s => s.seatNumber === seatNumber);
        if (seat) {
            seat.passengerName = name;
        }
    }

    // Validation check for booking submission
    isBookingFormValid(): boolean {
        const hasSeats = this.selectedSeats.length > 0;
        // Check if all selected seats have a non-empty passenger name
        const allNamesEntered = this.selectedSeats.every(s => s.passengerName && s.passengerName.trim().length > 0);
        // Check if point IDs and mobile number are set
        const hasPointsAndMobile = !!this.mobileNumber && !!this.boardingPointId && !!this.droppingPointId;
        return hasSeats && allNamesEntered && hasPointsAndMobile;
    }


    // Get CSS class for a seat based on status and selection
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


    // Calculate total price of selected seats
    getTotalPrice(): number {
        return this.selectedSeats.reduce((sum, seat) => sum + seat.price, 0);
    }

    // Submit booking request
    async proceedToBooking(): Promise<void> {
        if (this.isBooking) return;

        // Client-Side Validation
        if (!this.isBookingFormValid()) {
            this.error = "Validation Error: Please select seats, provide a mobile number, select Boarding/Dropping points, and enter a name for *each* selected seat.";
            return;
        }

        if (!this.scheduleId) {
            this.error = "Error: Trip details are missing. Cannot proceed.";
            return;
        }

        this.isBooking = true;
        this.error = null;

        // Prepare Payload (BookSeatInputDto)
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
            const booking$ = this.searchService.bookSeats(bookingPayload);
            const response = await lastValueFrom(booking$);

            // Handle Success
            this.error = `Booking successful! Reference ID: ${response.bookingId}. Your seats are confirmed.`;
            this.resetStateAfterBooking();

        } catch (err: any) {
            // Handle Error
            this.error = `Booking failed. ${err.error?.message || 'Server error or network connection failed.'}`;
            console.error('Booking Error:', err);

        } finally {
            this.isBooking = false;
        }
    }

    // Reset form and reload seats after booking
    private resetStateAfterBooking(): void {
        // Clear form and refresh seat map
        this.selectedSeats = [];
        this.mobileNumber = '';

        // reset points to the default first one here:
        if (this.busDetails?.boardingPoints.length) {
            this.boardingPointId = this.busDetails.boardingPoints[0].pointId;
        }
        if (this.busDetails?.droppingPoints.length) {
            this.droppingPointId = this.busDetails.droppingPoints[0].pointId;
        }

        if (this.scheduleId) {
            this.fetchBusDetails(this.scheduleId); // Re-fetch to show updated seat status
        }
    }
}