// src/ClientApp/src/app/bus-details/bus-details.ts

import { Component, OnInit, inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { SearchService } from '../features/search/services/search';
import {
    AvailableBus,
    SeatStatus,
    BookSeatInputDto,
    BookingResponseDto,
    // 🎯 NEW: Import PointOption (Used for Boarding/Dropping points)
    PointOption 
} from '../features/search/models/available-bus.model';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { lastValueFrom } from 'rxjs'; 

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

// 🎯 NEW INTERFACE: Combines SeatGridItem with the required passenger details for booking
interface SelectedSeatDetails extends SeatGridItem {
    passengerName: string; // The key addition
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

    seatRows: (SeatGridItem | null)[][] = [];
    // 🎯 CHANGE: Use the new SelectedSeatDetails interface
    selectedSeats: SelectedSeatDetails[] = []; 
    seatStatusCodes = SeatStatusCodes;

    // --- FORM STATE (UPDATED) ---
    // 🎯 CHANGE: Now stores the Point ID strings (GUIDs)
    boardingPointId: string = ''; 
    droppingPointId: string = ''; 
    mobileNumber: string = '';
    isBooking = false;

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
        this.error = null;
        this.isLoading = true;

        // 🎯 FIX: Renamed GetBusDetails to match ISearchService update (GetScheduleAndSeatDetailsAsync)
        // Ensure your searchService.getBusDetails implementation uses the new method name in the backend service.
        this.searchService.getBusDetails(id).subscribe({
            next: (data) => {
                console.log("data checking here : ",data);
                this.busDetails = data;
                this.isLoading = false;
                if (data.seatLayout) {
                    this.processSeatData(data.seatLayout);
                }
                
                // 🎯 NEW: Set default point IDs to the first available option for convenience
                if (data.boardingPoints.length) {
                   this.boardingPointId = data.boardingPoints[0].pointId;
                }
                if (data.droppingPoints.length) {
                   this.droppingPointId = data.droppingPoints[0].pointId;
                }
            },
            error: (err) => {
                this.error = 'Failed to load bus details.';
                console.error('API Error:', err);
                this.isLoading = false;
            }
        });
    }

    // Transforms the flat list of seats into a 2D grid (Logic remains mostly the same)
    // src/ClientApp/src/app/bus-details/bus-details.ts

// ...

// Transforms the flat list of seats into a 2D grid
private processSeatData(seatLayout: SeatStatus[]): void {
    this.selectedSeats = [];
    this.seatRows = [];

    // The maximum row letters you expect (A-H is 8 rows, adjust if needed)
    const rows = ['A', 'B', 'C', 'D', 'E', 'F', 'G', 'H']; 
    
    // Sort seats numerically (A1, A2, A3, A4) then alphabetically (B1, B2...)
    seatLayout.sort((a, b) => a.seatNumber.localeCompare(b.seatNumber, undefined, { numeric: true, sensitivity: 'base' }));

    rows.forEach(rowLetter => {
        const rowSeats = seatLayout
            .filter(seat => seat.seatNumber.startsWith(rowLetter))
            .map(seat => ({ ...seat, isSelected: false } as SeatGridItem));

        if (rowSeats.length === 0) {
            return; // Skip empty row letters
        }
        
        let structuredRow: (SeatGridItem | null)[] = [];

        // 🎯 FIX 3: Dynamic Logic for 2x2 (4 seats) vs. 2x1 (3 seats)
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
            // 2x1 Layout (or 1x2, typically 2 seats on one side, 1 on the other): 
            // Assumed layout: [Seat, Seat, Aisle, Seat]
            structuredRow = [
                rowSeats[0],
                null, // Aisle Placeholder
                rowSeats[1],
                rowSeats[2]
            ];
            
            // Alternative 2x1 rendering (1 seat, Aisle, 2 seats):
            /*
            structuredRow = [
                rowSeats[0],
                null, // Aisle Placeholder
                rowSeats[1],
                rowSeats[2]
            ];
            */
            
        } else {
             // Handle other unexpected counts if necessary, or just render them flat
             // structuredRow = rowSeats; 
             return; // Ignore complex layouts for now to avoid breaking the grid.
        }

        this.seatRows.push(structuredRow);
    });
}

    // Handles click events on seats
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
            // 🎯 CHANGE: Add 'passengerName: '' to the selection
            this.selectedSeats.push({ ...seat, passengerName: '' } as SelectedSeatDetails); 
        }
    }
    
    // 🎯 NEW: Helper to update passenger name for a selected seat (called from template)
    updatePassengerName(seatNumber: string, name: string): void {
        const seat = this.selectedSeats.find(s => s.seatNumber === seatNumber);
        if (seat) {
            seat.passengerName = name;
        }
    }
    
    // 🎯 NEW: Validation check for booking submission
    isBookingFormValid(): boolean {
        const hasSeats = this.selectedSeats.length > 0;
        // Check if all selected seats have a non-empty passenger name
        const allNamesEntered = this.selectedSeats.every(s => s.passengerName && s.passengerName.trim().length > 0);
        // Check if point IDs and mobile number are set
        const hasPointsAndMobile = !!this.mobileNumber && !!this.boardingPointId && !!this.droppingPointId;
        
        return hasSeats && allNamesEntered && hasPointsAndMobile;
    }


    // Helper for template to apply Tailwind classes based on seat status (No change needed)
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
        if (this.isBooking) return;

        // 1. Client-Side Validation
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

        // 2. Prepare Payload (BookSeatInputDto)
        const bookingPayload: BookSeatInputDto = {
            scheduleId: this.scheduleId,
            // 🎯 CHANGE: Use Point IDs
            boardingPointId: this.boardingPointId,
            droppingPointId: this.droppingPointId,
            mobileNumber: this.mobileNumber,
            seatBookings: this.selectedSeats.map(seat => ({
                seatNumber: seat.seatNumber,
                price: seat.price,
                // 🎯 NEW: Include Passenger Name
                passengerName: seat.passengerName 
            }))
        };

        try {
            // 3. API Call: Still calling bookSeats via searchService (which should internally call the Bookings API endpoint)
            const booking$ = this.searchService.bookSeats(bookingPayload); 
            const response = await lastValueFrom(booking$);

            // 4. Handle Success
            this.error = `Booking successful! Reference ID: ${response.bookingId}. Your seats are confirmed.`; 
            this.resetStateAfterBooking();

        } catch (err: any) {
            // 5. Handle Error
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
        
        // Reset point selection to defaults or clear, depending on desired UX
        // We'll reset points to the default first one here:
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