import { TranslatePipe } from '../../../shared/i18n/translate.pipe';
import { Component, DestroyRef, OnInit, computed, inject, signal } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { DialogModule } from 'primeng/dialog';
import { finalize, forkJoin, Observable } from 'rxjs';
import {
  InternationalIncoming,
  InternationalIncomingSave,
} from '../../../features/containers/models/international-incoming';
import {
  cargoTypes,
  orderStatuses,
  optionLabel,
} from '../../../features/containers/models/container-options';
import { InternationalIncomingService } from '../../../features/containers/services/international-incoming';
import { Customer } from '../../../features/customers/models/customer';
import { CustomerService } from '../../../features/customers/services/customer';
import { ConfirmDialogService } from '../../../shared/services/confirm-dialog';
import { ResultDialogService } from '../../../shared/services/result-dialog';

@Component({
  selector: 'app-international-incoming',
  imports: [TranslatePipe, ReactiveFormsModule, DialogModule],
  templateUrl: './international-incoming.html',
  styleUrl: './international-incoming.scss',
})
export class InternationalIncomingPage implements OnInit {
  private readonly api = inject(InternationalIncomingService);
  private readonly customersApi = inject(CustomerService);
  private readonly confirm = inject(ConfirmDialogService);
  private readonly result = inject(ResultDialogService);
  private readonly destroyRef = inject(DestroyRef);
  readonly records = signal<InternationalIncoming[]>([]);
  readonly customers = signal<Customer[]>([]);
  readonly loading = signal(false);
  readonly failed = signal(false);
  readonly saving = signal(false);
  readonly deleting = signal(false);
  readonly visible = signal(false);
  readonly details = signal<InternationalIncoming | null>(null);
  readonly editingId = signal<number | null>(null);
  readonly search = signal('');
  readonly statusFilter = signal(0);
  readonly statuses = orderStatuses;
  readonly types = cargoTypes;
  readonly filtered = computed(() =>
    this.records().filter(
      (x) =>
        (!this.statusFilter() || x.status === this.statusFilter()) &&
        (!this.search().trim() ||
          [x.cargoNumber, x.customerName, x.country, x.customsDeclarationNumber].some((v) =>
            v.toLowerCase().includes(this.search().trim().toLowerCase()),
          )),
    ),
  );
  readonly paidCount = computed(() => this.records().filter((x) => x.isPaid).length);
  readonly completedCount = computed(() => this.records().filter((x) => x.status === 3).length);
  readonly form = new FormGroup({
    cargoNumber: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.maxLength(100)],
    }),
    customerId: new FormControl(0, { nonNullable: true, validators: [Validators.min(1)] }),
    country: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.maxLength(100)],
    }),
    cargoType: new FormControl(1, { nonNullable: true, validators: [Validators.required] }),
    loadingLocation: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.maxLength(200)],
    }),
    factoryName: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.maxLength(200)],
    }),
    weight: new FormControl(0, { nonNullable: true, validators: [Validators.min(0.01)] }),
    notes: new FormControl('', { nonNullable: true, validators: [Validators.maxLength(1000)] }),
    customsDeclarationNumber: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.maxLength(100)],
    }),
    tripDuration: new FormControl(0, { nonNullable: true, validators: [Validators.min(1)] }),
    unloadingLocation: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.maxLength(200)],
    }),
    distance: new FormControl(0, { nonNullable: true, validators: [Validators.min(0.01)] }),
    status: new FormControl(1, { nonNullable: true }),
    isPaid: new FormControl(false, { nonNullable: true }),
  });
  ngOnInit(): void {
    this.load();
  }
  load(): void {
    this.loading.set(true);
    this.failed.set(false);
    forkJoin({ records: this.api.getAll(), customers: this.customersApi.getAll() })
      .pipe(
        finalize(() => this.loading.set(false)),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({
        next: ({ records, customers }) => {
          this.records.set(records);
          this.customers.set(customers);
          const id = this.details()?.id;
          if (id) this.details.set(records.find((x) => x.id === id) ?? null);
        },
        error: (e) => {
          this.failed.set(true);
          this.result.show('Error', this.error(e), 'error');
        },
      });
  }
  openAdd(): void {
    this.editingId.set(null);
    this.form.reset({ cargoType: 1, status: 1, customerId: 0, isPaid: false });
    this.visible.set(true);
  }
  openEdit(x: InternationalIncoming): void {
    this.editingId.set(x.id);
    this.form.reset({ ...x, notes: x.notes ?? '' });
    this.visible.set(true);
  }
  showDetails(x: InternationalIncoming): void {
    this.details.set(x);
  }
  label(options: readonly { value: number; label: string }[], value: number): string {
    return optionLabel(options, value);
  }
  save(): void {
    if (this.saving()) return;
    this.form.markAllAsTouched();
    const raw = this.form.getRawValue();
    if (this.form.invalid || (raw.status === 3 && !raw.isPaid)) {
      this.result.show(
        'Validation',
        'Complete valid fields. A completed order must be paid.',
        'error',
      );
      return;
    }
    const dto: InternationalIncomingSave = {
      ...raw,
      cargoNumber: raw.cargoNumber.trim(),
      country: raw.country.trim(),
      loadingLocation: raw.loadingLocation.trim(),
      factoryName: raw.factoryName.trim(),
      customsDeclarationNumber: raw.customsDeclarationNumber.trim(),
      unloadingLocation: raw.unloadingLocation.trim(),
      notes: raw.notes.trim() || null,
    };
    if (
      [
        dto.cargoNumber,
        dto.country,
        dto.loadingLocation,
        dto.factoryName,
        dto.customsDeclarationNumber,
        dto.unloadingLocation,
      ].some((x) => !x)
    ) {
      this.result.show('Validation', 'Complete all required fields.', 'error');
      return;
    }
    this.saving.set(true);
    const id = this.editingId();
    const request: Observable<unknown> =
      id === null ? this.api.create(dto) : this.api.update(id, dto);
    request
      .pipe(
        finalize(() => this.saving.set(false)),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({
        next: () => {
          this.visible.set(false);
          this.result.show('Success', 'International import saved successfully.', 'success');
          this.load();
        },
        error: (e) => this.result.show('Error', this.error(e), 'error'),
      });
  }
  delete(x: InternationalIncoming): void {
    this.confirm.show(
      () => {
        this.deleting.set(true);
        this.api
          .delete(x.id)
          .pipe(
            finalize(() => this.deleting.set(false)),
            takeUntilDestroyed(this.destroyRef),
          )
          .subscribe({
            next: () => {
              this.result.show('Success', 'International import deleted.', 'success');
              this.load();
            },
            error: (e) => this.result.show('Error', this.error(e), 'error'),
          });
      },
      'Delete International Import',
      'Delete this international import record?',
    );
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
