import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ExternalVehicles } from './external-vehicles';

describe('ExternalVehicles', () => {
  let component: ExternalVehicles;
  let fixture: ComponentFixture<ExternalVehicles>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ExternalVehicles],
    }).compileComponents();

    fixture = TestBed.createComponent(ExternalVehicles);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
