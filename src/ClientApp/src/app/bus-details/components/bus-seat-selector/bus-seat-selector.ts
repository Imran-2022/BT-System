import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SeatStatus } from '../../../features/search/models/available-bus.model';

interface SeatGridItem extends SeatStatus {
  isSelected: boolean;
}

@Component({
  selector: 'app-bus-seat-selector',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './bus-seat-selector.html',
})
export class BusSeatSelector {
  @Input() seatRows: (SeatGridItem | null)[][] = [];
  @Input() getSeatClass!: (seat: SeatGridItem) => string;
  @Input() toggleSeat!: (seat: SeatGridItem) => void;
}
