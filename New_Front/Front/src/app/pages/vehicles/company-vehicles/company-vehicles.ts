import { TranslatePipe } from '../../../shared/i18n/translate.pipe';
import { LanguageService } from '../../../shared/i18n/language.service';
import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DialogModule } from 'primeng/dialog';
import { VehicleService } from '../../../features/vehicles/services/vehicle';
import { Vehicle, CreateVehicle, UpdateVehicle } from '../../../features/vehicles/models/vehicle';
import { ConfirmDialogService } from '../../../shared/services/confirm-dialog';
import { ResultDialogService } from '../../../shared/services/result-dialog';

@Component({
  selector: 'app-company-vehicles',

  standalone: true,
  imports: [TranslatePipe, CommonModule, FormsModule, DialogModule],
  templateUrl: './company-vehicles.html',
  styleUrl: './company-vehicles.scss',
})
export class CompanyVehicles implements OnInit {
  // Services
  private vehicleService = inject(VehicleService);
  private confirmDialog = inject(ConfirmDialogService);
  private resultDialog = inject(ResultDialogService);
  private cdr = inject(ChangeDetectorRef);
  private readonly languageService = inject(LanguageService);

  // Data
  vehicles: Vehicle[] = [];
  // Search
  searchText = '';

  get filteredVehicles(): Vehicle[] {
    const search = this.searchText.trim().toLowerCase();

    if (!search) return this.vehicles;

    return this.vehicles.filter(vehicle =>
      vehicle.plateNumber.toString().includes(search) ||
      vehicle.make.toLowerCase().includes(search) ||
      vehicle.model.toLowerCase().includes(search) ||
      vehicle.year.toString().includes(search) ||
      vehicle.currentOdometer.toString().includes(search) ||
      this.languageService.text(this.getStatusName(vehicle.status)).toLowerCase().includes(search)
    );
  }

  // Get Status Name
  getStatusName(status: number): string {
    switch (status) {
      case 0: return 'Available';
      case 1: return 'In Use';
      case 2: return 'Under Maintenance';
      case 3: return 'Inactive';
      default: return '';
    }
  }

  // Loading States
  isLoading = false;
  isSaving = false;

  // Dialog
  displayVehicleDialog = false;
  isEditMode = false;
  selectedVehicleId: number | null = null;

  // Error
  errorMessage = '';

  // New Vehicle
  newVehicle: CreateVehicle = this.getEmptyVehicle();

  ngOnInit(): void {
    this.loadVehicles();
  }

  // Empty Vehicle Object
  getEmptyVehicle(): CreateVehicle {
    return {
      plateNumber: 0,
      make: '',
      year: 0,
      model: '',
      currentOdometer: 0,
      fuelTankCapacity: 0,
      status: 0,
    };
  }

  // Get All Vehicles
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

        this.errorMessage = error.error?.message || 'Failed to load vehicles.';

        this.isLoading = false;
        this.cdr.detectChanges();

        this.resultDialog.show('Loading Failed', this.errorMessage, 'error');
      },
    });
  }

  // Open Add Dialog
  openAddDialog(): void {
    this.isEditMode = false;
    this.selectedVehicleId = null;
    this.newVehicle = this.getEmptyVehicle();
    this.errorMessage = '';
    this.displayVehicleDialog = true;
  }

  // Open Edit Dialog
  openEditDialog(vehicle: Vehicle): void {
    this.isEditMode = true;
    this.selectedVehicleId = vehicle.vehicleId;

    this.newVehicle = {
      plateNumber: vehicle.plateNumber,
      make: vehicle.make,
      year: vehicle.year,
      model: vehicle.model,
      currentOdometer: vehicle.currentOdometer,
      fuelTankCapacity: vehicle.fuelTankCapacity,
      status: vehicle.status,
    };

    this.errorMessage = '';
    this.displayVehicleDialog = true;
  }

  // Close Dialog
  closeDialog(): void {
    if (this.isSaving) return;

    this.displayVehicleDialog = false;
    this.errorMessage = '';
  }

  // Form Validation
  isFormValid(): boolean {
    return (
      this.newVehicle.plateNumber > 0 &&
      !!this.newVehicle.make.trim() &&
      !!this.newVehicle.model.trim() &&
      this.newVehicle.year > 0 &&
      this.newVehicle.year <= new Date().getFullYear() &&
      this.newVehicle.currentOdometer >= 0 &&
      this.newVehicle.fuelTankCapacity > 0
    );
  }

  // Add / Update Vehicle
  saveVehicle(): void {
    if (!this.isFormValid()) {
      this.errorMessage = 'Please enter valid vehicle information.';
      return;
    }

    this.isSaving = true;
    this.errorMessage = '';

    if (this.isEditMode && this.selectedVehicleId !== null) {
      const dto: UpdateVehicle = { ...this.newVehicle };

      this.vehicleService.update(this.selectedVehicleId, dto).subscribe({
        next: () => {
          this.isSaving = false;
          this.displayVehicleDialog = false;

          this.resultDialog.show(
            'Updated Successfully',
            'Vehicle information updated successfully.',
            'success',
          );

          this.loadVehicles();
        },

        error: (error) => {
          console.error('Update Vehicle Error:', error);

          this.errorMessage = error.error?.message || 'Failed to update vehicle.';

          this.isSaving = false;

          this.resultDialog.show('Update Failed', this.errorMessage, 'error');

          this.cdr.detectChanges();
        },
      });
    } else {
      const dto: CreateVehicle = { ...this.newVehicle };

      this.vehicleService.create(dto).subscribe({
        next: () => {
          this.isSaving = false;
          this.displayVehicleDialog = false;

          this.resultDialog.show(
            'Added Successfully',
            'Company vehicle added successfully.',
            'success',
          );

          this.loadVehicles();
        },

        error: (error) => {
          console.error('Add Vehicle Error:', error);

          this.errorMessage = error.error?.message || 'Failed to add vehicle.';

          this.isSaving = false;

          this.resultDialog.show('Add Failed', this.errorMessage, 'error');

          this.cdr.detectChanges();
        },
      });
    }
  }

  // Show Delete Confirmation
  confirmDelete(id: number): void {
    this.confirmDialog.show(
      () => this.deleteVehicle(id),
      'Confirm Delete',
      'Are you sure you want to delete this company vehicle?',
    );
  }

  // Delete Vehicle
  deleteVehicle(id: number): void {
    this.vehicleService.delete(id).subscribe({
      next: () => {
        this.resultDialog.show(
          'Deleted Successfully',
          'Company vehicle deleted successfully.',
          'success',
        );

        this.loadVehicles();
      },

      error: (error) => {
        console.error('Delete Vehicle Error:', error);

        this.resultDialog.show(
          'Delete Failed',
          error.error?.message || 'Failed to delete company vehicle.',
          'error',
        );
      },
    });
  }

  // Get Vehicles Count By Status
  getCount(status: number): number {
    return this.vehicles.filter((v) => v.status === status).length;
  }
}
