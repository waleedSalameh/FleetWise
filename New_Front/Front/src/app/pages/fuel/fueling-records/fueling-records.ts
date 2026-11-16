import { TranslatePipe } from '../../../shared/i18n/translate.pipe';

import { Component, OnInit, ChangeDetectorRef, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DialogModule } from 'primeng/dialog';

import {
  FuelRecord,
  FuelSourceType,
  CreateExternalFuelRecord,
  CreateInternalFuelRecord,
  UpdateFuelRecord
} from '../../../features/fuel/models/fuel-record';

import { FuelPurchase } from '../../../features/fuel/models/fuel-purchase';

import { FuelRecordService } from '../../../features/fuel/services/fuel-record';
import { FuelPurchaseService } from '../../../features/fuel/services/fuel-purchase';

import { Vehicle } from '../../../features/vehicles/models/vehicle';
import { VehicleService } from '../../../features/vehicles/services/vehicle';

import { ConfirmDialogService } from '../../../shared/services/confirm-dialog';
import { ResultDialogService } from '../../../shared/services/result-dialog';
import { LanguageService } from '../../../shared/i18n/language.service';
import { FuelOperationsService } from '../../../features/fuel/services/fuel-operations';
import { CreateFuelTransfer, FuelTransfer, FuelTransferDestinationType } from '../../../features/fuel/models/fuel-operations';

@Component({
  selector: 'app-fueling-records',
  standalone: true,
  imports: [TranslatePipe, CommonModule, FormsModule, DialogModule],
  templateUrl: './fueling-records.html',
  styleUrl: './fueling-records.scss'
})
export class FuelingRecordsComponent implements OnInit {

  private fuelRecordService = inject(FuelRecordService);
  private fuelPurchaseService = inject(FuelPurchaseService);
  private vehicleService = inject(VehicleService);
  private confirmDialog = inject(ConfirmDialogService);
  private resultDialog = inject(ResultDialogService);
  private readonly languageService = inject(LanguageService);
  private fuelOperations = inject(FuelOperationsService);
  private cdr = inject(ChangeDetectorRef);

  records: FuelRecord[] = [];
  vehicles: Vehicle[] = [];
  fuelPurchases: FuelPurchase[] = [];
  transfers: FuelTransfer[] = [];
  loading = false;
  transferLoading = false;
  transferVisible = false;
  transferSaving = false;
  FuelTransferDestinationType = FuelTransferDestinationType;
  filterVehicleId = 0;
  filterSource: 'all' | FuelSourceType = 'all';
  filterStart = '';
  filterEnd = '';
  transferSearch = '';
  transferForm: CreateFuelTransfer = { sourceVehicleId: 0, occurredAt: '', notes: null, destinations: [] };

  searchText = '';

  dialogVisible = false;
  isEditMode = false;
  isSaving = false;

  FuelSourceType = FuelSourceType;
  selectedSource: FuelSourceType = FuelSourceType.External;

  selectedRecordId: number | null = null;

  form = {
    vehicleId: 0,
    date: this.localDateTimeInput(),
    quantity: 0,
    pricePerLiter: 0,
    odometer: null as number | null,
    stationName: ''
  };

  selectedBatches: {
    fuelPurchaseId: number;
    quantity: number;
  }[] = [];

  private localDateTimeInput(date = new Date()): string {
    return new Date(date.getTime() - date.getTimezoneOffset() * 60000)
      .toISOString().slice(0, 16);
  }

  ngOnInit(): void {
    this.loadRecords();
    this.loadVehicles();
    this.loadFuelPurchases();
    this.loadTransfers();
  }

  loadRecords(): void {
    this.loading = true;
    this.fuelRecordService.getAll().subscribe({
      next: (data) => {
        this.records = data;
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.loading = false;
        this.resultDialog.show(
          this.languageService.text('Error'),
          this.languageService.text('Failed to load fueling records.'),
          'error'
        );
      }
    });
  }

  loadVehicles(): void {
    this.vehicleService.getAll().subscribe({
      next: (data) => {
        this.vehicles = data;
        this.cdr.detectChanges();
      },
      error: () => {
        this.resultDialog.show(
          this.languageService.text('Error'),
          this.languageService.text('Failed to load vehicles.'),
          'error'
        );
      }
    });
  }

  loadFuelPurchases(): void {
    this.fuelPurchaseService.getAll().subscribe({
      next: (data) => {
        this.fuelPurchases = data;
        this.cdr.detectChanges();
      },
      error: () => {
        this.resultDialog.show(
          this.languageService.text('Error'),
          this.languageService.text('Failed to load fuel inventory.'),
          'error'
        );
      }
    });
  }

  get filteredRecords(): FuelRecord[] {
    const search = this.searchText.trim().toLowerCase();

    return this.records.filter(record =>
      (record.plateNumber.toString().includes(search) || record.fuelRecordId.toString().includes(search) || (record.stationName ?? '').toLowerCase().includes(search)) &&
      (!this.filterVehicleId || record.vehicleId === this.filterVehicleId) &&
      (this.filterSource === 'all' || record.fuelSourceType === this.filterSource) &&
      (!this.filterStart || record.date.slice(0, 10) >= this.filterStart) &&
      (!this.filterEnd || record.date.slice(0, 10) <= this.filterEnd)
    );
  }

  get totalRecords(): number {
    return this.records.length;
  }

  get internalRecords(): number {
    return this.records.filter(
      x => x.fuelSourceType === FuelSourceType.Internal
    ).length;
  }

  get externalRecords(): number {
    return this.records.filter(
      x => x.fuelSourceType === FuelSourceType.External
    ).length;
  }

  get totalFuelQuantity(): number {
    return this.records.reduce(
      (sum, x) => sum + x.quantity,
      0
    );
  }

  get totalFuelCost(): number {
    return this.records.reduce(
      (sum, x) => sum + x.totalCost,
      0
    );
  }

  loadTransfers(): void {
    this.transferLoading = true;
    this.fuelOperations.getTransfers().subscribe({ next: data => { this.transfers = data; this.transferLoading = false; this.cdr.detectChanges(); }, error: e => { this.transferLoading = false; this.resultDialog.show(this.languageService.text('Error'), e.error?.message || this.languageService.text('Failed to load transfers.'), 'error'); } });
  }
  get filteredTransfers(): FuelTransfer[] { const q = this.transferSearch.trim().toLowerCase(); return this.transfers.filter(x => (String(x.id).includes(q) || String(x.sourcePlateNumber).includes(q) || (x.notes ?? '').toLowerCase().includes(q)) && (!this.filterVehicleId || x.sourceVehicleId === this.filterVehicleId || x.destinations.some(d => d.destinationVehicleId === this.filterVehicleId)) && (!this.filterStart || x.occurredAt.slice(0, 10) >= this.filterStart) && (!this.filterEnd || x.occurredAt.slice(0, 10) <= this.filterEnd)); }
  openTransfer(): void { this.transferForm = { sourceVehicleId: 0, occurredAt: this.localDateTimeInput(), notes: null, destinations: [{ destinationType: FuelTransferDestinationType.Vehicle, destinationVehicleId: null, quantity: 0 }] }; this.transferVisible = true; }
  addDestination(): void { this.transferForm.destinations = [...this.transferForm.destinations, { destinationType: FuelTransferDestinationType.Vehicle, destinationVehicleId: null, quantity: 0 }]; }
  removeDestination(index: number): void { this.transferForm.destinations = this.transferForm.destinations.filter((_, i) => i !== index); }
  get transferQuantity(): number { return this.transferForm.destinations.reduce((sum, x) => sum + Number(x.quantity || 0), 0); }
  transferTotalQuantity(transfer: FuelTransfer): number { return transfer.destinations.reduce((sum, x) => sum + x.quantity, 0); }
  transferTotalValue(transfer: FuelTransfer): number { return transfer.destinations.reduce((sum, x) => sum + x.totalCostJod, 0); }
  saveTransfer(): void {
    const f = this.transferForm;
    const vehicleIds = f.destinations.filter(x => x.destinationType === FuelTransferDestinationType.Vehicle).map(x => x.destinationVehicleId);
    if (!f.sourceVehicleId || !f.occurredAt || !f.destinations.length || f.destinations.some(x => x.quantity <= 0 || (x.destinationType === FuelTransferDestinationType.Vehicle && (!x.destinationVehicleId || x.destinationVehicleId === f.sourceVehicleId))) || new Set(vehicleIds).size !== vehicleIds.length || f.destinations.filter(x => x.destinationType === FuelTransferDestinationType.Inventory).length > 1) {
      this.resultDialog.show(this.languageService.text('Validation'), this.languageService.text('Check the source, date, destination vehicles and quantities. Destinations must be unique.'), 'error'); return;
    }
    this.transferSaving = true;
    this.fuelOperations.createTransfer({ ...f, occurredAt: new Date(f.occurredAt).toISOString(), destinations: f.destinations.map(x => ({ ...x, destinationVehicleId: x.destinationType === FuelTransferDestinationType.Inventory ? null : x.destinationVehicleId })) }).subscribe({ next: () => { this.transferSaving = false; this.transferVisible = false; this.resultDialog.show(this.languageService.text('Success'), this.languageService.text('Fuel transferred successfully.'), 'success'); this.loadTransfers(); this.loadFuelPurchases(); }, error: e => { this.transferSaving = false; this.resultDialog.show(this.languageService.text('Error'), e.error?.message || this.languageService.text('Fuel transfer failed.'), 'error'); } });
  }
  deleteTransfer(transfer: FuelTransfer): void {
    this.confirmDialog.show(
      () => this.fuelOperations.deleteTransfer(transfer.id).subscribe({
        next: () => {
          this.resultDialog.show(
            this.languageService.text('Success'),
            this.languageService.text('fuel.extra.019'),
            'success'
          );
          this.loadTransfers();
          this.loadFuelPurchases();
        },
        error: e => this.resultDialog.show(
          this.languageService.text('Error'),
          e.error?.message || this.languageService.text('fuel.extra.020'),
          'error'
        )
      }),
      this.languageService.text('fuel.extra.021'),
      this.languageService.text('fuel.confirm_reverse_transfer', { id: transfer.id })
    );
  }

  openAddDialog(): void {
    this.isEditMode = false;
    this.selectedRecordId = null;
    this.selectedSource = FuelSourceType.External;

    this.form = {
      vehicleId: 0,
      date: this.localDateTimeInput(),
      quantity: 0,
      pricePerLiter: 0,
      odometer: null,
      stationName: ''
    };

    this.selectedBatches = [
      { fuelPurchaseId: 0, quantity: 0 }
    ];

    this.dialogVisible = true;
  }

  openEditDialog(record: FuelRecord): void {
    if (record.fuelSourceType === FuelSourceType.Internal) return;

    this.isEditMode = true;
    this.selectedRecordId = record.fuelRecordId;
    this.selectedSource = FuelSourceType.External;

    this.form = {
      vehicleId: record.vehicleId,
      date: this.localDateTimeInput(new Date(record.date)),
      quantity: record.quantity,
      pricePerLiter: record.pricePerLiter,
      odometer: record.odometer,
      stationName: record.stationName ?? ''
    };

    this.dialogVisible = true;
  }

  onSourceChange(): void {
    this.selectedBatches = [
      { fuelPurchaseId: 0, quantity: 0 }
    ];

    this.form.pricePerLiter = 0;
    this.form.stationName = '';
  }

  addBatchRow(): void {
    this.selectedBatches.push({
      fuelPurchaseId: 0,
      quantity: 0
    });
  }

  removeBatchRow(index: number): void {
    this.selectedBatches.splice(index, 1);
  }

  get availableBatches(): FuelPurchase[] {
    return this.fuelPurchases.filter(
      x => x.status === 0 && x.remainingQuantity > 0
    );
  }

  getBatchRemaining(id: number): number {
    const batch = this.fuelPurchases.find(
      x => x.fuelPurchaseId === id
    );

    return batch?.remainingQuantity ?? 0;
  }

  getBatchPrice(id: number): number {
    const batch = this.fuelPurchases.find(
      x => x.fuelPurchaseId === id
    );

    return batch?.pricePerLiter ?? 0;
  }

  getBatchCost(batchId: number, quantity: number): number {
    return quantity * this.getBatchPrice(batchId);
  }

  get internalTotalQuantity(): number {
    return this.selectedBatches.reduce(
      (sum, x) => sum + Number(x.quantity || 0),
      0
    );
  }

  get internalTotalCost(): number {
    return this.selectedBatches.reduce(
      (sum, x) =>
        sum + this.getBatchCost(
          x.fuelPurchaseId,
          Number(x.quantity || 0)
        ),
      0
    );
  }

  saveRecord(): void {

    if (!this.form.vehicleId || !this.form.date) {
      this.resultDialog.show(
        this.languageService.text('Validation'),
        this.languageService.text('Please select a vehicle and date.'),
        'error'
      );
      return;
    }

    if (
      this.form.odometer === null ||
      this.form.odometer === undefined ||
      this.form.odometer === ('' as any)
    ) {
      this.resultDialog.show(
        this.languageService.text('Validation'),
        this.languageService.text('Please enter the odometer reading.'),
        'error'
      );
      return;
    }

    const odometer = Number(this.form.odometer);

    if (
      !Number.isInteger(odometer) ||
      odometer < 0 ||
      odometer > 2147483647
    ) {
      this.resultDialog.show(
        this.languageService.text('Validation'),
        this.languageService.text('Please enter a valid whole odometer number.'),
        'error'
      );
      return;
    }

    if (this.selectedSource === FuelSourceType.External) {
      this.saveExternalRecord();
    } else {
      this.saveInternalRecord();
    }
  }

  private saveExternalRecord(): void {

    if (
      !this.form.quantity ||
      !this.form.pricePerLiter ||
      !this.form.stationName.trim()
    ) {
      this.resultDialog.show(
        this.languageService.text('Validation'),
        this.languageService.text('Please complete all external fueling fields.'),
        'error'
      );
      return;
    }

    const dto: CreateExternalFuelRecord = {
      vehicleId: this.form.vehicleId,
      date: new Date(this.form.date).toISOString(),
      quantity: this.form.quantity,
      pricePerLiter: this.form.pricePerLiter,
      odometer: Number(this.form.odometer),
      stationName: this.form.stationName.trim()
    };

    this.isSaving = true;

    if (this.isEditMode && this.selectedRecordId !== null) {

      const updateDto: UpdateFuelRecord = dto;

      this.fuelRecordService.update(
        this.selectedRecordId,
        updateDto
      ).subscribe({
        next: () => this.finishSave(
          this.languageService.text('Fuel record updated successfully.')
        ),
        error: (err) => this.handleSaveError(err)
      });

    } else {

      this.fuelRecordService.createExternal(dto).subscribe({
        next: () => this.finishSave(
          this.languageService.text('External fueling record added successfully.')
        ),
        error: (err) => this.handleSaveError(err)
      });

    }
  }

  private saveInternalRecord(): void {

    if (!this.form.quantity || this.form.quantity <= 0) {
      this.resultDialog.show(
        this.languageService.text('Validation'),
        this.languageService.text('Please enter a valid fuel quantity.'),
        'error'
      );
      return;
    }

    if (this.selectedBatches.length === 0) {
      this.resultDialog.show(
        this.languageService.text('Validation'),
        this.languageService.text('Please select at least one fuel batch.'),
        'error'
      );
      return;
    }

    const validBatches = this.selectedBatches.filter(
      x => x.fuelPurchaseId > 0 && x.quantity > 0
    );

    if (validBatches.length !== this.selectedBatches.length) {
      this.resultDialog.show(
        this.languageService.text('Validation'),
        this.languageService.text('Please select a batch and enter its quantity for every row.'),
        'error'
      );
      return;
    }

    if (
      this.internalTotalQuantity !== Number(this.form.quantity)
    ) {
      this.resultDialog.show(
        this.languageService.text('Validation'),
        this.languageService.text('The sum of batch quantities must equal the total fuel quantity.'),
        'error'
      );
      return;
    }

    const duplicateBatches = validBatches.some(
      (batch, index) =>
        validBatches.findIndex(
          x => x.fuelPurchaseId === batch.fuelPurchaseId
        ) !== index
    );

    if (duplicateBatches) {
      this.resultDialog.show(
        this.languageService.text('Validation'),
        this.languageService.text('You cannot select the same batch more than once.'),
        'error'
      );
      return;
    }

    if (validBatches.some(batch => batch.quantity > this.getBatchRemaining(batch.fuelPurchaseId))) {
      this.resultDialog.show(this.languageService.text('Validation'), this.languageService.text('A selected batch has insufficient available fuel.'), 'error');
      return;
    }

    const dto: CreateInternalFuelRecord = {
      vehicleId: this.form.vehicleId,
      date: new Date(this.form.date).toISOString(),
      quantity: this.form.quantity,
      odometer: Number(this.form.odometer),
      batches: validBatches
    };

    this.isSaving = true;

    this.fuelRecordService.createInternal(dto).subscribe({
      next: () => {
        this.finishSave(
          this.languageService.text('Internal fueling record added successfully.')
        );

        this.loadFuelPurchases();
      },
      error: (err) => this.handleSaveError(err)
    });
  }

  private finishSave(message: string): void {
    this.isSaving = false;
    this.dialogVisible = false;

    this.resultDialog.show(
      this.languageService.text('Success'),
      message,
      'success'
    );

    this.loadRecords();
    this.cdr.detectChanges();
  }

  private handleSaveError(err: any): void {
    this.isSaving = false;

    const message =
      err.error?.message ||
      this.languageService.text('An error occurred while saving the fueling record.');

    this.resultDialog.show(
      this.languageService.text('Error'),
      message,
      'error'
    );

    this.cdr.detectChanges();
  }

  confirmDelete(record: FuelRecord): void {
    this.confirmDialog.show(
      () => this.deleteRecord(record.fuelRecordId),
      this.languageService.text('Delete Fuel Record'),
      this.languageService.text('Are you sure you want to delete this fueling record?')
    );
  }

  private deleteRecord(id: number): void {
    this.fuelRecordService.delete(id).subscribe({
      next: () => {
        this.resultDialog.show(
          this.languageService.text('Success'),
          this.languageService.text('Fuel record deleted successfully.'),
          'success'
        );

        this.loadRecords();
        this.loadFuelPurchases();
      },
      error: (err) => {
        this.resultDialog.show(
          this.languageService.text('Error'),
          err.error?.message || this.languageService.text('Failed to delete fuel record.'),
          'error'
        );
      }
    });
  }

  getSourceLabel(source: FuelSourceType): string {
    return source === FuelSourceType.Internal
      ? 'Internal'
      : 'External';
  }

  getSourceClass(source: FuelSourceType): string {
    return source === FuelSourceType.Internal
      ? 'internal'
      : 'external';
  }

  formatDate(date: string): string {
    return new Date(date).toLocaleDateString(this.languageService.language() === 'ar' ? 'ar-JO' : 'en-GB');
  }

  closeDialog(): void {
    if (!this.isSaving) {
      this.dialogVisible = false;
    }
  }
  onOdometerChange(value: number | string | null): void {
    this.form.odometer =
      value === null || value === ''
        ? null
        : Number(value);
  }
}

