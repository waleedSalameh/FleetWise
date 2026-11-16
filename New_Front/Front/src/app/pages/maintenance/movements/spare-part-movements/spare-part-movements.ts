import { TranslatePipe } from '../../../../shared/i18n/translate.pipe';
import { Component, DestroyRef, OnInit, computed, inject, signal } from '@angular/core';
import { DatePipe, DecimalPipe } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { DialogModule } from 'primeng/dialog';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { Subject, catchError, finalize, forkJoin, of, switchMap, throwError } from 'rxjs';

import { SparePart } from '../../../../features/maintenance/models/spare-part';
import {
  CreateSparePartMovement,
  InventoryMovementType,
  SparePartMovement
} from '../../../../features/maintenance/models/spare-part-movement';
import { SparePartService } from '../../../../features/maintenance/services/spare-part';
import { ResultDialogService } from '../../../../shared/services/result-dialog';
import { SparePartMovementService } from '../../../../features/maintenance/services/spare-part-movement';

@Component({
  imports: [
    DatePipe,
    DecimalPipe,
    ReactiveFormsModule,
    DialogModule,
    RouterLink,
    TranslatePipe
  ],
  templateUrl: './spare-part-movements.html',
  styleUrl: './spare-part-movements.scss'
})
export class SparePartMovementsComponent implements OnInit {
  private readonly movementService = inject(SparePartMovementService);
  private readonly sparePartService = inject(SparePartService);
  private readonly resultDialog = inject(ResultDialogService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly reload = new Subject<void>();
  private readonly stockRefresh = new Subject<number>();

  readonly movements = signal<SparePartMovement[]>([]);
  readonly spareParts = signal<SparePart[]>([]);
  readonly isLoading = signal(false);
  readonly loadFailed = signal(false);
  readonly partsLoadFailed = signal(false);
  readonly searchText = signal('');
  readonly selectedType = signal('');
  readonly selectedPartId = signal<number | null>(null);
  readonly selectedPartAvailable = computed(() => this.spareParts().some(part => part.sparePartId === this.selectedPartId()));
  readonly dialogVisible = signal(false);
  readonly isSaving = signal(false);
  readonly isLoadingStock = signal(false);
  readonly currentStock = signal<number | null>(null);
  readonly stockUnavailable = signal(false);
  readonly InventoryMovementType = InventoryMovementType;
  readonly movementTypes = [
    { value: InventoryMovementType.Receipt, label: 'Receipt' },
    { value: InventoryMovementType.Issue, label: 'Issue' },
    { value: InventoryMovementType.Return, label: 'Return' },
    { value: InventoryMovementType.Adjustment, label: 'Adjustment' }
  ];

  private readonly partsById = computed(() => new Map(
    this.spareParts().map(part => [part.sparePartId, part])
  ));
  readonly totalMovements = computed(() => this.movements().length);
  readonly totalReceived = computed(() => this.sumQuantities('Receipt'));
  readonly totalIssued = computed(() => this.sumQuantities('Issue'));
  readonly totalReturned = computed(() => this.sumQuantities('Return'));
  readonly filteredMovements = computed(() => {
    const search = this.searchText().trim().toLowerCase();
    const type = this.selectedType();
    return this.movements().filter(movement =>
      (this.selectedPartId() === null || movement.sparePartId === this.selectedPartId()) &&
      (!type || movement.movementType === type) &&
      (!search || [movement.partName, movement.partNumber, movement.referenceNumber, movement.notes]
        .some(value => (value ?? '').toLowerCase().includes(search)))
    );
  });

  readonly form = new FormGroup({
    sparePartId: new FormControl(0, { nonNullable: true, validators: [Validators.required, Validators.min(1)] }),
    movementType: new FormControl(InventoryMovementType.Receipt, { nonNullable: true, validators: [Validators.required] }),
    quantity: new FormControl<number | null>(null, [Validators.required, Validators.min(0.001)]),
    unitCost: new FormControl<number | null>(0, [Validators.required, Validators.min(0)]),
    date: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    referenceNumber: new FormControl('', { nonNullable: true, validators: [Validators.maxLength(100)] }),
    notes: new FormControl('', { nonNullable: true, validators: [Validators.maxLength(500)] })
  });

  ngOnInit(): void {
    this.route.queryParamMap.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(params => {
      const rawId = params.get('sparePartId');
      const id = rawId !== null && /^[1-9]\d*$/.test(rawId) ? Number(rawId) : null;
      this.selectedPartId.set(id !== null && Number.isSafeInteger(id) && id <= 2147483647 ? id : null);
    });
    this.reload.pipe(
      switchMap(() => {
        this.isLoading.set(true);
        this.loadFailed.set(false);
        this.partsLoadFailed.set(false);
        return forkJoin({
          movements: this.movementService.getAll(),
          parts: this.sparePartService.getAll().pipe(
            catchError(() => {
              this.partsLoadFailed.set(true);
              return of([] as SparePart[]);
            })
          )
        }).pipe(
          catchError((error: unknown) => {
            this.loadFailed.set(true);
            this.movements.set([]);
            this.spareParts.set([]);
            this.resultDialog.show('Error', this.getErrorMessage(error, 'Failed to load spare part movements.'), 'error');
            return of(null);
          }),
          finalize(() => this.isLoading.set(false))
        );
      }),
      takeUntilDestroyed(this.destroyRef)
    ).subscribe(data => {
      if (!data) return;
      this.movements.set([...data.movements].sort((a, b) =>
        new Date(b.date).getTime() - new Date(a.date).getTime() || b.sparePartMovementId - a.sparePartMovementId
      ));
      this.spareParts.set(data.parts);
      if (this.partsLoadFailed()) {
        this.resultDialog.show('Error', 'Failed to load spare parts. Adding movements and identifying units are unavailable.', 'error');
      }
    });

    this.form.controls.sparePartId.valueChanges.pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(id => this.stockRefresh.next(id));

    this.stockRefresh.pipe(
      switchMap(id => {
        this.currentStock.set(null);
        this.stockUnavailable.set(false);
        this.isLoadingStock.set(false);
        if (!this.partsById().has(id)) return of(null);
        this.isLoadingStock.set(true);
        return this.movementService.getCurrentStock(id).pipe(
          catchError(() => {
            this.stockUnavailable.set(true);
            return of(null);
          }),
          finalize(() => this.isLoadingStock.set(false))
        );
      }),
      takeUntilDestroyed(this.destroyRef)
    ).subscribe(stock => {
      this.currentStock.set(stock !== null && Number.isFinite(stock) ? stock : null);
      if (stock !== null && !Number.isFinite(stock)) this.stockUnavailable.set(true);
    });

    this.loadMovements();
  }

  loadMovements(): void {
    this.reload.next();
  }

  filterByPart(rawId: string): void {
    const id = Number(rawId);
    void this.router.navigate([], {
      relativeTo: this.route,
      queryParams: { sparePartId: Number.isInteger(id) && id > 0 ? id : null },
      queryParamsHandling: 'merge'
    });
  }

  getUnit(sparePartId: number): string {
    return this.partsById().get(sparePartId)?.unit ?? '';
  }

  openAddDialog(): void {
    if (this.isSaving() || this.isLoading()) return;
    if (this.partsLoadFailed() || !this.spareParts().length) {
      this.resultDialog.show('Error', 'An available spare part is required before adding a movement.', 'error');
      return;
    }
    const today = new Date();
    const selectedId = this.selectedPartId() ?? 0;
    const date = `${today.getFullYear()}-${String(today.getMonth() + 1).padStart(2, '0')}-${String(today.getDate()).padStart(2, '0')}`;
    this.form.reset({
      sparePartId: this.partsById().has(selectedId) ? selectedId : 0,
      movementType: InventoryMovementType.Receipt,
      quantity: null,
      unitCost: 0,
      date,
      referenceNumber: '',
      notes: ''
    });
    this.dialogVisible.set(true);
  }

  closeDialog(): void {
    if (!this.isSaving()) this.dialogVisible.set(false);
  }

  saveMovement(): void {
    if (this.isSaving()) return;
    this.form.markAllAsTouched();
    const value = this.form.getRawValue();
    const date = new Date(`${value.date}T00:00:00`);
    const validDate = /^\d{4}-\d{2}-\d{2}$/.test(value.date) && Number.isFinite(date.getTime()) &&
      date.getFullYear() === Number(value.date.slice(0, 4)) &&
      date.getMonth() + 1 === Number(value.date.slice(5, 7)) && date.getDate() === Number(value.date.slice(8, 10));

    if (this.form.invalid || !this.partsById().has(value.sparePartId) ||
      !this.movementTypes.some(type => type.value === value.movementType) || !validDate ||
      typeof value.quantity !== 'number' || !Number.isFinite(value.quantity) ||
      typeof value.unitCost !== 'number' || !Number.isFinite(value.unitCost)) {
      this.resultDialog.show('Validation', 'Please select a spare part and movement type, enter a valid date, a quantity of at least 0.001 and a non-negative unit cost.', 'error');
      return;
    }

    const dto: CreateSparePartMovement = {
      sparePartId: value.sparePartId,
      movementType: value.movementType,
      quantity: value.quantity,
      unitCost: value.unitCost,
      date: `${value.date}T00:00:00`,
      maintenanceRecordId: null,
      referenceNumber: value.referenceNumber.trim() || null,
      notes: value.notes.trim() || null
    };
    this.isSaving.set(true);
    this.form.disable({ emitEvent: false });

    const request = dto.movementType === InventoryMovementType.Issue
      ? this.movementService.getCurrentStock(dto.sparePartId).pipe(
        switchMap(stock => {
          if (!Number.isFinite(stock)) return throwError(() => new Error('Unable to verify the current stock. Please try again.'));
          this.currentStock.set(stock);
          if (dto.quantity > stock) return throwError(() => new Error('Insufficient stock for this issue.'));
          return this.movementService.create(dto);
        })
      )
      : this.movementService.create(dto);

    request.pipe(
      finalize(() => {
        this.isSaving.set(false);
        this.form.enable({ emitEvent: false });
      }),
      takeUntilDestroyed(this.destroyRef)
    ).subscribe({
      next: () => {
        this.stockRefresh.next(dto.sparePartId);
        this.dialogVisible.set(false);
        this.resultDialog.show('Success', 'Spare part movement added successfully.', 'success');
        this.loadMovements();
      },
      error: (error: unknown) => {
        this.resultDialog.show('Error', this.getErrorMessage(error, 'Failed to save spare part movement.'), 'error');
      }
    });
  }

  private sumQuantities(type: string): { unit: string; quantity: number }[] {
    const totals = new Map<string, number>();
    for (const movement of this.movements()) {
      if (movement.movementType !== type) continue;
      const unit = this.getUnit(movement.sparePartId) || `Unit unavailable (#${movement.sparePartId})`;
      totals.set(unit, (totals.get(unit) ?? 0) + movement.quantity);
    }
    return [...totals].map(([unit, quantity]) => ({ unit, quantity }));
  }

  private getErrorMessage(error: unknown, fallback: string): string {
    if (!(error instanceof HttpErrorResponse)) return error instanceof Error ? error.message : fallback;
    const body: unknown = error.error;
    if (typeof body === 'string' && body.trim()) return body;
    if (typeof body !== 'object' || body === null) return fallback;
    const payload = body as Record<string, unknown>;
    if (typeof payload['errors'] === 'object' && payload['errors'] !== null) {
      const messages = Object.values(payload['errors']).flatMap(value =>
        Array.isArray(value) ? value.filter((message): message is string => typeof message === 'string') : []
      );
      if (messages.length) return messages.join(' ');
    }
    for (const key of ['message', 'detail', 'title']) {
      const message = payload[key];
      if (typeof message === 'string' && message.trim()) return message;
    }
    return fallback;
  }
}
