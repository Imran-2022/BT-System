import { Component, Input } from '@angular/core';
import { AvailableBus } from '../../../features/search/models/available-bus.model';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-bus-info-card',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './bus-info-card.html',
})
export class BusInfoCard {
  @Input() busDetails!: AvailableBus;
  @Input() currentDate!: Date;
}
