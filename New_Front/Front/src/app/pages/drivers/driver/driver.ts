import { TranslatePipe } from '../../../shared/i18n/translate.pipe';

import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DialogModule } from 'primeng/dialog';
import {
  Driver,
  CreateDriver,
  UpdateDriver,
  DriverStatus
} from '../../../features/drivers/models/driver';
import { DriverService } from '../../../features/drivers/services/driver';
import { ConfirmDialogService } from '../../../shared/services/confirm-dialog';
import { ResultDialogService } from '../../../shared/services/result-dialog';

@Component({
  selector: 'app-driver',
  standalone: true,
  imports: [TranslatePipe, CommonModule, FormsModule, DialogModule],
  templateUrl: './driver.html',
  styleUrl: './driver.scss'
})
export class DriverComponent implements OnInit {

  drivers: Driver[] = [];

  isLoading = false;
  isSaving = false;

  searchText = '';

  displayDriverDialog = false;
  isEditMode = false;
  selectedDriverId: number | null = null;

  driverStatuses = [
    { label: 'Active', value: DriverStatus.Active },
    { label: 'Inactive', value: DriverStatus.Inactive },
    { label: 'Suspended', value: DriverStatus.Suspended }
  ];

  newDriver: CreateDriver = this.getEmptyDriver();

  constructor(
    private driverService: DriverService,
    private confirmDialog: ConfirmDialogService,
    private resultDialog: ResultDialogService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.loadDrivers();
  }

  getEmptyDriver(): CreateDriver {
    return {
      fullName: '',
      phone: '',
      licenseNumber: '',
      licenseExpiryDate: '',
      status: DriverStatus.Active
    };
  }

  get filteredDrivers(): Driver[] {
    const search = this.searchText.toLowerCase().trim();

    if (!search) {
      return this.drivers;
    }

    return this.drivers.filter(driver =>
      driver.fullName.toLowerCase().includes(search) ||
      driver.phone.includes(search) ||
      driver.licenseNumber.toLowerCase().includes(search)
    );
  }

  get activeDriversCount(): number {
    return this.drivers.filter(d => d.status === DriverStatus.Active).length;
  }

  get inactiveDriversCount(): number {
    return this.drivers.filter(d => d.status === DriverStatus.Inactive).length;
  }

  get suspendedDriversCount(): number {
    return this.drivers.filter(d => d.status === DriverStatus.Suspended).length;
  }

  loadDrivers(): void {
    this.isLoading = true;

    this.driverService.getAll().subscribe({
      next: (data) => {
        this.drivers = data;
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.isLoading = false;
        this.resultDialog.show(
          'Error',
          'Failed to load drivers.',
          'error'
        );
        this.cdr.detectChanges();
      }
    });
  }

  getStatusLabel(status: number): string {
    switch (status) {
      case DriverStatus.Active:
        return 'Active';
      case DriverStatus.Inactive:
        return 'Inactive';
      case DriverStatus.Suspended:
        return 'Suspended';
      default:
        return 'Unknown';
    }
  }

  getStatusClass(status: number): string {
    switch (status) {
      case DriverStatus.Active:
        return 'active';
      case DriverStatus.Inactive:
        return 'inactive';
      case DriverStatus.Suspended:
        return 'suspended';
      default:
        return '';
    }
  }

  openAddDialog(): void {
    this.isEditMode = false;
    this.selectedDriverId = null;
    this.newDriver = this.getEmptyDriver();
    this.displayDriverDialog = true;
  }

  openEditDialog(driver: Driver): void {
    this.isEditMode = true;
    this.selectedDriverId = driver.driverId;

    this.newDriver = {
      fullName: driver.fullName,
      phone: driver.phone,
      licenseNumber: driver.licenseNumber,
      licenseExpiryDate: driver.licenseExpiryDate.substring(0, 10),
      status: driver.status
    };

    this.displayDriverDialog = true;
  }

  closeDialog(): void {
    this.displayDriverDialog = false;
    this.newDriver = this.getEmptyDriver();
    this.selectedDriverId = null;
  }

  saveDriver(): void {
    if (
      !this.newDriver.fullName.trim() ||
      !this.newDriver.phone.trim() ||
      !this.newDriver.licenseNumber.trim() ||
      !this.newDriver.licenseExpiryDate
    ) {
      this.resultDialog.show(
        'Validation Error',
        'Please fill in all required fields.',
        'error'
      );
      return;
    }

    this.isSaving = true;

    if (this.isEditMode && this.selectedDriverId !== null) {
      const driverData: UpdateDriver = { ...this.newDriver };

      this.driverService.update(this.selectedDriverId, driverData).subscribe({
        next: () => {
          this.isSaving = false;
          this.closeDialog();
          this.resultDialog.show(
            'Success',
            'Driver updated successfully.',
            'success'
          );
          this.loadDrivers();
        },
        error: (err) => {
          this.isSaving = false;
          this.resultDialog.show(
            'Error',
            err.error?.message || 'Failed to update driver.',
            'error'
          );
        }
      });
    } else {
      this.driverService.create(this.newDriver).subscribe({
        next: () => {
          this.isSaving = false;
          this.closeDialog();
          this.resultDialog.show(
            'Success',
            'Driver added successfully.',
            'success'
          );
          this.loadDrivers();
        },
        error: (err) => {
          this.isSaving = false;
          this.resultDialog.show(
            'Error',
            err.error?.message || 'Failed to add driver.',
            'error'
          );
        }
      });
    }
  }

  confirmDelete(driver: Driver): void {
    this.confirmDialog.show(
      () => this.deleteDriver(driver.driverId),
      'Delete Driver',
      `Are you sure you want to delete ${driver.fullName}?`
    );
  }

  deleteDriver(id: number): void {
    this.driverService.delete(id).subscribe({
      next: () => {
        this.resultDialog.show(
          'Success',
          'Driver deleted successfully.',
          'success'
        );
        this.loadDrivers();
      },
      error: (err) => {
        this.resultDialog.show(
          'Cannot Delete Driver',
          err.error?.message || 'Failed to delete driver.',
          'error'
        );
      }
    });
  }
}
