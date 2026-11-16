import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { TranslatePipe } from '../../../shared/i18n/translate.pipe';
import { LanguageService } from '../../../shared/i18n/language.service';
import { DecimalPipe, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DialogModule } from 'primeng/dialog';
import { forkJoin } from 'rxjs';
import { Vehicle } from '../../../features/vehicles/models/vehicle';
import { VehicleService } from '../../../features/vehicles/services/vehicle';
import { ConfirmDialogService } from '../../../shared/services/confirm-dialog';
import { ResultDialogService } from '../../../shared/services/result-dialog';
import { FuelOperationsService } from '../../../features/fuel/services/fuel-operations';
import { CreateFuelMeasurement, FuelApprovalStatus, FuelMeasurement, FuelMeasurementType, FuelTransaction, MonthlyFuelReport } from '../../../features/fuel/models/fuel-operations';

@Component({
  imports: [FormsModule, DialogModule, DecimalPipe, TranslatePipe],
  templateUrl: './monthly-fuel-reports.html',
  styleUrl: './monthly-fuel-reports.scss'
})
export class MonthlyFuelReportsComponent implements OnInit {
  private readonly api = inject(FuelOperationsService);
  private readonly vehicleApi = inject(VehicleService);
  private readonly result = inject(ResultDialogService);
  private readonly language = inject(LanguageService);
  private readonly confirm = inject(ConfirmDialogService);
  readonly reports = signal<MonthlyFuelReport[]>([]);
  readonly transactions = signal<FuelTransaction[]>([]);
  readonly vehicles = signal<Vehicle[]>([]);
  readonly measurements = signal<FuelMeasurement[]>([]);
  readonly loading = signal(false);
  readonly saving = signal(false);
  readonly totalConsumed = computed(() => this.reports().filter(x => x.isComplete).reduce((a, x) => a + x.actualConsumptionLiters, 0));
  readonly totalCost = computed(() => this.reports().filter(x => x.isComplete).reduce((a, x) => a + x.actualConsumptionCostJod, 0));
  readonly fuelingCount = computed(() => this.reports().reduce((a, x) => a + x.totalFuelingCount, 0));
  readonly transferCount = computed(() => this.reports().reduce((a, x) => a + x.totalTransferCount, 0));
  readonly FuelMeasurementType = FuelMeasurementType;
  readonly FuelApprovalStatus = FuelApprovalStatus;
  text(value: string): string { return this.language.text(value); }
  displayDate(value: string): string {
    const date = new Date(value);
    return Number.isNaN(date.getTime()) ? value : new Intl.DateTimeFormat(this.language.language() === 'ar' ? 'ar-JO' : 'en-GB', { dateStyle: 'medium', timeStyle: 'short' }).format(date);
  }
  measurementTypeLabel(type: FuelMeasurementType): string { return this.text(type === FuelMeasurementType.Opening ? 'Opening' : type === FuelMeasurementType.Closing ? 'Closing' : 'Correction'); }
  transactionTypeLabel(type: string): string { return this.text(type); }
  displayLocation(value: string): string { return value.startsWith('Vehicle ') ? `${this.text('Vehicle')} ${value.slice(8)}` : this.text(value); }
  vehicleId = 0;
  startDate = '';
  endDate = '';
  searchText = '';
  detail: MonthlyFuelReport | null = null;
  detailVisible = false;
  measurementVisible = false;
  measurementVehicleId = 0;
  approvedBy = '';
  measurement: CreateFuelMeasurement = { vehicleId: 0, measuredAt: '', measuredQuantity: 0, measuredValueJod: null, measurementType: FuelMeasurementType.Opening, notes: null };

  ngOnInit(): void {
    this.resetFilters();
    this.vehicleApi.getAll().subscribe({ next: x => this.vehicles.set(x.filter(v => !v.isDeleted)), error: () => this.result.show(this.text('Error'), this.text('Failed to load vehicles.'), 'error') });
  }
  resetFilters(): void {
    const now = new Date();
    this.vehicleId = 0;
    this.startDate = `${now.getFullYear()}-${String(now.getMonth() + 1).padStart(2, '0')}-01`;
    this.endDate = `${now.getFullYear()}-${String(now.getMonth() + 1).padStart(2, '0')}-${String(new Date(now.getFullYear(), now.getMonth() + 1, 0).getDate()).padStart(2, '0')}`;
    this.applyFilters();
  }
  applyFilters(): void {
    if (!this.startDate || !this.endDate || this.endDate < this.startDate) { this.result.show(this.text('Validation'), this.text('Choose a valid date range.'), 'error'); return; }
    this.loading.set(true);
    const vehicle = this.vehicleId || undefined;
    forkJoin({ reports: this.api.getMonthly(this.startDate, this.endDate, vehicle), transactions: this.api.getTransactions(`${this.startDate}T00:00:00`, `${this.endDate}T23:59:59`, vehicle) }).subscribe({
      next: ({ reports, transactions }) => { this.reports.set(reports); this.transactions.set(transactions); this.loading.set(false); },
      error: e => { this.loading.set(false); this.result.show(this.text('Error'), e.error?.message || this.text('Failed to load fuel reports.'), 'error'); }
    });
  }
  get filteredReports(): MonthlyFuelReport[] { const q = this.searchText.trim(); return this.reports().filter(x => String(x.plateNumber).includes(q)); }
  openDetail(report: MonthlyFuelReport): void { this.detail = report; this.detailVisible = true; }
  get detailTransactions(): FuelTransaction[] {
    return this.detail
      ? this.transactions().filter(x => x.relatedVehicleId === this.detail!.vehicleId)
      : [];
  }
  openMeasurement(vehicleId = this.vehicleId): void {
    this.measurementVehicleId = vehicleId || this.vehicles()[0]?.vehicleId || 0;
    this.measurement = { vehicleId: this.measurementVehicleId, measuredAt: new Date().toISOString().slice(0, 16), measuredQuantity: 0, measuredValueJod: null, measurementType: FuelMeasurementType.Opening, notes: null };
    this.measurementVisible = true;
    this.loadMeasurements();
  }
  loadMeasurements(): void {
    if (!this.measurementVehicleId) { this.measurements.set([]); return; }
    this.measurement.vehicleId = this.measurementVehicleId;
    this.api.getMeasurements(this.measurementVehicleId).subscribe({ next: x => this.measurements.set(x), error: e => this.result.show(this.text('Error'), e.error?.message || this.text('Failed to load measurements.'), 'error') });
  }
  get selectedVehicle(): Vehicle | undefined { return this.vehicles().find(x => x.vehicleId === this.measurementVehicleId); }
  get hasOpening(): boolean { return this.measurements().some(x => x.measurementType === FuelMeasurementType.Opening && x.status === FuelApprovalStatus.Approved); }
  saveMeasurement(): void {
    const m = this.measurement;
    if (!m.vehicleId || !m.measuredAt || m.measuredQuantity < 0 || m.measuredQuantity > (this.selectedVehicle?.fuelTankCapacity ?? 0) || (m.measurementType === FuelMeasurementType.Opening && (this.hasOpening || m.measuredValueJod === null || m.measuredValueJod < 0))) {
      this.result.show(this.text('Validation'), this.text('Check the vehicle, date, measured quantity and opening value.'), 'error'); return;
    }
    this.saving.set(true);
    this.api.createMeasurement({ ...m, measuredAt: new Date(m.measuredAt).toISOString(), measuredValueJod: m.measurementType === FuelMeasurementType.Opening ? m.measuredValueJod : null }).subscribe({
      next: () => { this.saving.set(false); this.result.show(this.text('Success'), this.text('Measurement saved as draft.'), 'success'); this.loadMeasurements(); },
      error: e => { this.saving.set(false); this.result.show(this.text('Error'), e.error?.message || this.text('Failed to save measurement.'), 'error'); }
    });
  }
  approve(m: FuelMeasurement): void {
    if (!this.approvedBy.trim()) { this.result.show(this.text('Validation'), this.text('Enter the approver name.'), 'error'); return; }
    this.confirm.show(() => {
      this.saving.set(true);
      this.api.approveMeasurement(m.vehicleId, m.id, this.approvedBy.trim()).subscribe({
        next: () => { this.saving.set(false); this.result.show(this.text('Success'), this.text('Measurement approved.'), 'success'); this.loadMeasurements(); this.applyFilters(); },
        error: e => { this.saving.set(false); this.result.show(this.text('Error'), e.error?.message || this.text('Approval failed.'), 'error'); }
      });
    }, this.text('Approve Fuel Measurement'), this.text('Approve this measurement and its accounting effect?'), {
      actionLabel: 'fuel.ui.069',
      variant: 'primary'
    });
  }
}
