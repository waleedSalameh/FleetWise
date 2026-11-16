import { TranslatePipe } from '../../../shared/i18n/translate.pipe';
import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DialogModule } from 'primeng/dialog';
import { ExternalVehicleService } from '../../../features/external-vehicles/services/external-vehicle';
import { ExternalVehicle, CreateExternalVehicle } from '../../../features/external-vehicles/models/external-vehicle';
import { ConfirmDialogService } from '../../../shared/services/confirm-dialog';
import { ResultDialogService } from '../../../shared/services/result-dialog';

@Component({
  selector: 'app-external-vehicles',
  standalone: true,
  imports: [TranslatePipe, CommonModule, FormsModule, DialogModule],
  templateUrl: './external-vehicles.html',
  styleUrl: './external-vehicles.scss'
})
export class ExternalVehicles implements OnInit {

  // Services
  private vehicleService = inject(ExternalVehicleService);
  private confirmDialog = inject(ConfirmDialogService);
  private resultDialog = inject(ResultDialogService);
  private cdr = inject(ChangeDetectorRef);

  // Data
  vehicles: ExternalVehicle[] = [];
  searchText = '';

  get filteredVehicles(): ExternalVehicle[] {
    const search = this.searchText.trim().toLowerCase();

    if (!search) return this.vehicles;

    return this.vehicles.filter(vehicle =>
      vehicle.plateNumber.toString().includes(search) ||
      vehicle.driverName.toLowerCase().includes(search) ||
      vehicle.phoneNumber.includes(search)
    );
  }

  // Loading States
  isLoading = false;
  isSaving = false;

  // Dialog
  displayVehicleDialog = false;

  // Error
  errorMessage = '';
  // New Vehicle
  newVehicle: CreateExternalVehicle = this.getEmptyVehicle();

  ngOnInit(): void {
    this.loadVehicles();
  }

  // Empty Vehicle Object
  getEmptyVehicle(): CreateExternalVehicle {
    return {
      driverName: '',
      plateNumber: 0,
      phoneNumber: ''
    };
  }

  // Get All External Vehicles
  loadVehicles(): void {

    this.isLoading = true;
    this.errorMessage = '';

    this.vehicleService.getAll().subscribe({

      next: (data) => {

        this.vehicles = [...data];

        this.isLoading = false;

        this.cdr.detectChanges();
      },

      error: (error) => {

        console.error('API Error:', error);

        this.errorMessage =
          error.error?.message || 'Failed to load external vehicles.';

        this.isLoading = false;

        this.cdr.detectChanges();
      }

    });
  }

  // Open Add Dialog
  openAddDialog(): void {

    this.newVehicle = this.getEmptyVehicle();

    this.errorMessage = '';

    this.displayVehicleDialog = true;
  }

  // Close Dialog
  closeDialog(): void {

    if (this.isSaving) return;

    this.displayVehicleDialog = false;

    this.errorMessage = '';
  }

  // Validate Form
  isFormValid(): boolean {

    const vehicle = this.newVehicle;

    if (!vehicle.driverName.trim()) return false;

    if (vehicle.plateNumber <= 0) return false;

    // Jordanian Phone Number Validation
    if (!/^07[789]\d{7}$/.test(vehicle.phoneNumber)) return false;

    return true;
  }

  // Add External Vehicle
  saveVehicle(): void {

    if (!this.isFormValid()) {

      this.errorMessage = 'Please enter valid vehicle information.';

      return;
    }

    this.isSaving = true;

    this.errorMessage = '';

    const dto: CreateExternalVehicle = {
      ...this.newVehicle,
      driverName: this.newVehicle.driverName.trim(),
      phoneNumber: this.newVehicle.phoneNumber.trim()
    };

    this.vehicleService.create(dto).subscribe({

      next: () => {

        this.isSaving = false;

        this.displayVehicleDialog = false;

        this.resultDialog.show(
          'Added Successfully',
          'External vehicle added successfully.',
          'success'
        );

        this.loadVehicles();
      },

      error: (error) => {

        console.error('Add External Vehicle Error:', error);

        this.isSaving = false;

        this.resultDialog.show(
          'Add Failed',
          error.error?.message || 'Failed to add external vehicle.',
          'error'
        );

        this.cdr.detectChanges();
      }

    });
  }

  // Open Global Delete Confirmation
  confirmDelete(id: number): void {

    this.confirmDialog.show(
      () => this.deleteVehicle(id),
      'Confirm Delete',
      'Are you sure you want to delete this external vehicle?'
    );
  }

  // Delete External Vehicle
  deleteVehicle(id: number): void {

    this.vehicleService.delete(id).subscribe({

      next: () => {

        this.resultDialog.show(
          'Deleted Successfully',
          'External vehicle deleted successfully.',
          'success'
        );

        this.loadVehicles();
      },

      error: (error) => {

        console.error('Delete External Vehicle Error:', error);

        this.resultDialog.show(
          'Delete Failed',
          error.error?.message || 'Failed to delete external vehicle.',
          'error'
        );

      }

    });
  }

}
