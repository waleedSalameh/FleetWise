import { TranslatePipe } from '../../../shared/i18n/translate.pipe';
import { Component, DestroyRef, OnInit, computed, inject, input, signal } from '@angular/core';
import { NgTemplateOutlet } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { DialogModule } from 'primeng/dialog';
import { finalize, forkJoin, Observable } from 'rxjs';
import {
  IncomingAqabaBill,
  IncomingAqabaContainer,
  IncomingAqabaContainerSave,
} from '../../../features/containers/models/incoming-aqaba';
import {
  ExportAqabaBooking,
  ExportAqabaContainer,
  ExportAqabaContainerSave,
} from '../../../features/containers/models/export-aqaba';
import {
  aqabaStatuses,
  billStatuses,
  containerSizes,
  containerTypes,
  optionLabel,
  routeStatuses,
} from '../../../features/containers/models/container-options';
import { IncomingAqabaService } from '../../../features/containers/services/incoming-aqaba';
import { ExportAqabaService } from '../../../features/containers/services/export-aqaba';
import { Customer } from '../../../features/customers/models/customer';
import { CustomerService } from '../../../features/customers/services/customer';
import { ShippingLine } from '../../../features/shipping-lines/models/shipping-line';
import { ShippingLineService } from '../../../features/shipping-lines/services/shipping-line';
import { DeliveryLocation } from '../../../features/delivery-locations/models/delivery-location';
import { DeliveryLocationService } from '../../../features/delivery-locations/services/delivery-location';
import { ConfirmDialogService } from '../../../shared/services/confirm-dialog';
import { ResultDialogService } from '../../../shared/services/result-dialog';

type Bill = IncomingAqabaBill | ExportAqabaBooking;
type Container = IncomingAqabaContainer | ExportAqabaContainer;
@Component({
  selector: 'app-incoming-aqaba',
  imports: [TranslatePipe, ReactiveFormsModule, DialogModule, NgTemplateOutlet],
  templateUrl: './incoming-aqaba.html',
  styleUrl: './incoming-aqaba.scss',
})
export class IncomingAqabaPage implements OnInit {
  readonly mode = input<'import' | 'export'>('import');
  private readonly importApi = inject(IncomingAqabaService);
  private readonly exportApi = inject(ExportAqabaService);
  private readonly customersApi = inject(CustomerService);
  private readonly linesApi = inject(ShippingLineService);
  private readonly locationsApi = inject(DeliveryLocationService);
  private readonly confirm = inject(ConfirmDialogService);
  private readonly result = inject(ResultDialogService);
  private readonly destroyRef = inject(DestroyRef);
  readonly records = signal<Bill[]>([]);
  readonly customers = signal<Customer[]>([]);
  readonly lines = signal<ShippingLine[]>([]);
  readonly locations = signal<DeliveryLocation[]>([]);
  readonly loading = signal(false);
  readonly failed = signal(false);
  readonly saving = signal(false);
  readonly deleting = signal(false);
  readonly billVisible = signal(false);
  readonly containerVisible = signal(false);
  readonly selected = signal<Bill | null>(null);
  readonly editingBillId = signal<number | null>(null);
  readonly editingContainerId = signal<number | null>(null);
  readonly createContainerRows = signal<{ id: number; form: FormGroup }[]>([]);
  private nextContainerRowId = 0;
  readonly search = signal('');
  readonly statusFilter = signal(0);
  readonly containerSearch = signal('');
  readonly billStatuses = billStatuses;
  readonly containerTypes = containerTypes;
  readonly containerSizes = containerSizes;
  readonly aqabaStatuses = aqabaStatuses;
  readonly routeStatuses = routeStatuses;
  readonly filtered = computed(() =>
    this.records().filter(
      (x) =>
        (!this.statusFilter() || x.status === this.statusFilter()) &&
        (!this.search().trim() ||
          [this.billNumber(x), x.customerName, ...x.containers.map((c) => c.containerNumber)].some(
            (v) => v.toLowerCase().includes(this.search().trim().toLowerCase()),
          )),
    ),
  );
  readonly selectedContainers = computed(() =>
    (this.selected()?.containers ?? []).filter(
      (c) =>
        !this.containerSearch().trim() ||
        c.containerNumber.toLowerCase().includes(this.containerSearch().trim().toLowerCase()),
    ),
  );
  readonly totalContainers = computed(() =>
    this.records().reduce((sum, x) => sum + x.containers.length, 0),
  );
  readonly completed = computed(() => this.records().filter((x) => x.status === 3).length);
  readonly billForm = new FormGroup({
    number: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.maxLength(100)],
    }),
    customerId: new FormControl(0, { nonNullable: true, validators: [Validators.min(1)] }),
    status: new FormControl(1, { nonNullable: true }),
  });
  readonly importForm = new FormGroup({
    containerNumber: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.maxLength(50)],
    }),
    containerType: new FormControl(1, { nonNullable: true }),
    containerSize: new FormControl(1, { nonNullable: true }),
    shippingLineId: new FormControl(0, { nonNullable: true, validators: [Validators.min(1)] }),
    deliveryLocationId: new FormControl(0, { nonNullable: true, validators: [Validators.min(1)] }),
    weightInTons: new FormControl(0, { nonNullable: true, validators: [Validators.min(0.01)] }),
    vesselArrivalDate: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required],
    }),
    portFreeTimeEndDate: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required],
    }),
    emptyReturnFreeTimeEndDate: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required],
    }),
    containerLoadingDate: new FormControl('', { nonNullable: true }),
    notes: new FormControl('', { nonNullable: true, validators: [Validators.maxLength(1000)] }),
    status: new FormControl(1, { nonNullable: true }),
  });
  readonly exportForm = new FormGroup({
    containerNumber: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.maxLength(50)],
    }),
    containerSize: new FormControl(1, { nonNullable: true }),
    containerType: new FormControl(1, { nonNullable: true }),
    loadingLocationId: new FormControl(0, { nonNullable: true, validators: [Validators.min(1)] }),
    routeStatus: new FormControl(1, { nonNullable: true }),
    withdrawalDate: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    loadingDate: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    deliveryLocationId: new FormControl(0, { nonNullable: true, validators: [Validators.min(1)] }),
    weightInTons: new FormControl(0, { nonNullable: true, validators: [Validators.min(0.01)] }),
    notes: new FormControl('', { nonNullable: true, validators: [Validators.maxLength(1000)] }),
  });
  ngOnInit(): void {
    this.load();
  }
  load(): void {
    this.loading.set(true);
    this.failed.set(false);
    const request = this.mode() === 'import' ? this.importApi.getAll() : this.exportApi.getAll();
    forkJoin({
      records: request,
      customers: this.customersApi.getAll(),
      lines: this.linesApi.getAll(),
      locations: this.locationsApi.getAll(),
    })
      .pipe(
        finalize(() => this.loading.set(false)),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({
        next: ({ records, customers, lines, locations }) => {
          this.records.set(records);
          this.customers.set(customers);
          this.lines.set(lines);
          this.locations.set(locations);
          const id = this.selected()?.id;
          if (id) this.selected.set(records.find((x) => x.id === id) ?? null);
        },
        error: (e) => {
          this.failed.set(true);
          this.result.show('Error', this.error(e), 'error');
        },
      });
  }
  billNumber(x: Bill): string {
    return 'billNumber' in x ? x.billNumber : x.bookingNumber;
  }
  label(options: readonly { value: number; label: string }[], value: number): string {
    return optionLabel(options, value);
  }
  isImportContainer(x: Container): x is IncomingAqabaContainer {
    return 'shippingLineId' in x;
  }
  isExportContainer(x: Container): x is ExportAqabaContainer {
    return 'bookingId' in x;
  }
  openAddBill(): void {
    this.editingBillId.set(null);
    this.billForm.reset({ number: '', customerId: 0, status: 1 });
    this.resetContainer();
    this.createContainerRows.set([]);
    this.addCreateContainer();
    this.billVisible.set(true);
  }
  addCreateContainer(): void {
    const form = this.mode() === 'import' ? this.createImportForm() : this.createExportForm();
    this.createContainerRows.update((rows) => [...rows, { id: this.nextContainerRowId++, form }]);
  }
  removeCreateContainer(id: number): void {
    this.createContainerRows.update((rows) => rows.filter((row) => row.id !== id));
  }
  private createImportForm(): FormGroup {
    return new FormGroup({
      containerNumber: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.maxLength(50)] }),
      containerType: new FormControl(1, { nonNullable: true }),
      containerSize: new FormControl(1, { nonNullable: true }),
      shippingLineId: new FormControl(0, { nonNullable: true, validators: [Validators.min(1)] }),
      deliveryLocationId: new FormControl(0, { nonNullable: true, validators: [Validators.min(1)] }),
      weightInTons: new FormControl(0, { nonNullable: true, validators: [Validators.min(0.01)] }),
      vesselArrivalDate: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
      portFreeTimeEndDate: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
      emptyReturnFreeTimeEndDate: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
      containerLoadingDate: new FormControl('', { nonNullable: true }),
      notes: new FormControl('', { nonNullable: true, validators: [Validators.maxLength(1000)] }),
      status: new FormControl(1, { nonNullable: true }),
    });
  }
  private createExportForm(): FormGroup {
    return new FormGroup({
      containerNumber: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.maxLength(50)] }),
      containerSize: new FormControl(1, { nonNullable: true }),
      containerType: new FormControl(1, { nonNullable: true }),
      loadingLocationId: new FormControl(0, { nonNullable: true, validators: [Validators.min(1)] }),
      routeStatus: new FormControl(1, { nonNullable: true }),
      withdrawalDate: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
      loadingDate: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
      deliveryLocationId: new FormControl(0, { nonNullable: true, validators: [Validators.min(1)] }),
      weightInTons: new FormControl(0, { nonNullable: true, validators: [Validators.min(0.01)] }),
      notes: new FormControl('', { nonNullable: true, validators: [Validators.maxLength(1000)] }),
    });
  }
  openEditBill(x: Bill): void {
    this.editingBillId.set(x.id);
    this.billForm.reset({ number: this.billNumber(x), customerId: x.customerId, status: x.status });
    this.billVisible.set(true);
  }
  openContainers(x: Bill): void {
    this.selected.set(x);
    this.containerSearch.set('');
  }
  openAddContainer(): void {
    if (!this.selected()) return;
    this.editingContainerId.set(null);
    this.resetContainer();
    this.containerVisible.set(true);
  }
  openEditContainer(x: Container): void {
    this.editingContainerId.set(x.id);
    if (this.isImportContainer(x)) {
      this.importForm.reset({
        ...x,
        containerLoadingDate: x.containerLoadingDate ?? '',
        notes: x.notes ?? '',
      });
    } else {
      this.exportForm.reset({
        ...x,
        withdrawalDate: this.toLocalDateTime(x.withdrawalDate),
        loadingDate: this.toLocalDateTime(x.loadingDate),
        notes: x.notes ?? '',
      });
    }
    this.containerVisible.set(true);
  }
  private resetContainer(): void {
    this.importForm.reset({
      containerNumber: '',
      containerType: 1,
      containerSize: 1,
      shippingLineId: 0,
      deliveryLocationId: 0,
      weightInTons: 0,
      status: 1,
    });
    this.exportForm.reset({
      containerNumber: '',
      containerType: 1,
      containerSize: 1,
      loadingLocationId: 0,
      deliveryLocationId: 0,
      routeStatus: 1,
      weightInTons: 0,
    });
  }
  private toLocalDateTime(value: string): string {
    return value?.slice(0, 16) ?? '';
  }
  private importDto(form: FormGroup = this.importForm): IncomingAqabaContainerSave | null {
    form.markAllAsTouched();
    const x = form.getRawValue() as ReturnType<typeof this.importForm.getRawValue>;
    if (form.invalid || !x.containerNumber.trim()) return null;
    return {
      ...x,
      containerNumber: x.containerNumber.trim(),
      containerLoadingDate: x.containerLoadingDate || null,
      notes: x.notes.trim() || null,
    };
  }
  private exportDto(form: FormGroup = this.exportForm): ExportAqabaContainerSave | null {
    form.markAllAsTouched();
    const x = form.getRawValue() as ReturnType<typeof this.exportForm.getRawValue>;
    if (form.invalid || !x.containerNumber.trim()) return null;
    return { ...x, containerNumber: x.containerNumber.trim(), notes: x.notes.trim() || null };
  }
  saveBill(): void {
    if (this.saving()) return;
    this.billForm.markAllAsTouched();
    const x = this.billForm.getRawValue();
    if (this.billForm.invalid || !x.number.trim()) {
      this.invalid();
      return;
    }
    const id = this.editingBillId();
    let request: Observable<unknown>;
    if (this.mode() === 'import') {
      const base = { billNumber: x.number.trim(), customerId: x.customerId, status: x.status };
      if (id === null) {
        const containers = this.createContainerRows().map((row) => this.importDto(row.form));
        const numbers = containers.filter((item): item is IncomingAqabaContainerSave => item !== null).map((item) => item.containerNumber.toLowerCase());
        if (containers.some((item) => item === null) || numbers.length !== new Set(numbers).size) {
          this.invalid();
          return;
        }
        request = this.importApi.createBill({ ...base, containers: containers as IncomingAqabaContainerSave[] });
      } else request = this.importApi.updateBill(id, base);
    } else {
      const base = { bookingNumber: x.number.trim(), customerId: x.customerId, status: x.status };
      if (id === null) {
        const containers = this.createContainerRows().map((row) => this.exportDto(row.form));
        const numbers = containers.filter((item): item is ExportAqabaContainerSave => item !== null).map((item) => item.containerNumber.toLowerCase());
        if (containers.some((item) => item === null) || numbers.length !== new Set(numbers).size) {
          this.invalid();
          return;
        }
        request = this.exportApi.createBooking({ ...base, containers: containers as ExportAqabaContainerSave[] });
      } else request = this.exportApi.updateBooking(id, base);
    }
    this.saving.set(true);
    request
      .pipe(
        finalize(() => this.saving.set(false)),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({
        next: () => {
          this.billVisible.set(false);
          this.createContainerRows.set([]);
          this.billForm.reset({ number: '', customerId: 0, status: 1 });
          this.result.show('Success', 'Record saved successfully.', 'success');
          this.load();
        },
        error: (e) => this.result.show('Error', this.error(e), 'error'),
      });
  }
  saveContainer(): void {
    if (this.saving()) return;
    const bill = this.selected();
    if (!bill) return;
    const id = this.editingContainerId();
    let request: Observable<unknown>;
    if (this.mode() === 'import') {
      const dto = this.importDto();
      if (!dto) {
        this.invalid();
        return;
      }
      request =
        id === null
          ? this.importApi.createContainer(bill.id, dto)
          : this.importApi.updateContainer(id, dto);
    } else {
      const dto = this.exportDto();
      if (!dto) {
        this.invalid();
        return;
      }
      request =
        id === null
          ? this.exportApi.createContainer(bill.id, dto)
          : this.exportApi.updateContainer(id, dto);
    }
    this.saving.set(true);
    request
      .pipe(
        finalize(() => this.saving.set(false)),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({
        next: () => {
          this.containerVisible.set(false);
          this.result.show('Success', 'Container saved successfully.', 'success');
          this.load();
        },
        error: (e) => this.result.show('Error', this.error(e), 'error'),
      });
  }
  deleteBill(x: Bill): void {
    this.confirm.show(
      () => {
        this.deleting.set(true);
        const request =
          this.mode() === 'import'
            ? this.importApi.deleteBill(x.id)
            : this.exportApi.deleteBooking(x.id);
        request
          .pipe(
            finalize(() => this.deleting.set(false)),
            takeUntilDestroyed(this.destroyRef),
          )
          .subscribe({
            next: () => {
              this.selected.set(null);
              this.result.show('Success', 'Record deleted.', 'success');
              this.load();
            },
            error: (e) => this.result.show('Error', this.error(e), 'error'),
          });
      },
      'Delete Record',
      'Delete this record and its containers?',
    );
  }
  deleteContainer(x: Container): void {
    this.confirm.show(
      () => {
        this.deleting.set(true);
        const request =
          this.mode() === 'import'
            ? this.importApi.deleteContainer(x.id)
            : this.exportApi.deleteContainer(x.id);
        request
          .pipe(
            finalize(() => this.deleting.set(false)),
            takeUntilDestroyed(this.destroyRef),
          )
          .subscribe({
            next: () => {
              this.result.show('Success', 'Container deleted.', 'success');
              this.load();
            },
            error: (e) => this.result.show('Error', this.error(e), 'error'),
          });
      },
      'Delete Container',
      'Delete this container?',
    );
  }
  private invalid(): void {
    this.result.show('Validation', 'Complete all required fields with valid values.', 'error');
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
