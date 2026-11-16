import { TranslatePipe } from '../../../shared/i18n/translate.pipe';
import {
  Component,
  OnInit,
  ChangeDetectorRef,
  inject
} from '@angular/core';

import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DialogModule } from 'primeng/dialog';
import { ActivatedRoute } from '@angular/router';
import { forkJoin, of, catchError } from 'rxjs';

import {
  MaintenanceRecord,
  MaintenanceLocation,
  MaintenanceStatus,
  CreateMaintenanceRecord,
  UpdateMaintenanceRecord
  , OilConsumption, SparePartConsumption
} from '../../../features/maintenance/models/maintenance-record';

import { MaintenanceRecordService } from '../../../features/maintenance/services/maintenance-record';

import { Vehicle } from '../../../features/vehicles/models/vehicle';
import { VehicleService } from '../../../features/vehicles/services/vehicle';
import { Oil } from '../../../features/maintenance/models/oil';
import { SparePart } from '../../../features/maintenance/models/spare-part';
import { OilService } from '../../../features/maintenance/services/oil';
import { SparePartService } from '../../../features/maintenance/services/spare-part';
import { OilMovementService } from '../../../features/maintenance/services/oil-movement';
import { SparePartMovementService } from '../../../features/maintenance/services/spare-part-movement';

import { ConfirmDialogService } from '../../../shared/services/confirm-dialog';
import { ResultDialogService } from '../../../shared/services/result-dialog';

@Component({
  selector: 'app-maintenance-records',
  standalone: true,
  imports: [TranslatePipe, 
    CommonModule,
    FormsModule,
    DialogModule
  ],
  templateUrl: './maintenance-records.html',
  styleUrl: './maintenance-records.scss'
})
export class MaintenanceRecords implements OnInit {

  private maintenanceService = inject(MaintenanceRecordService);
  private vehicleService = inject(VehicleService);
  private route = inject(ActivatedRoute);
  private oilService = inject(OilService);
  private partService = inject(SparePartService);
  private oilMovementService = inject(OilMovementService);
  private partMovementService = inject(SparePartMovementService);

  private confirmDialog = inject(ConfirmDialogService);
  private resultDialog = inject(ResultDialogService);

  private cdr = inject(ChangeDetectorRef);

  records: MaintenanceRecord[] = [];
  vehicles: Vehicle[] = [];
  oils: Oil[] = [];
  spareParts: SparePart[] = [];
  oilConsumption: OilConsumption[] = [];
  partConsumption: SparePartConsumption[] = [];
  originalOilConsumption: OilConsumption[] = [];
  originalPartConsumption: SparePartConsumption[] = [];
  oilStocks = new Map<number, number>();
  partStocks = new Map<number, number>();
  inventoryLoading = false;
  inventoryFailed = false;
  editLoading = false;

  searchText = '';

  dialogVisible = false;
  isEditMode = false;
  isSaving = false;

  MaintenanceLocation = MaintenanceLocation;
  MaintenanceStatus = MaintenanceStatus;

  selectedRecordId: number | null = null;

  form = {
    vehicleId: 0,
    maintenanceType: '',
    maintenanceLocation: MaintenanceLocation.Internal,
    maintenanceDate: new Date().toISOString().substring(0, 10),
    odometer: null as number | null,
    description: '',
    workshopName: '',
    technicianName: '',
    laborCost: 0,
    status: MaintenanceStatus.Scheduled
  };

  ngOnInit(): void {
    const recordId = this.route.snapshot.queryParamMap.get('maintenanceRecordId');
    if (recordId && /^[1-9]\d*$/.test(recordId)) this.searchText = recordId;
    this.loadRecords();
    this.loadVehicles();
    this.loadInventory();
  }

  // =========================
  // Load Data
  // =========================

  loadRecords(): void {

    this.maintenanceService.getAll().subscribe({

      next: (data) => {
        this.records = data;
        this.cdr.detectChanges();
      },

      error: () => {
        this.resultDialog.show(
          'Error',
          'Failed to load maintenance records.',
          'error'
        );
      }

    });
  }

  loadVehicles(): void {

    this.vehicleService.getAll().subscribe({

      next: (data) => {
        this.vehicles = data.filter(x => !x.isDeleted);
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

  loadInventory(): void {
    this.inventoryLoading = true;
    this.inventoryFailed = false;
    forkJoin({ oils: this.oilService.getAll(), parts: this.partService.getAll() }).subscribe({
      next: ({ oils, parts }) => {
        this.oils = oils;
        this.spareParts = parts;
        const oilRequests = oils.map(oil => this.oilMovementService.getCurrentStock(oil.oilId).pipe(
          catchError(() => of(null))
        ));
        const partRequests = parts.map(part => this.partMovementService.getCurrentStock(part.sparePartId).pipe(
          catchError(() => of(null))
        ));
        forkJoin({ oilStocks: oilRequests.length ? forkJoin(oilRequests) : of([]),
          partStocks: partRequests.length ? forkJoin(partRequests) : of([]) }).subscribe({
          next: ({ oilStocks, partStocks }) => {
            this.oilStocks = new Map(oils.flatMap((oil, index) =>
              oilStocks[index] !== null && Number.isFinite(oilStocks[index]) ? [[oil.oilId, oilStocks[index] as number] as const] : []
            ));
            this.partStocks = new Map(parts.flatMap((part, index) =>
              partStocks[index] !== null && Number.isFinite(partStocks[index]) ? [[part.sparePartId, partStocks[index] as number] as const] : []
            ));
            this.inventoryFailed = this.oilStocks.size !== oils.length || this.partStocks.size !== parts.length;
            this.inventoryLoading = false;
            this.cdr.detectChanges();
          },
          error: () => { this.inventoryFailed = true; this.inventoryLoading = false; this.cdr.detectChanges(); }
        });
      },
      error: () => { this.inventoryFailed = true; this.inventoryLoading = false; this.cdr.detectChanges(); }
    });
  }

  addOil(): void { this.oilConsumption.push({ oilId: 0, quantity: 0 }); }
  removeOil(index: number): void { this.oilConsumption.splice(index, 1); }
  addPart(): void { this.partConsumption.push({ sparePartId: 0, quantity: 0 }); }
  removePart(index: number): void { this.partConsumption.splice(index, 1); }

  get estimatedMaterialsCost(): number {
    return this.oilConsumption.reduce((sum, item) => sum + item.quantity * (this.oils.find(o => o.oilId === item.oilId)?.purchasePrice ?? 0), 0)
      + this.partConsumption.reduce((sum, item) => sum + item.quantity * (this.spareParts.find(p => p.sparePartId === item.sparePartId)?.purchasePrice ?? 0), 0);
  }

  // =========================
  // Search
  // =========================

  get filteredRecords(): MaintenanceRecord[] {

    const search = this.searchText.trim().toLowerCase();

    if (!search) {
      return this.records;
    }

    return this.records.filter(record =>

      record.plateNumber?.toString().toLowerCase().includes(search) ||

      record.maintenanceRecordId.toString().includes(search) ||

      record.maintenanceType.toLowerCase().includes(search) ||

      (record.workshopName ?? '').toLowerCase().includes(search)

    );
  }

  // =========================
  // Statistics
  // =========================

  get totalRecords(): number {
    return this.records.length;
  }

  get internalRecords(): number {

    return this.records.filter(
      x => x.maintenanceLocation === MaintenanceLocation.Internal
    ).length;

  }

  get externalRecords(): number {

    return this.records.filter(
      x => x.maintenanceLocation === MaintenanceLocation.External
    ).length;

  }

  get completedRecords(): number {

    return this.records.filter(
      x => x.status === MaintenanceStatus.Completed
    ).length;

  }

  get totalMaintenanceCost(): number {

    return this.records.reduce(
      (sum, x) => sum + (x.totalCost || 0),
      0
    );

  }

  // =========================
  // Add Dialog
  // =========================

  openAddDialog(): void {

    this.isEditMode = false;
    this.selectedRecordId = null;
    this.oilConsumption = [];
    this.partConsumption = [];
    this.originalOilConsumption = [];
    this.originalPartConsumption = [];

    this.form = {
      vehicleId: 0,
      maintenanceType: '',
      maintenanceLocation: MaintenanceLocation.Internal,
      maintenanceDate: new Date().toISOString().substring(0, 10),
      odometer: null,
      description: '',
      workshopName: '',
      technicianName: '',
      laborCost: 0,
      status: MaintenanceStatus.Scheduled
    };

    this.dialogVisible = true;
  }

  // =========================
  // Edit Dialog
  // =========================

  openEditDialog(record: MaintenanceRecord): void {

    this.editLoading = true;
    this.maintenanceService.getById(record.maintenanceRecordId).subscribe({
      next: saved => {
        this.editLoading = false;
        this.populateEditDialog(saved);
      },
      error: () => {
        this.editLoading = false;
        this.resultDialog.show('Error', 'Failed to load saved maintenance consumption.', 'error');
        this.cdr.detectChanges();
      }
    });
  }

  private populateEditDialog(record: MaintenanceRecord): void {

    this.isEditMode = true;

    this.selectedRecordId = record.maintenanceRecordId;
    this.oilConsumption = record.oils.map(item => ({ ...item }));
    this.partConsumption = record.spareParts.map(item => ({ ...item }));
    this.originalOilConsumption = record.oils.map(item => ({ ...item }));
    this.originalPartConsumption = record.spareParts.map(item => ({ ...item }));

    this.form = {
      vehicleId: record.vehicleId,
      maintenanceType: record.maintenanceType,
      maintenanceLocation: record.maintenanceLocation,
      maintenanceDate: record.maintenanceDate.substring(0, 10),
      odometer: record.odometer,
      description: record.description ?? '',
      workshopName: record.workshopName ?? '',
      technicianName: record.technicianName ?? '',
      laborCost: record.laborCost,
      status: record.status
    };

    this.dialogVisible = true;
    this.cdr.detectChanges();
  }

  // =========================
  // Save Record
  // =========================

  saveRecord(): void {

    if (this.isSaving || this.inventoryLoading || this.editLoading) return;
    const consumes = this.form.maintenanceLocation === MaintenanceLocation.Internal &&
      this.form.status !== MaintenanceStatus.Cancelled;
    if (consumes) {
      if ((this.inventoryFailed && (this.oilConsumption.length > 0 || this.partConsumption.length > 0)) ||
          new Set(this.oilConsumption.map(x => x.oilId)).size !== this.oilConsumption.length ||
          new Set(this.partConsumption.map(x => x.sparePartId)).size !== this.partConsumption.length ||
          this.oilConsumption.some(x => !this.oils.some(o => o.oilId === x.oilId) || !Number.isFinite(x.quantity) || x.quantity < 0.001 ||
            !this.oilStocks.has(x.oilId) || x.quantity > (this.oilStocks.get(x.oilId) ?? 0) +
              (this.isEditMode ? (this.originalOilConsumption.find(o => o.oilId === x.oilId)?.quantity ?? 0) : 0)) ||
          this.partConsumption.some(x => !this.spareParts.some(p => p.sparePartId === x.sparePartId) || !Number.isFinite(x.quantity) || x.quantity < 0.001 ||
            !this.partStocks.has(x.sparePartId) || x.quantity > (this.partStocks.get(x.sparePartId) ?? 0) +
              (this.isEditMode ? (this.originalPartConsumption.find(p => p.sparePartId === x.sparePartId)?.quantity ?? 0) : 0))) {
        this.resultDialog.show('Validation', 'Select each available item once, enter a positive quantity, and keep consumption within available stock.', 'error');
        return;
      }
    }

    if (
      !this.form.vehicleId ||
      !this.form.maintenanceType.trim() ||
      !this.form.maintenanceDate
    ) {

      this.resultDialog.show(
        'Validation',
        'Please complete all required fields.',
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
        'Validation',
        'Please enter the odometer reading.',
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
        'Validation',
        'Please enter a valid whole odometer number.',
        'error'
      );

      return;
    }

    if (
      this.form.laborCost < 0 ||
      !Number.isFinite(Number(this.form.laborCost))
    ) {

      this.resultDialog.show(
        'Validation',
        'Please enter a valid labor cost.',
        'error'
      );

      return;
    }

    const dto: CreateMaintenanceRecord = {

      vehicleId: this.form.vehicleId,

      maintenanceType: this.form.maintenanceType.trim(),

      maintenanceLocation: this.form.maintenanceLocation,

      maintenanceDate: this.form.maintenanceDate,

      odometer: odometer,

      description: this.form.description.trim(),

      workshopName: this.form.workshopName.trim(),

      technicianName: this.form.technicianName.trim(),

      laborCost: Number(this.form.laborCost),

      oils: consumes ? this.oilConsumption.map(item => ({ ...item })) : [],

      spareParts: consumes ? this.partConsumption.map(item => ({ ...item })) : []

    };

    this.isSaving = true;

    if (
      this.isEditMode &&
      this.selectedRecordId !== null
    ) {

      const updateDto: UpdateMaintenanceRecord = {

        ...dto,

        status: this.form.status

      };

      this.maintenanceService.update(
        this.selectedRecordId,
        updateDto
      ).subscribe({

        next: () => {
          this.finishSave(
            'Maintenance record updated successfully.'
          );
        },

        error: (err) => {
          this.handleSaveError(err);
        }

      });

    } else {

      this.maintenanceService.create(dto).subscribe({

        next: () => {
          this.finishSave(
            'Maintenance record added successfully.'
          );
        },

        error: (err) => {
          this.handleSaveError(err);
        }

      });

    }
  }

  // =========================
  // Finish Save
  // =========================

  private finishSave(message: string): void {

    this.isSaving = false;

    this.dialogVisible = false;

    this.resultDialog.show(
      'Success',
      message,
      'success'
    );

    this.loadRecords();
    this.loadInventory();

    this.cdr.detectChanges();
  }

  // =========================
  // Handle Errors
  // =========================

  private handleSaveError(err: any): void {

    this.isSaving = false;

    const message =
      err.error?.message ||
      'An error occurred while saving the maintenance record.';

    this.resultDialog.show(
      'Error',
      message,
      'error'
    );

    this.cdr.detectChanges();
  }

  // =========================
  // Delete
  // =========================

  confirmDelete(record: MaintenanceRecord): void {

    this.confirmDialog.show(

      () => this.deleteRecord(record.maintenanceRecordId),

      'Delete Maintenance Record',

      'Are you sure you want to delete this maintenance record?'

    );
  }

  private deleteRecord(id: number): void {

    this.maintenanceService.delete(id).subscribe({

      next: () => {

        this.resultDialog.show(
          'Success',
          'Maintenance record deleted successfully.',
          'success'
        );

        this.loadRecords();

      },

      error: (err) => {

        this.resultDialog.show(
          'Error',
          err.error?.message || 'Failed to delete maintenance record.',
          'error'
        );

      }

    });
  }

  // =========================
  // Labels
  // =========================

  getLocationLabel(location: MaintenanceLocation): string {

    return location === MaintenanceLocation.Internal
      ? 'Internal'
      : 'External';

  }

  getLocationClass(location: MaintenanceLocation): string {

    return location === MaintenanceLocation.Internal
      ? 'internal'
      : 'external';

  }

  getStatusLabel(status: MaintenanceStatus): string {

    switch (status) {

      case MaintenanceStatus.Scheduled:
        return 'Scheduled';

      case MaintenanceStatus.InProgress:
        return 'In Progress';

      case MaintenanceStatus.Completed:
        return 'Completed';

      case MaintenanceStatus.Cancelled:
        return 'Cancelled';

      default:
        return 'Unknown';

    }
  }

  getStatusClass(status: MaintenanceStatus): string {

    switch (status) {

      case MaintenanceStatus.Scheduled:
        return 'scheduled';

      case MaintenanceStatus.InProgress:
        return 'in-progress';

      case MaintenanceStatus.Completed:
        return 'completed';

      case MaintenanceStatus.Cancelled:
        return 'cancelled';

      default:
        return '';

    }
  }

  // =========================
  // Helpers
  // =========================

  formatDate(date: string): string {

    return new Date(date).toLocaleDateString('en-GB');

  }

  getVehiclePlate(vehicleId: number): string {

    const vehicle = this.vehicles.find(
      x => x.vehicleId === vehicleId
    );

    return vehicle
      ? vehicle.plateNumber.toString()
      : '';

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
