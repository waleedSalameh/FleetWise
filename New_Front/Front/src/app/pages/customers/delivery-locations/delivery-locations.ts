import { TranslatePipe } from '../../../shared/i18n/translate.pipe';
import { Component, DestroyRef, OnInit, computed, inject, signal } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { DialogModule } from 'primeng/dialog';
import { Subject, catchError, finalize, map, of, switchMap } from 'rxjs';
import { DeliveryLocation, SaveDeliveryLocation } from '../../../features/delivery-locations/models/delivery-location';
import { DeliveryLocationService } from '../../../features/delivery-locations/services/delivery-location';
import { ConfirmDialogService } from '../../../shared/services/confirm-dialog';
import { ResultDialogService } from '../../../shared/services/result-dialog';

@Component({
  selector: 'app-delivery-locations',
  imports: [TranslatePipe, ReactiveFormsModule, DialogModule],
  templateUrl: './delivery-locations.html',
  styleUrl: './delivery-locations.scss'
})
export class DeliveryLocationsPage implements OnInit {
  private readonly service = inject(DeliveryLocationService);
  private readonly confirmDialog = inject(ConfirmDialogService);
  private readonly resultDialog = inject(ResultDialogService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly reload = new Subject<void>();

  readonly records = signal<DeliveryLocation[]>([]);
  readonly searchText = signal('');
  readonly isLoading = signal(false);
  readonly loadFailed = signal(false);
  readonly isSaving = signal(false);
  readonly deletingId = signal<number | null>(null);
  readonly dialogVisible = signal(false);
  readonly editingId = signal<number | null>(null);
  readonly filteredRecords = computed(() => {
    const search = this.searchText().trim().toLowerCase();
    return this.records().filter(record => !search || [record.name ?? ''].some(value => value.toLowerCase().includes(search)));
  });
  readonly totalRecords = computed(() => this.records().length);

  readonly form = new FormGroup({
    name: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.maxLength(150), Validators.pattern(/\S/)] })
  });

  ngOnInit(): void {
    this.reload.pipe(
      switchMap(() => {
        this.isLoading.set(true);
        this.loadFailed.set(false);
        return this.service.getAll().pipe(
          catchError((error: unknown) => {
            this.records.set([]);
            this.loadFailed.set(true);
            this.resultDialog.show('Error', this.errorMessage(error, 'Failed to load delivery locations.'), 'error');
            return of(null);
          }),
          finalize(() => this.isLoading.set(false))
        );
      }),
      takeUntilDestroyed(this.destroyRef)
    ).subscribe(records => { if (records) this.records.set(records); });
    this.loadRecords();
  }

  loadRecords(): void { this.reload.next(); }

  openAddDialog(): void {
    if (this.isSaving()) return;
    this.editingId.set(null);
    this.form.reset({ name: '' });
    this.dialogVisible.set(true);
  }

  openEditDialog(record: DeliveryLocation): void {
    if (this.isSaving()) return;
    this.editingId.set(record.deliveryLocationId);
    this.form.reset({ name: record.name ?? '' });
    this.dialogVisible.set(true);
  }

  closeDialog(): void { if (!this.isSaving()) this.dialogVisible.set(false); }

  saveRecord(): void {
    if (this.isSaving()) return;
    this.form.markAllAsTouched();
    const value = this.form.getRawValue();
    if (this.form.invalid) {
      this.resultDialog.show('Validation', 'Complete all required fields.', 'error');
      return;
    }
    const dto: SaveDeliveryLocation = { name: value.name.trim() };
    const id = this.editingId();
    this.isSaving.set(true);
    this.form.disable();
    const request = id === null ? this.service.create(dto).pipe(map(() => undefined)) : this.service.update(id, dto);
    request.pipe(
      finalize(() => { this.isSaving.set(false); this.form.enable(); }),
      takeUntilDestroyed(this.destroyRef)
    ).subscribe({
      next: () => {
        this.dialogVisible.set(false);
        this.resultDialog.show('Success', 'Delivery Location ' + (id === null ? 'added' : 'updated') + ' successfully.', 'success');
        this.loadRecords();
      },
      error: (error: unknown) => this.resultDialog.show('Error', this.errorMessage(error, 'Failed to save delivery location.'), 'error')
    });
  }

  confirmDelete(record: DeliveryLocation): void {
    if (this.deletingId() !== null || this.isSaving()) return;
    this.confirmDialog.show(() => this.deleteRecord(record.deliveryLocationId), 'Delete Delivery Location', 'Are you sure you want to delete this delivery location?');
  }

  private deleteRecord(id: number): void {
    if (this.deletingId() !== null) return;
    this.deletingId.set(id);
    this.service.delete(id).pipe(
      finalize(() => this.deletingId.set(null)),
      takeUntilDestroyed(this.destroyRef)
    ).subscribe({
      next: () => {
        this.resultDialog.show('Success', 'Delivery Location deleted successfully.', 'success');
        this.loadRecords();
      },
      error: (error: unknown) => this.resultDialog.show('Error', this.errorMessage(error, 'Failed to delete delivery location.'), 'error')
    });
  }

  private errorMessage(error: unknown, fallback: string): string {
    if (!(error instanceof HttpErrorResponse)) return fallback;
    const body: unknown = error.error;
    if (typeof body === 'object' && body !== null && 'message' in body && typeof body.message === 'string') return body.message;
    return typeof body === 'string' && body.trim() ? body : fallback;
  }
}
