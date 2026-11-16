import { TranslatePipe } from '../../../../shared/i18n/translate.pipe';
import {
  Component,
  OnInit,
  ChangeDetectorRef,
  inject
} from '@angular/core';

import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DialogModule } from 'primeng/dialog';
import { VehicleAssignmentService } from '../../../../features/vehicle-assignments/services/vehicle-assignment';
import { VehicleService } from '../../../../features/vehicles/services/vehicle';
import { Trailer } from '../../../../features/trailers/models/trailer';
import { TrailerService } from '../../../../features/trailers/services/trailer';
import { VehicleAssignment, CreateVehicleAssignment, UpdateVehicleAssignment } from '../../../../features/vehicle-assignments/models/vehicle-assignment';
import { Vehicle } from '../../../../features/vehicles/models/vehicle';
import { ConfirmDialogService } from '../../../../shared/services/confirm-dialog';
import { ResultDialogService } from '../../../../shared/services/result-dialog';



@Component({
  selector: 'app-vehicle-assignment',
  standalone: true,
  imports: [TranslatePipe, 
    CommonModule,
    FormsModule,
    DialogModule
  ],
  templateUrl: './vehicle-assignment.html',
  styleUrl: './vehicle-assignment.scss'
})
export class VehicleAssignmentComponent implements OnInit {

  private assignmentService = inject(VehicleAssignmentService);
  private vehicleService = inject(VehicleService);
  private trailerService = inject(TrailerService);

  private confirmDialog = inject(ConfirmDialogService);
  private resultDialog = inject(ResultDialogService);

  private cdr = inject(ChangeDetectorRef);

  assignments: VehicleAssignment[] = [];
  vehicles: Vehicle[] = [];
  trailers: Trailer[] = [];

  isLoading = false;
  isSaving = false;

  errorMessage = '';
  searchText = '';

  displayAssignmentDialog = false;
  isEditMode = false;
  selectedAssignmentId: number | null = null;

  newAssignment: CreateVehicleAssignment = this.getEmptyAssignment();

  ngOnInit(): void {
    this.loadAssignments();
    this.loadVehicles();
    this.loadTrailers();
  }

  getEmptyAssignment(): CreateVehicleAssignment {
    return {
      vehicleId: 0,
      trailerId: 0,
      startDate: '',
      endDate: null
    };
  }

  // Load Assignments
  loadAssignments(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.assignmentService.getAll().subscribe({
      next: (data) => {
        this.assignments = data;
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: (error) => {
        this.isLoading = false;
        this.errorMessage =
          error.error?.message || 'Failed to load assignments.';

        this.cdr.detectChanges();
      }
    });
  }

  // Load Vehicles
  loadVehicles(): void {
    this.vehicleService.getAll().subscribe({
      next: (data) => {
        this.vehicles = data;
        this.cdr.detectChanges();
      },
      error: () => {
        this.resultDialog.show(
          'Error',
          'Failed to load vehicles.',
          'error'
        );
      }
    });
  }

  // Load Trailers
  loadTrailers(): void {
    this.trailerService.getAll().subscribe({
      next: (data) => {
        this.trailers = data;
        this.cdr.detectChanges();
      },
      error: () => {
        this.resultDialog.show(
          'Error',
          'Failed to load trailers.',
          'error'
        );
      }
    });
  }

  // Search
  get filteredAssignments(): VehicleAssignment[] {
    const search = this.searchText.toLowerCase().trim();

    if (!search) {
      return this.assignments;
    }

    return this.assignments.filter(assignment =>
      assignment.plateNumber.toString().includes(search) ||
      this.getTrailerPlate(assignment.trailerId).includes(search) ||
      assignment.assignmentId.toString().includes(search)
    );
  }

  // Get Vehicle Plate
  getVehiclePlate(vehicleId: number): string {
    const vehicle = this.vehicles.find(v => v.vehicleId === vehicleId);

    return vehicle ? vehicle.plateNumber.toString() : '-';
  }

  // Get Trailer Plate
  getTrailerPlate(trailerId: number): string {
    const trailer = this.trailers.find(t => t.id === trailerId);

    return trailer ? trailer.plateNumber.toString() : '-';
  }

  // Add Dialog
  openAddDialog(): void {
    this.isEditMode = false;
    this.selectedAssignmentId = null;
    this.newAssignment = this.getEmptyAssignment();
    this.errorMessage = '';
    this.displayAssignmentDialog = true;
  }

  // Edit Dialog
  openEditDialog(assignment: VehicleAssignment): void {
    this.isEditMode = true;
    this.selectedAssignmentId = assignment.assignmentId;

    this.newAssignment = {
      vehicleId: assignment.vehicleId,
      trailerId: assignment.trailerId,
      startDate: assignment.startDate.substring(0, 10),
      endDate: assignment.endDate
        ? assignment.endDate.substring(0, 10)
        : null
    };

    this.errorMessage = '';
    this.displayAssignmentDialog = true;
  }

  // Close Dialog
  closeDialog(): void {
    this.displayAssignmentDialog = false;
    this.errorMessage = '';
  }

  // Save Assignment
  saveAssignment(): void {
    if (
      !this.newAssignment.vehicleId ||
      !this.newAssignment.trailerId ||
      !this.newAssignment.startDate
    ) {
      this.errorMessage = 'Please fill in all required fields.';
      return;
    }

    this.isSaving = true;
    this.errorMessage = '';

    const dto: CreateVehicleAssignment = {
      ...this.newAssignment,
      endDate: this.newAssignment.endDate || null
    };

    if (this.isEditMode && this.selectedAssignmentId !== null) {

      const updateDto: UpdateVehicleAssignment = dto;

      this.assignmentService.update(
        this.selectedAssignmentId,
        updateDto
      ).subscribe({
        next: () => {
          this.isSaving = false;
          this.displayAssignmentDialog = false;

          this.resultDialog.show(
            'Success',
            'Assignment updated successfully.',
            'success'
          );

          this.loadAssignments();
          this.cdr.detectChanges();
        },
        error: (error) => {
          this.isSaving = false;
          this.errorMessage =
            error.error?.message || 'Failed to update assignment.';

          this.cdr.detectChanges();
        }
      });

    } else {

      this.assignmentService.create(dto).subscribe({
        next: () => {
          this.isSaving = false;
          this.displayAssignmentDialog = false;

          this.resultDialog.show(
            'Success',
            'Assignment created successfully.',
            'success'
          );

          this.loadAssignments();
          this.cdr.detectChanges();
        },
        error: (error) => {
          this.isSaving = false;
          this.errorMessage =
            error.error?.message || 'Failed to create assignment.';

          this.cdr.detectChanges();
        }
      });
    }
  }

  // Confirm Delete
  confirmDelete(id: number): void {
    this.confirmDialog.show(
      () => this.deleteAssignment(id),
      'Confirm Delete',
      'Are you sure you want to delete this assignment?'
    );
  }

  // Delete Assignment
  deleteAssignment(id: number): void {
    this.assignmentService.delete(id).subscribe({
      next: () => {
        this.resultDialog.show(
          'Success',
          'Assignment deleted successfully.',
          'success'
        );

        this.loadAssignments();
      },
      error: (error) => {
        this.resultDialog.show(
          'Error',
          error.error?.message || 'Failed to delete assignment.',
          'error'
        );
      }
    });
  }

  // Active Assignments
  get activeAssignmentsCount(): number {
    return this.assignments.filter(a => !a.endDate).length;
  }

  // Completed Assignments
  get completedAssignmentsCount(): number {
    return this.assignments.filter(a => !!a.endDate).length;
  }
}
