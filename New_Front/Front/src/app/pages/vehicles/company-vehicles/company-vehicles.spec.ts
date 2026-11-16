import { ComponentFixture, TestBed } from '@angular/core/testing';
import { CompanyVehicles } from './company-vehicles';

describe('CompanyVehicles', () => {
  let component: CompanyVehicles;
  let fixture: ComponentFixture<CompanyVehicles>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CompanyVehicles],
    }).compileComponents();

    fixture = TestBed.createComponent(CompanyVehicles);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
