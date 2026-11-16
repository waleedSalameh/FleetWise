import { TranslatePipe } from '../../../shared/i18n/translate.pipe';
import { Component, DestroyRef, OnInit, computed, inject, signal } from '@angular/core';
import { DecimalPipe } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { DialogModule } from 'primeng/dialog';
import { RouterLink } from '@angular/router';
import { Subject, catchError, finalize, from, map, mergeMap, of, switchMap, toArray } from 'rxjs';

import { CreateOil, Oil } from '../../../features/maintenance/models/oil';
import { OilService } from '../../../features/maintenance/services/oil';
import { OilMovementService } from '../../../features/maintenance/services/oil-movement';
import { ConfirmDialogService } from '../../../shared/services/confirm-dialog';
import { ResultDialogService } from '../../../shared/services/result-dialog';

type StockStatus = 'loading' | 'unavailable' | 'out-of-stock' | 'low-stock' | 'in-stock';

@Component({
  selector: 'app-oil-inventory',
  imports: [TranslatePipe, DecimalPipe, ReactiveFormsModule, DialogModule, RouterLink],
  templateUrl: './oil-inventory.html',
  styleUrl: './oil-inventory.scss'
})
export class OilInventoryComponent implements OnInit {
  private readonly oilService = inject(OilService);
  private readonly oilMovementService = inject(OilMovementService);
  private readonly confirmDialog = inject(ConfirmDialogService);
  private readonly resultDialog = inject(ResultDialogService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly reload = new Subject<void>();

  readonly oils = signal<Oil[]>([]);
  readonly stocks = signal<ReadonlyMap<number, number | null>>(new Map());
  readonly searchText = signal('');
  readonly isLoading = signal(false);
  readonly isLoadingStock = signal(false);
  readonly loadFailed = signal(false);
  readonly dialogVisible = signal(false);
  readonly isSaving = signal(false);
  readonly selectedOilId = signal<number | null>(null);
  readonly deletingOilId = signal<number | null>(null);
  readonly isEditMode = computed(() => this.selectedOilId() !== null);

  readonly filteredOils = computed(() => {
    const search = this.searchText().trim().toLowerCase();
    return this.oils().filter(oil => !search ||
      [oil.oilName, oil.brand, oil.viscosity].some(value =>
        (value ?? '').toLowerCase().includes(search)
      )
    );
  });

  readonly totalOils = computed(() => this.oils().length);
  readonly totalBrands = computed(() => new Set(
    this.oils().map(oil => (oil.brand ?? '').trim().toLowerCase()).filter(Boolean)
  ).size);
  readonly lowStockOils = computed(() => this.oils().filter(oil =>
    this.getStockStatus(oil) === 'low-stock'
  ).length);
  readonly outOfStockOils = computed(() => this.oils().filter(oil =>
    this.getStockStatus(oil) === 'out-of-stock'
  ).length);
  readonly hasUnavailableStock = computed(() =>
    [...this.stocks().values()].some(stock => stock === null)
  );

  readonly form = new FormGroup({
    oilName: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.maxLength(150), Validators.pattern(/\S/)]
    }),
    brand: new FormControl('', { nonNullable: true, validators: [Validators.maxLength(100)] }),
    viscosity: new FormControl('', { nonNullable: true, validators: [Validators.maxLength(50)] }),
    unit: new FormControl('Liter', {
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
        this.oils.set([]);
        this.stocks.set(new Map());

        return this.oilService.getAll().pipe(
          switchMap(oils => {
            this.oils.set(oils);
            this.isLoading.set(false);
            this.isLoadingStock.set(oils.length > 0);

            return from(oils).pipe(
              mergeMap(oil => this.oilMovementService.getCurrentStock(oil.oilId).pipe(
                map(stock => ({ oilId: oil.oilId, stock: Number.isFinite(stock) ? stock : null })),
                catchError(() => of({ oilId: oil.oilId, stock: null }))
              ), 6),
              map(({ oilId, stock }) => {
                this.stocks.update(current => new Map(current).set(oilId, stock));
              }),
              toArray()
            );
          }),
          catchError((error: unknown) => {
            this.loadFailed.set(true);
            this.resultDialog.show('Error', this.getErrorMessage(error, 'Failed to load oil inventory.'), 'error');
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

    this.loadOils();
  }

  loadOils(): void {
    this.reload.next();
  }

  getStock(oilId: number): number | null {
    return this.stocks().get(oilId) ?? null;
  }

  getStockStatus(oil: Oil): StockStatus {
    const stocks = this.stocks();
    if (!stocks.has(oil.oilId)) return 'loading';
    const stock = stocks.get(oil.oilId);
    if (stock === null || stock === undefined) return 'unavailable';
    if (stock <= 0) return 'out-of-stock';
    return stock < oil.minimumQuantity ? 'low-stock' : 'in-stock';
  }

  getStockStatusLabel(oil: Oil): string {
    const labels: Record<StockStatus, string> = {
      loading: 'Loading...',
      unavailable: 'Unavailable',
      'out-of-stock': 'Out of Stock',
      'low-stock': 'Low Stock',
      'in-stock': 'In Stock'
    };
    return labels[this.getStockStatus(oil)];
  }

  openAddDialog(): void {
    if (this.isSaving()) return;
    this.selectedOilId.set(null);
    this.form.reset({ oilName: '', brand: '', viscosity: '', unit: 'Liter', purchasePrice: 0, minimumQuantity: 0 });
    this.dialogVisible.set(true);
  }

  openEditDialog(oil: Oil): void {
    if (this.isSaving()) return;
    this.selectedOilId.set(oil.oilId);
    this.form.reset({
      oilName: oil.oilName,
      brand: oil.brand ?? '',
      viscosity: oil.viscosity ?? '',
      unit: oil.unit,
      purchasePrice: oil.purchasePrice,
      minimumQuantity: oil.minimumQuantity
    });
    this.dialogVisible.set(true);
  }

  closeDialog(): void {
    if (!this.isSaving()) this.dialogVisible.set(false);
  }

  saveOil(): void {
    if (this.isSaving()) return;
    this.form.markAllAsTouched();
    const value = this.form.getRawValue();

    if (this.form.invalid || typeof value.purchasePrice !== 'number' ||
        typeof value.minimumQuantity !== 'number' || !Number.isFinite(value.purchasePrice) ||
        !Number.isFinite(value.minimumQuantity)) {
      this.resultDialog.show('Validation', 'Please complete the required fields and enter valid non-negative numbers.', 'error');
      return;
    }

    const dto: CreateOil = {
      oilName: value.oilName.trim(),
      brand: value.brand.trim() || null,
      viscosity: value.viscosity.trim() || null,
      unit: value.unit.trim(),
      purchasePrice: value.purchasePrice,
      minimumQuantity: value.minimumQuantity
    };
    const id = this.selectedOilId();
    this.isSaving.set(true);
    this.form.disable();
    const request = id === null
      ? this.oilService.create(dto).pipe(map(() => undefined))
      : this.oilService.update(id, dto);

    request.pipe(
      finalize(() => {
        this.isSaving.set(false);
        this.form.enable();
      }),
      takeUntilDestroyed(this.destroyRef)
    ).subscribe({
      next: () => {
        this.dialogVisible.set(false);
        this.resultDialog.show('Success', id === null ? 'Oil added successfully.' : 'Oil updated successfully.', 'success');
        this.loadOils();
      },
      error: (error: unknown) => {
        this.resultDialog.show('Error', this.getErrorMessage(error, 'Failed to save oil.'), 'error');
      }
    });
  }

  confirmDelete(oil: Oil): void {
    if (this.deletingOilId() !== null || this.isSaving()) return;
    this.confirmDialog.show(
      () => this.deleteOil(oil.oilId),
      'Delete Oil',
      `Are you sure you want to delete "${oil.oilName}"? Oils with stock movements cannot be deleted.`
    );
  }

  private deleteOil(id: number): void {
    if (this.deletingOilId() !== null || this.isSaving()) return;
    this.deletingOilId.set(id);
    this.oilService.delete(id).pipe(
      finalize(() => this.deletingOilId.set(null)),
      takeUntilDestroyed(this.destroyRef)
    ).subscribe({
      next: () => {
        this.resultDialog.show('Success', 'Oil deleted successfully.', 'success');
        this.loadOils();
      },
      error: (error: unknown) => {
        this.resultDialog.show('Error', this.getErrorMessage(error, 'Failed to delete oil.'), 'error');
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
