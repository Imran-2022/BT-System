import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BusSeatSelector } from './bus-seat-selector';

describe('BusSeatSelector', () => {
  let component: BusSeatSelector;
  let fixture: ComponentFixture<BusSeatSelector>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BusSeatSelector]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BusSeatSelector);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
