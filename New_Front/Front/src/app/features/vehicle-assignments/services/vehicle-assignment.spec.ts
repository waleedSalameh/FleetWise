import { TestBed } from '@angular/core/testing';
import { VehicleAssignment } from './vehicle-assignment';

describe('VehicleAssignment', () => {
  let service: VehicleAssignment;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(VehicleAssignment);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
