import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BusInfoCard } from './bus-info-card';

describe('BusInfoCard', () => {
  let component: BusInfoCard;
  let fixture: ComponentFixture<BusInfoCard>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BusInfoCard]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BusInfoCard);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
