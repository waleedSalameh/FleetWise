import { TranslatePipe } from '../../../shared/i18n/translate.pipe';

import { Component, OnInit, ChangeDetectorRef, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DialogModule } from 'primeng/dialog';
import { TrailerService } from '../../../features/trailers/services/trailer';
import { Trailer, CreateTrailer, UpdateTrailer } from '../../../features/trailers/models/trailer';
import { ConfirmDialogService } from '../../../shared/services/confirm-dialog';
import { ResultDialogService } from '../../../shared/services/result-dialog';
import { SelectModule } from 'primeng/select';
@Component({
  selector: 'app-trailer',
  standalone: true,
  imports: [TranslatePipe, 
    CommonModule,
    FormsModule,
    DialogModule,
    SelectModule
  ],
  templateUrl: './trailers.html',
  styleUrl: './trailers.scss'
})
export class TrailerComponent implements OnInit {

  private trailerService = inject(TrailerService);
  private confirmDialog = inject(ConfirmDialogService);
  private resultDialog = inject(ResultDialogService);
  private cdr = inject(ChangeDetectorRef);

  // Trailers
  trailers: Trailer[] = [];

  // Loading & Errors
  isLoading = false;
  isSaving = false;
  errorMessage = '';

  // Search
  searchText = '';

  // Dialog
  displayTrailerDialog = false;
  isEditMode = false;
  selectedTrailerId: number | null = null;

  // Form
  newTrailer: CreateTrailer = this.getEmptyTrailer();

  ngOnInit(): void {
    this.loadTrailers();
  }

  // Empty Trailer
  getEmptyTrailer(): CreateTrailer {
    return {
      plateNumber: 0,
      axleCount: 1,
      maxLoad: 0,
      trailerType: 1,
      licenseExpiryDate: ''
    };
  }

  // Load Trailers
  loadTrailers(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.trailerService.getAll().subscribe({
      next: (data) => {
        this.trailers = data;
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: (error) => {
        this.isLoading = false;
        this.errorMessage =
          error.error?.message || 'Failed to load trailers.';

        this.cdr.detectChanges();
      }
    });
  }

  // Search
  get filteredTrailers(): Trailer[] {
    const search = this.searchText.trim().toLowerCase();

    if (!search) {
      return this.trailers;
    }

    return this.trailers.filter(trailer =>
      trailer.plateNumber.toString().includes(search) ||
      trailer.axleCount.toString().includes(search) ||
      trailer.maxLoad.toString().includes(search) ||
      this.getTrailerTypeName(trailer.trailerType).toLowerCase().includes(search) ||
      trailer.licenseExpiryDate.includes(search)
    );
  }

  // Trailer Type Name
  getTrailerTypeName(type: number): string {
    switch (type) {
      case 1:
        return 'Container Transport';
      case 2:
        return 'Crude Oil';
      case 3:
        return 'Refrigerated';
      case 4:
        return 'Sides';
      default:
        return 'Unknown';
    }
  }

  // Count Trailers By Type
  getCountByType(type: number): number {
    return this.trailers.filter(trailer => trailer.trailerType === type).length;
  }

  // Open Add Dialog
  openAddDialog(): void {
    this.isEditMode = false;
    this.selectedTrailerId = null;
    this.newTrailer = this.getEmptyTrailer();
    this.errorMessage = '';
    this.displayTrailerDialog = true;
  }

  // Open Edit Dialog
  openEditDialog(trailer: Trailer): void {
    this.isEditMode = true;
    this.selectedTrailerId = trailer.id;
    this.errorMessage = '';

    this.newTrailer = {
      plateNumber: trailer.plateNumber,
      axleCount: trailer.axleCount,
      maxLoad: trailer.maxLoad,
      trailerType: trailer.trailerType,
      licenseExpiryDate: trailer.licenseExpiryDate
    };

    this.displayTrailerDialog = true;
  }

  // Close Dialog
  closeDialog(): void {
    if (this.isSaving) {
      return;
    }

    this.displayTrailerDialog = false;
    this.errorMessage = '';
  }

  // Save Trailer
  saveTrailer(): void {
    if (this.isSaving) {
      return;
    }

    this.errorMessage = '';

    if (
      !this.newTrailer.plateNumber ||
      this.newTrailer.axleCount <= 0 ||
      this.newTrailer.maxLoad <= 0 ||
      !this.newTrailer.licenseExpiryDate
    ) {
      this.errorMessage = 'Please enter all required information.';
      return;
    }

    this.isSaving = true;

    if (this.isEditMode && this.selectedTrailerId !== null) {
      const dto: UpdateTrailer = { ...this.newTrailer };

      this.trailerService.update(this.selectedTrailerId, dto).subscribe({
        next: () => {
          this.isSaving = false;
          this.displayTrailerDialog = false;

          this.resultDialog.show(
            'Updated Successfully',
            'Trailer information updated successfully.',
            'success'
          );

          this.loadTrailers();
        },
        error: (error) => {
          this.isSaving = false;
          this.errorMessage =
            error.error?.message || 'Failed to update trailer.';

          this.cdr.detectChanges();
        }
      });

    } else {
      const dto: CreateTrailer = { ...this.newTrailer };

      this.trailerService.create(dto).subscribe({
        next: () => {
          this.isSaving = false;
          this.displayTrailerDialog = false;

          this.resultDialog.show(
            'Added Successfully',
            'Trailer added successfully.',
            'success'
          );

          this.loadTrailers();
        },
        error: (error) => {
          this.isSaving = false;
          this.errorMessage =
            error.error?.message || 'Failed to add trailer.';

          this.cdr.detectChanges();
        }
      });
    }
  }

  // Confirm Delete
  confirmDelete(id: number): void {
    this.confirmDialog.show(
      () => this.deleteTrailer(id),
      'Confirm Delete',
      'Are you sure you want to delete this trailer?'
    );
  }

  // Delete Trailer
  deleteTrailer(id: number): void {
    this.trailerService.delete(id).subscribe({
      next: () => {
        this.resultDialog.show(
          'Deleted Successfully',
          'Trailer deleted successfully.',
          'success'
        );

        this.loadTrailers();
      },
      error: (error) => {
        this.resultDialog.show(
          'Delete Failed',
          error.error?.message ||
          'Failed to delete trailer. It may have an active assignment.',
          'error'
        );
      }
    });
  }

}
