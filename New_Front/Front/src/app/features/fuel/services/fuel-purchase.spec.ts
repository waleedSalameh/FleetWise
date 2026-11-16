import { TestBed } from '@angular/core/testing';
import { FuelPurchase } from './fuel-purchase';

describe('FuelPurchase', () => {
  let service: FuelPurchase;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(FuelPurchase);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
