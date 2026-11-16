import { TestBed } from '@angular/core/testing';
import { ExternalVehicle } from './external-vehicle';

describe('ExternalVehicle', () => {
  let service: ExternalVehicle;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ExternalVehicle);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
