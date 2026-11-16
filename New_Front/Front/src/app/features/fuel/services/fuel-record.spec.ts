import { TestBed } from '@angular/core/testing';
import { FuelRecord } from './fuel-record';

describe('FuelRecord', () => {
  let service: FuelRecord;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(FuelRecord);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
