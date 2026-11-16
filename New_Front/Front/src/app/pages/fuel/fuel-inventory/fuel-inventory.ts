import { LanguageService } from '../../../shared/i18n/language.service';
import {
  Component,
  OnInit,
  ChangeDetectorRef,
  inject
} from '@angular/core';

import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DialogModule } from 'primeng/dialog';

import {
  FuelPurchase,
  FuelPurchaseStatus,
  CreateFuelPurchase,
  FuelInventoryBatchOrigin,
  FuelInventorySummary
} from '../../../features/fuel/models/fuel-purchase';

import { TranslatePipe } from '../../../shared/i18n/translate.pipe';
import { FuelPurchaseService } from '../../../features/fuel/services/fuel-purchase';
import { ResultDialogService } from '../../../shared/services/result-dialog';
import { FuelOperationsService } from '../../../features/fuel/services/fuel-operations';
import { FuelTransaction } from '../../../features/fuel/models/fuel-operations';

@Component({
  imports: [
    CommonModule,
    FormsModule,
    DialogModule,
    TranslatePipe,
    FormsModule,
    DialogModule
  ],
  templateUrl: './fuel-inventory.html',
  styleUrl: './fuel-inventory.scss'
})
export class FuelInventoryComponent implements OnInit {

  private fuelPurchaseService = inject(FuelPurchaseService);
  private fuelOperations = inject(FuelOperationsService);
  private resultDialog = inject(ResultDialogService);
  private cdr = inject(ChangeDetectorRef);
  private readonly languageService = inject(LanguageService);

  displayDate(value: string): string {
    const date = new Date(value);
    return Number.isNaN(date.getTime()) ? value : new Intl.DateTimeFormat(this.languageService.language() === 'ar' ? 'ar-JO' : 'en-GB', { day: '2-digit', month: 'short', year: 'numeric' }).format(date);
  }

  purchases: FuelPurchase[] = [];
  summary: FuelInventorySummary | null = null;
  movements: FuelTransaction[] = [];
  loading = false;
  FuelInventoryBatchOrigin = FuelInventoryBatchOrigin;

  searchText = '';
  selectedStatus: 'all' | FuelPurchaseStatus = 'all';

  dialogVisible = false;
  isSaving = false;

  purchaseDate = '';
  quantity: number | null = null;
  pricePerLiter: number | null = null;
  supplierName = '';
  invoiceNumber = '';

  ngOnInit(): void {
    this.loadPurchases();
  }

  // Load Fuel Purchases
  loadPurchases(): void {
    this.loading = true;
    this.fuelPurchaseService.getAll().subscribe({
      next: (data) => {
        this.purchases = data;
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.loading = false;
        this.resultDialog.show(
          this.languageService.text('Error'),
          this.languageService.text('Failed to load fuel inventory.'),
          'error'
        );
      }
    });
    this.fuelPurchaseService.getSummary().subscribe({ next: data => { this.summary = data; this.cdr.detectChanges(); }, error: () => this.resultDialog.show(this.languageService.text('Error'), this.languageService.text('Failed to load inventory summary.'), 'error') });
    this.fuelOperations.getTransactions('2000-01-01T00:00:00', '2100-01-01T00:00:00').subscribe({ next: data => { this.movements = data.filter(x => ['InventoryPurchase', 'VehicleFuelReturn', 'InventoryFuelReturnReversal', 'InternalFueling'].includes(x.transactionType)); this.cdr.detectChanges(); }, error: () => this.resultDialog.show(this.languageService.text('Error'), this.languageService.text('Failed to load inventory movements.'), 'error') });
  }

  // Search and Filter
  get filteredPurchases(): FuelPurchase[] {
    return this.purchases.filter(purchase => {

      const search = this.searchText.toLowerCase();

      const matchesSearch =
        purchase.fuelPurchaseId.toString().includes(search) ||
        String(purchase.sourceVehicleId ?? '').includes(search) ||
        (purchase.supplierName ?? '').toLowerCase().includes(search) ||
        (purchase.invoiceNumber ?? '').toLowerCase().includes(search);

      const matchesStatus =
        this.selectedStatus === 'all' ||
        purchase.status === this.selectedStatus;

      return matchesSearch && matchesStatus;
    });
  }

  // Summary
  get totalPurchases(): number {
    return this.purchases.length;
  }

  get totalQuantity(): number {
    return this.purchases.reduce(
      (total, purchase) => total + purchase.quantity,
      0
    );
  }

  get remainingQuantity(): number {
    return this.purchases.reduce(
      (total, purchase) => total + purchase.remainingQuantity,
      0
    );
  }

  get consumedQuantity(): number {
    return this.totalQuantity - this.remainingQuantity;
  }

  get inUseCount(): number {
    return this.purchases.filter(
      purchase => purchase.status === FuelPurchaseStatus.InUse
    ).length;
  }

  get completedCount(): number {
    return this.purchases.filter(
      purchase => purchase.status === FuelPurchaseStatus.Completed
    ).length;
  }

  // Open Add Dialog
  openAddDialog(): void {
    this.resetForm();
    this.dialogVisible = true;
  }

  // Reset Form
  resetForm(): void {
    this.purchaseDate = new Date().toISOString().split('T')[0];
    this.quantity = null;
    this.pricePerLiter = null;
    this.supplierName = '';
    this.invoiceNumber = '';
  }

  // Save Purchase
  savePurchase(): void {

    if (
      !this.purchaseDate ||
      this.quantity === null ||
      this.quantity <= 0 ||
      this.pricePerLiter === null ||
      this.pricePerLiter < 0
    ) {
      this.resultDialog.show(
        this.languageService.text('Validation Error'),
        this.languageService.text('Please enter valid purchase information.'),
        'error'
      );
      return;
    }

    const dto: CreateFuelPurchase = {
      purchaseDate: this.purchaseDate,
      quantity: this.quantity,
      pricePerLiter: this.pricePerLiter,
      supplierName: this.supplierName.trim() || null,
      invoiceNumber: this.invoiceNumber.trim() || null
    };

    this.isSaving = true;

    this.fuelPurchaseService.create(dto).subscribe({
      next: () => {
        this.isSaving = false;
        this.dialogVisible = false;

        this.resultDialog.show(
          this.languageService.text('Success'),
          this.languageService.text('Fuel purchase added successfully.'),
          'success'
        );

        this.loadPurchases();
        this.cdr.detectChanges();
      },
      error: (error) => {
        this.isSaving = false;

        this.resultDialog.show(
          this.languageService.text('Error'),
          error.error?.message || this.languageService.text('Failed to add fuel purchase.'),
          'error'
        );

        this.cdr.detectChanges();
      }
    });
  }

  // Status
  getStatusLabel(status: FuelPurchaseStatus): string {
    return status === FuelPurchaseStatus.InUse
      ? this.languageService.text('In Use')
      : 'Completed';
  }

  getBatchOriginLabel(purchase: FuelPurchase): string {
    return purchase.batchOrigin === FuelInventoryBatchOrigin.VehicleReturn ? this.languageService.text('Vehicle return') : 'Purchase';
  }

  getBatchSource(purchase: FuelPurchase): string {
    return purchase.batchOrigin === FuelInventoryBatchOrigin.VehicleReturn
      ? `${this.languageService.text('Vehicle')} #${purchase.sourceVehicleId ?? '—'}`
      : purchase.supplierName || '—';
  }

  displayMovementLocation(value: string): string {
    return value.startsWith('Vehicle ')
      ? `${this.languageService.text('Vehicle')} ${value.slice(8)}`
      : this.languageService.text(value);
  }

  getStatusClass(status: FuelPurchaseStatus): string {
    return status === FuelPurchaseStatus.InUse
      ? 'in-use'
      : 'completed';
  }

  // Close Dialog
  closeDialog(): void {
    this.dialogVisible = false;
  }
}
