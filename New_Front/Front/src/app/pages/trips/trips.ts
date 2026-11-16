import { TranslatePipe } from '../../shared/i18n/translate.pipe';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, DestroyRef, OnInit, computed, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { DialogModule } from 'primeng/dialog';
import { finalize, forkJoin, Observable } from 'rxjs';
import {
  Trip,
  TripSave,
  UpdateTrip,
  collectionMethods,
  tripOptionLabel,
  tripStatuses,
  tripTypes,
  vehicleSources,
  VehicleSourceType,
} from '../../features/trips/models/trip';
import { TripService } from '../../features/trips/services/trip';
import { VehicleAssignment } from '../../features/vehicle-assignments/models/vehicle-assignment';
import { VehicleAssignmentService } from '../../features/vehicle-assignments/services/vehicle-assignment';
import { Driver, DriverStatus } from '../../features/drivers/models/driver';
import { DriverService } from '../../features/drivers/services/driver';
import { ExternalVehicle } from '../../features/external-vehicles/models/external-vehicle';
import { ExternalVehicleService } from '../../features/external-vehicles/services/external-vehicle';
import { IncomingAqabaBill } from '../../features/containers/models/incoming-aqaba';
import { IncomingAqabaService } from '../../features/containers/services/incoming-aqaba';
import { ExportAqabaBooking } from '../../features/containers/models/export-aqaba';
import { ExportAqabaService } from '../../features/containers/services/export-aqaba';
import { InternationalIncoming } from '../../features/containers/models/international-incoming';
import { InternationalIncomingService } from '../../features/containers/services/international-incoming';
import { InternationalExport } from '../../features/containers/models/international-export';
import { InternationalExportService } from '../../features/containers/services/international-export';
import { InlandTransport } from '../../features/containers/models/inland-transport';
import { InlandTransportService } from '../../features/containers/services/inland-transport';
import { ConfirmDialogService } from '../../shared/services/confirm-dialog';
import { ResultDialogService } from '../../shared/services/result-dialog';

@Component({
  selector: 'app-trips',
  imports: [TranslatePipe, ReactiveFormsModule, DialogModule],
  templateUrl: './trips.html',
  styleUrl: './trips.scss',
})
export class TripsPage implements OnInit {
  private previousVehicleSource: number = VehicleSourceType.Internal;
  private readonly api = inject(TripService);
  private readonly assignmentsApi = inject(VehicleAssignmentService);
  private readonly driversApi = inject(DriverService);
  private readonly externalApi = inject(ExternalVehicleService);
  private readonly importApi = inject(IncomingAqabaService);
  private readonly exportApi = inject(ExportAqabaService);
  private readonly internationalImportApi = inject(InternationalIncomingService);
  private readonly internationalExportApi = inject(InternationalExportService);
  private readonly inlandApi = inject(InlandTransportService);
  private readonly confirm = inject(ConfirmDialogService);
  private readonly result = inject(ResultDialogService);
  private readonly destroyRef = inject(DestroyRef);

  readonly records = signal<Trip[]>([]);
  readonly assignments = signal<VehicleAssignment[]>([]);
  readonly drivers = signal<Driver[]>([]);
  readonly externalVehicles = signal<ExternalVehicle[]>([]);
  readonly importBills = signal<IncomingAqabaBill[]>([]);
  readonly exportBookings = signal<ExportAqabaBooking[]>([]);
  readonly internationalImports = signal<InternationalIncoming[]>([]);
  readonly internationalExports = signal<InternationalExport[]>([]);
  readonly inlandTransports = signal<InlandTransport[]>([]);
  readonly loading = signal(false);
  readonly failed = signal(false);
  readonly saving = signal(false);
  readonly deleting = signal(false);
  readonly formVisible = signal(false);
  readonly details = signal<Trip | null>(null);
  readonly editingId = signal<number | null>(null);
  readonly search = signal('');
  readonly statusFilter = signal(-1);
  readonly typeFilter = signal(0);
  readonly sourceFilter = signal(0);
  readonly statuses = tripStatuses;
  readonly types = tripTypes;
  readonly sources = vehicleSources;
  readonly methods = collectionMethods;
  readonly activeDrivers = computed(() =>
    this.drivers().filter((x) => !x.isDeleted && x.status === DriverStatus.Active),
  );
  selectedImportBill(): IncomingAqabaBill | undefined {
    return this.importBills().find((x) => x.id === this.form.controls.incomingAqabaBillId.value);
  }
  selectedExportBooking(): ExportAqabaBooking | undefined {
    return this.exportBookings().find(
      (x) => x.id === this.form.controls.exportAqabaBookingId.value,
    );
  }
  readonly filtered = computed(() =>
    this.records().filter(
      (x) =>
        (this.statusFilter() < 0 || x.status === this.statusFilter()) &&
        (!this.typeFilter() || x.tripType === this.typeFilter()) &&
        (!this.sourceFilter() || x.vehicleSourceType === this.sourceFilter()) &&
        (!this.search().trim() ||
          [
            String(x.tripId),
            x.startLocation,
            x.destination,
            x.driverName ?? '',
            x.externalDriverName ?? '',
            this.relatedName(x),
            this.vehicleName(x),
          ].some((v) => v.toLowerCase().includes(this.search().trim().toLowerCase()))),
    ),
  );
  readonly planned = computed(() => this.records().filter((x) => x.status === 0).length);
  readonly inProgress = computed(() => this.records().filter((x) => x.status === 1).length);
  readonly completed = computed(() => this.records().filter((x) => x.status === 2).length);

  readonly form = new FormGroup({
    vehicleSourceType: new FormControl(VehicleSourceType.Internal, { nonNullable: true }),
    assignmentId: new FormControl<number | null>(null),
    driverId: new FormControl<number | null>(null),
    externalVehicleId: new FormControl<number | null>(null),
    tripType: new FormControl(1, { nonNullable: true }),
    incomingAqabaBillId: new FormControl<number | null>(null),
    incomingAqabaContainerId: new FormControl<number | null>(null),
    exportAqabaBookingId: new FormControl<number | null>(null),
    exportAqabaContainerId: new FormControl<number | null>(null),
    internationalIncomingId: new FormControl<number | null>(null),
    internationalExportId: new FormControl<number | null>(null),
    inlandTransportId: new FormControl<number | null>(null),
    startLocation: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.maxLength(100)],
    }),
    destination: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.maxLength(100)],
    }),
    loadingDate: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    unloadingDate: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    distance: new FormControl(0, { nonNullable: true, validators: [Validators.min(0)] }),
    driverTip: new FormControl<number | null>(null, Validators.min(0)),
    dieselCost: new FormControl<number | null>(null, Validators.min(0)),
    leakageCost: new FormControl<number | null>(null, Validators.min(0)),
    otherExpenses: new FormControl<number | null>(null, Validators.min(0)),
    purchasePrice: new FormControl<number | null>(null, Validators.min(0)),
    salePrice: new FormControl<number | null>(null, Validators.min(0)),
    advancePayment: new FormControl<number | null>(null, Validators.min(0)),
    collectionMethod: new FormControl<number | null>(null),
    notes: new FormControl('', { nonNullable: true, validators: [Validators.maxLength(500)] }),
    status: new FormControl(0, { nonNullable: true }),
  });

  ngOnInit(): void {
    this.load();
  }
  load(): void {
    this.loading.set(true);
    this.failed.set(false);
    forkJoin({
      trips: this.api.getAll(),
      assignments: this.assignmentsApi.getAll(),
      drivers: this.driversApi.getAll(),
      external: this.externalApi.getAll(),
      imports: this.importApi.getAll(),
      exports: this.exportApi.getAll(),
      internationalImports: this.internationalImportApi.getAll(),
      internationalExports: this.internationalExportApi.getAll(),
      inland: this.inlandApi.getAll(),
    })
      .pipe(
        finalize(() => this.loading.set(false)),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({
        next: (data) => {
          this.records.set(data.trips);
          this.assignments.set(data.assignments);
          this.drivers.set(data.drivers);
          this.externalVehicles.set(data.external);
          this.importBills.set(data.imports);
          this.exportBookings.set(data.exports);
          this.internationalImports.set(data.internationalImports);
          this.internationalExports.set(data.internationalExports);
          this.inlandTransports.set(data.inland);
          const id = this.details()?.tripId;
          if (id) this.details.set(data.trips.find((x) => x.tripId === id) ?? null);
        },
        error: (e) => {
          this.failed.set(true);
          this.result.show('Error', this.error(e), 'error');
        },
      });
  }
  label(options: readonly { value: number; label: string }[], value: number | null): string {
    return tripOptionLabel(options, value);
  }
  vehicleName(x: Trip): string {
    if (x.vehicleSourceType === 2)
      return (
        x.externalVehiclePlateNumber?.toString() ??
        this.externalVehicles()
          .find((v) => v.id === x.externalVehicleId)
          ?.plateNumber.toString() ??
        '—'
      );
    return (
      this.assignments()
        .find((a) => a.assignmentId === x.assignmentId)
        ?.plateNumber.toString() ?? (x.assignmentId ? `Assignment #${x.assignmentId}` : '—')
    );
  }
  driverName(x: Trip): string {
    return x.vehicleSourceType === 2
      ? (x.externalDriverName ??
          this.externalVehicles().find((v) => v.id === x.externalVehicleId)?.driverName ??
          '—')
      : (x.driverName ?? '—');
  }
  relatedName(x: Trip): string {
    switch (x.tripType) {
      case 1:
        return [x.billNumber, x.incomingContainerNumber].filter(Boolean).join(' / ') || '—';
      case 2:
        return [x.bookingNumber, x.exportContainerNumber].filter(Boolean).join(' / ') || '—';
      case 3:
        return x.internationalIncomingCargoNumber ?? '—';
      case 4:
        return x.internationalExportCargoNumber ?? '—';
      case 5:
        return x.inlandTransportTripCode ?? '—';
      default:
        return '—';
    }
  }
  showDetails(x: Trip): void {
    this.details.set(x);
  }
  openAdd(): void {
    this.editingId.set(null);
    this.form.reset({
      vehicleSourceType: VehicleSourceType.Internal,
      tripType: 1,
      distance: 0,
      status: 0,
    });
    this.previousVehicleSource = VehicleSourceType.Internal;
    this.formVisible.set(true);
  }
  openEdit(x: Trip): void {
    this.editingId.set(x.tripId);
    this.previousVehicleSource = x.vehicleSourceType;
    this.form.reset({
      ...x,
      loadingDate: this.localDateTime(x.loadingDate),
      unloadingDate: this.localDateTime(x.unloadingDate),
      notes: x.notes ?? '',
    });
    this.formVisible.set(true);
  }
  onVehicleSourceChange(): void {
    const source = this.form.controls.vehicleSourceType;
    const nextSource = source.value;
    const previousSource = this.previousVehicleSource;
    if (nextSource === previousSource) return;

    const hasIncompatibleValues =
      nextSource === VehicleSourceType.Internal
        ? this.form.controls.purchasePrice.value !== null ||
          this.form.controls.salePrice.value !== null
        : this.form.controls.driverTip.value !== null ||
          this.form.controls.dieselCost.value !== null ||
          this.form.controls.leakageCost.value !== null ||
          this.form.controls.otherExpenses.value !== null;

    if (hasIncompatibleValues) {
      const confirmed = window.confirm(
        'Changing the vehicle source will clear financial values that do not apply to the new source. Continue?',
      );
      if (!confirmed) {
        source.setValue(previousSource, { emitEvent: false });
        return;
      }
    }

    if (nextSource === VehicleSourceType.Internal) {
      this.form.patchValue({
        externalVehicleId: null,
        purchasePrice: null,
        salePrice: null,
      });
    } else {
      this.form.patchValue({
        assignmentId: null,
        driverId: null,
        driverTip: null,
        dieselCost: null,
        leakageCost: null,
        otherExpenses: null,
      });
    }
    this.previousVehicleSource = nextSource;
  }
  onTripTypeChange(): void {
    this.form.patchValue({
      incomingAqabaBillId: null,
      incomingAqabaContainerId: null,
      exportAqabaBookingId: null,
      exportAqabaContainerId: null,
      internationalIncomingId: null,
      internationalExportId: null,
      inlandTransportId: null,
    });
  }
  onImportBillChange(): void {
    this.form.controls.incomingAqabaContainerId.setValue(null);
  }
  onExportBookingChange(): void {
    this.form.controls.exportAqabaContainerId.setValue(null);
  }
  private localDateTime(value: string): string {
    return value?.slice(0, 16) ?? '';
  }
  save(): void {
    if (this.saving()) return;
    this.form.markAllAsTouched();
    const raw = this.form.getRawValue();
    if (
      this.form.invalid ||
      !raw.startLocation.trim() ||
      !raw.destination.trim() ||
      raw.unloadingDate < raw.loadingDate
    ) {
      this.validation('Check the required fields and date order.');
      return;
    }
    if (raw.vehicleSourceType === 1 && (!raw.assignmentId || !raw.driverId)) {
      this.validation('Select an assignment and active driver.');
      return;
    }
    if (raw.vehicleSourceType === 2 && !raw.externalVehicleId) {
      this.validation('Select an external vehicle.');
      return;
    }
    if (
      (raw.tripType === 1 && (!raw.incomingAqabaBillId || !raw.incomingAqabaContainerId)) ||
      (raw.tripType === 2 && (!raw.exportAqabaBookingId || !raw.exportAqabaContainerId)) ||
      (raw.tripType === 3 && !raw.internationalIncomingId) ||
      (raw.tripType === 4 && !raw.internationalExportId) ||
      (raw.tripType === 5 && !raw.inlandTransportId)
    ) {
      this.validation('Select the related operation.');
      return;
    }
    const dto: TripSave = {
      vehicleSourceType: raw.vehicleSourceType,
      assignmentId: raw.vehicleSourceType === 1 ? raw.assignmentId : null,
      driverId: raw.vehicleSourceType === 1 ? raw.driverId : null,
      externalVehicleId: raw.vehicleSourceType === 2 ? raw.externalVehicleId : null,
      tripType: raw.tripType,
      collectionMethod: raw.collectionMethod,
      incomingAqabaBillId: raw.tripType === 1 ? raw.incomingAqabaBillId : null,
      incomingAqabaContainerId: raw.tripType === 1 ? raw.incomingAqabaContainerId : null,
      exportAqabaBookingId: raw.tripType === 2 ? raw.exportAqabaBookingId : null,
      exportAqabaContainerId: raw.tripType === 2 ? raw.exportAqabaContainerId : null,
      internationalIncomingId: raw.tripType === 3 ? raw.internationalIncomingId : null,
      internationalExportId: raw.tripType === 4 ? raw.internationalExportId : null,
      inlandTransportId: raw.tripType === 5 ? raw.inlandTransportId : null,
      startLocation: raw.startLocation.trim(),
      destination: raw.destination.trim(),
      loadingDate: raw.loadingDate,
      unloadingDate: raw.unloadingDate,
      distance: raw.distance,
      driverTip: raw.driverTip,
      dieselCost: raw.dieselCost,
      leakageCost: raw.leakageCost,
      otherExpenses: raw.otherExpenses,
      purchasePrice: raw.purchasePrice,
      salePrice: raw.salePrice,
      advancePayment: raw.advancePayment,
      notes: raw.notes.trim() || null,
    };
    const id = this.editingId();
    const request: Observable<unknown> =
      id === null
        ? this.api.create(dto)
        : this.api.update(id, { ...dto, status: raw.status } as UpdateTrip);
    this.saving.set(true);
    request
      .pipe(
        finalize(() => this.saving.set(false)),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({
        next: () => {
          this.formVisible.set(false);
          this.result.show('Success', 'Trip saved successfully.', 'success');
          this.load();
        },
        error: (e) => this.result.show('Error', this.error(e), 'error'),
      });
  }
  delete(x: Trip): void {
    if (x.status !== 3 || this.deleting()) return;
    this.confirm.show(
      () => {
        this.deleting.set(true);
        this.api
          .delete(x.tripId)
          .pipe(
            finalize(() => this.deleting.set(false)),
            takeUntilDestroyed(this.destroyRef),
          )
          .subscribe({
            next: () => {
              this.details.set(null);
              this.result.show('Success', 'Trip deleted.', 'success');
              this.load();
            },
            error: (e) => this.result.show('Error', this.error(e), 'error'),
          });
      },
      'Delete Trip',
      'Delete this cancelled trip?',
    );
  }
  private validation(message: string): void {
    this.result.show('Validation', message, 'error');
  }
  private error(e: unknown): string {
    if (e instanceof HttpErrorResponse) {
      const body: unknown = e.error;
      if (body && typeof body === 'object' && 'message' in body && typeof body.message === 'string')
        return body.message;
      if (typeof body === 'string' && body.trim()) return body;
    }
    return 'The request could not be completed.';
  }
}
