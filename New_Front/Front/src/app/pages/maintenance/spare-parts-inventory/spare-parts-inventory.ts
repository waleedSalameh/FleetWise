import { Component, DestroyRef, OnInit, computed, inject, signal } from '@angular/core';
import { DecimalPipe } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { DialogModule } from 'primeng/dialog';
import { RouterLink } from '@angular/router';
import { Subject, catchError, finalize, from, map, mergeMap, of, switchMap, toArray } from 'rxjs';
import { CreateSparePart, SparePart } from '../../../features/maintenance/models/spare-part';
import { SparePartService } from '../../../features/maintenance/services/spare-part';
import { SparePartMovementService } from '../../../features/maintenance/services/spare-part-movement';
import { ConfirmDialogService } from '../../../shared/services/confirm-dialog';
import { ResultDialogService } from '../../../shared/services/result-dialog';
import { TranslatePipe } from '../../../shared/i18n/translate.pipe';

type StockStatus = 'loading' | 'unavailable' | 'out-of-stock' | 'low-stock' | 'in-stock';

@Component({
  selector: 'app-spare-parts-inventory',
  imports: [DecimalPipe, ReactiveFormsModule, DialogModule, RouterLink, TranslatePipe],
  templateUrl: './spare-parts-inventory.html',
  styleUrl: './spare-parts-inventory.scss'
})
export class SparePartsInventoryComponent implements OnInit {
  private readonly partService = inject(SparePartService);
  private readonly movementService = inject(SparePartMovementService);
  private readonly confirmDialog = inject(ConfirmDialogService);
  private readonly resultDialog = inject(ResultDialogService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly reload = new Subject<void>();

  readonly parts = signal<SparePart[]>([]);
  readonly stocks = signal<ReadonlyMap<number, number | null>>(new Map());
  readonly searchText = signal('');
  readonly selectedStatus = signal('');
  readonly isLoading = signal(false);
  readonly isLoadingStock = signal(false);
  readonly loadFailed = signal(false);
  readonly dialogVisible = signal(false);
  readonly isSaving = signal(false);
  readonly selectedPartId = signal<number | null>(null);
  readonly deletingPartId = signal<number | null>(null);
  readonly isEditMode = computed(() => this.selectedPartId() !== null);

  readonly filteredParts = computed(() => {
    const search = this.searchText().trim().toLowerCase();
    return this.parts().filter(part =>
      (!this.selectedStatus() || this.getStockStatus(part) === this.selectedStatus()) &&
      (!search || [part.partNumber, part.partName, part.brand, part.description].some(value =>
        (value ?? '').toLowerCase().includes(search)
      ))
    );
  });

  readonly totalParts = computed(() => this.parts().length);
  readonly inStockParts = computed(() => this.parts().filter(part =>
    this.getStockStatus(part) === 'in-stock'
  ).length);
  readonly lowStockParts = computed(() => this.parts().filter(part =>
    this.getStockStatus(part) === 'low-stock'
  ).length);
  readonly outOfStockParts = computed(() => this.parts().filter(part =>
    this.getStockStatus(part) === 'out-of-stock'
  ).length);
  readonly hasUnavailableStock = computed(() =>
    [...this.stocks().values()].some(stock => stock === null)
  );

  readonly form = new FormGroup({
    partNumber: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.maxLength(50), Validators.pattern(/\S/)]
    }),
    partName: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.maxLength(150), Validators.pattern(/\S/)]
    }),
    brand: new FormControl('', { nonNullable: true, validators: [Validators.maxLength(100)] }),
    description: new FormControl('', { nonNullable: true, validators: [Validators.maxLength(500)] }),
    unit: new FormControl('Piece', {
      nonNullable: true,
      validators: [Validators.required, Validators.maxLength(30), Validators.pattern(/\S/)]
    }),
    purchasePrice: new FormControl<number | null>(0, [Validators.required, Validators.min(0)]),
    minimumQuantity: new FormControl<number | null>(0, [Validators.required, Validators.min(0)])
  });

  ngOnInit(): void {
    this.reload.pipe(
      switchMap(() => {
        this.isLoading.set(true);
        this.isLoadingStock.set(false);
        this.loadFailed.set(false);
        this.parts.set([]);
        this.stocks.set(new Map());

        return this.partService.getAll().pipe(
          switchMap(parts => {
            this.parts.set(parts);
            this.isLoading.set(false);
            this.isLoadingStock.set(parts.length > 0);

            return from(parts).pipe(
              mergeMap(part => this.movementService.getCurrentStock(part.sparePartId).pipe(
                map(stock => ({ sparePartId: part.sparePartId, stock: Number.isFinite(stock) ? stock : null })),
                catchError(() => of({ sparePartId: part.sparePartId, stock: null }))
              ), 6),
              map(({ sparePartId, stock }) => {
                this.stocks.update(current => new Map(current).set(sparePartId, stock));
              }),
              toArray()
            );
          }),
          catchError((error: unknown) => {
            this.loadFailed.set(true);
            this.resultDialog.show('Error', this.getErrorMessage(error, 'Failed to load spare parts inventory.'), 'error');
            return of(null);
          }),
          finalize(() => {
            this.isLoading.set(false);
            this.isLoadingStock.set(false);
          })
        );
      }),
      takeUntilDestroyed(this.destroyRef)
    ).subscribe();

    this.loadParts();
  }

  loadParts(): void {
    this.reload.next();
  }

  getStock(sparePartId: number): number | null {
    return this.stocks().get(sparePartId) ?? null;
  }

  getStockStatus(part: SparePart): StockStatus {
    const stocks = this.stocks();
    if (!stocks.has(part.sparePartId)) return 'loading';
    const stock = stocks.get(part.sparePartId);
    if (stock === null || stock === undefined) return 'unavailable';
    if (stock <= 0) return 'out-of-stock';
    return stock < part.minimumQuantity ? 'low-stock' : 'in-stock';
  }

  getStockStatusLabel(part: SparePart): string {
    const labels: Record<StockStatus, string> = {
      loading: 'Loading...',
      unavailable: 'Unavailable',
      'out-of-stock': 'Out of Stock',
      'low-stock': 'Low Stock',
      'in-stock': 'In Stock'
    };
    return labels[this.getStockStatus(part)];
  }

  openAddDialog(): void {
    if (this.isSaving()) return;
    this.selectedPartId.set(null);
    this.form.reset({ partNumber: '', partName: '', brand: '', description: '', unit: 'Piece', purchasePrice: 0, minimumQuantity: 0 });
    this.dialogVisible.set(true);
  }

  openEditDialog(part: SparePart): void {
    if (this.isSaving()) return;
    this.selectedPartId.set(part.sparePartId);
    this.form.reset({
      partNumber: part.partNumber,
      partName: part.partName,
      brand: part.brand ?? '',
      description: part.description ?? '',
      unit: part.unit,
      purchasePrice: part.purchasePrice,
      minimumQuantity: part.minimumQuantity
    });
    this.dialogVisible.set(true);
  }

  closeDialog(): void {
    if (!this.isSaving()) this.dialogVisible.set(false);
  }

  savePart(): void {
    if (this.isSaving()) return;
    this.form.markAllAsTouched();
    const value = this.form.getRawValue();

    if (this.form.invalid || typeof value.purchasePrice !== 'number' ||
      typeof value.minimumQuantity !== 'number' || !Number.isFinite(value.purchasePrice) ||
      !Number.isFinite(value.minimumQuantity)) {
      this.resultDialog.show('Validation', 'Please complete the required fields and enter valid non-negative numbers.', 'error');
      return;
    }

    const dto: CreateSparePart = {
      partNumber: value.partNumber.trim(),
      partName: value.partName.trim(),
      brand: value.brand.trim() || null,
      description: value.description.trim() || null,
      unit: value.unit.trim(),
      purchasePrice: value.purchasePrice,
      minimumQuantity: value.minimumQuantity
    };
    const id = this.selectedPartId();
    this.isSaving.set(true);
    this.form.disable();
    const request = id === null
      ? this.partService.create(dto).pipe(map(() => undefined))
      : this.partService.update(id, dto);

    request.pipe(
      finalize(() => {
        this.isSaving.set(false);
        this.form.enable();
      }),
      takeUntilDestroyed(this.destroyRef)
    ).subscribe({
      next: () => {
        this.dialogVisible.set(false);
        this.resultDialog.show('Success', id === null ? 'Spare part added successfully.' : 'Spare part updated successfully.', 'success');
        this.loadParts();
      },
      error: (error: unknown) => {
        this.resultDialog.show('Error', this.getErrorMessage(error, 'Failed to save spare part.'), 'error');
      }
    });
  }

  confirmDelete(part: SparePart): void {
    if (this.deletingPartId() !== null || this.isSaving()) return;
    this.confirmDialog.show(
      () => this.deletePart(part.sparePartId),
      'Delete Spare Part',
      `Are you sure you want to delete "${part.partName}"? Spare parts with stock movements cannot be deleted.`
    );
  }

  private deletePart(id: number): void {
    if (this.deletingPartId() !== null || this.isSaving()) return;
    this.deletingPartId.set(id);
    this.partService.delete(id).pipe(
      finalize(() => this.deletingPartId.set(null)),
      takeUntilDestroyed(this.destroyRef)
    ).subscribe({
      next: () => {
        this.resultDialog.show('Success', 'Spare part deleted successfully.', 'success');
        this.loadParts();
      },
      error: (error: unknown) => {
        this.resultDialog.show('Error', this.getErrorMessage(error, 'Failed to delete spare part.'), 'error');
      }
    });
  }

  private getErrorMessage(error: unknown, fallback: string): string {
    if (!(error instanceof HttpErrorResponse)) return fallback;
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
