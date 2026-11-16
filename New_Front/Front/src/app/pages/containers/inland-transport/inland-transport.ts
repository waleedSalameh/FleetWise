import { TranslatePipe } from '../../../shared/i18n/translate.pipe';
import { Component, DestroyRef, OnInit, computed, inject, signal } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { DialogModule } from 'primeng/dialog';
import { finalize, forkJoin, Observable } from 'rxjs';
import {
  InlandTransport,
  InlandTransportSave,
} from '../../../features/containers/models/inland-transport';
import {
  cargoTypes,
  inlandStatuses,
  optionLabel,
} from '../../../features/containers/models/container-options';
import { InlandTransportService } from '../../../features/containers/services/inland-transport';
import { Customer } from '../../../features/customers/models/customer';
import { CustomerService } from '../../../features/customers/services/customer';
import { ConfirmDialogService } from '../../../shared/services/confirm-dialog';
import { ResultDialogService } from '../../../shared/services/result-dialog';
@Component({
  selector: 'app-inland-transport',
  imports: [TranslatePipe, ReactiveFormsModule, DialogModule],
  templateUrl: './inland-transport.html',
  styleUrl: './inland-transport.scss',
})
export class InlandTransportPage implements OnInit {
  private readonly api = inject(InlandTransportService);
  private readonly customersApi = inject(CustomerService);
  private readonly confirm = inject(ConfirmDialogService);
  private readonly result = inject(ResultDialogService);
  private readonly destroyRef = inject(DestroyRef);
  readonly records = signal<InlandTransport[]>([]);
  readonly customers = signal<Customer[]>([]);
  readonly loading = signal(false);
  readonly failed = signal(false);
  readonly saving = signal(false);
  readonly deleting = signal(false);
  readonly visible = signal(false);
  readonly details = signal<InlandTransport | null>(null);
  readonly editingId = signal<number | null>(null);
  readonly search = signal('');
  readonly statusFilter = signal(0);
  readonly statuses = inlandStatuses;
  readonly types = cargoTypes;
  readonly filtered = computed(() =>
    this.records().filter(
      (x) =>
        (!this.statusFilter() || x.tripStatus === this.statusFilter()) &&
        (!this.search().trim() ||
          [x.tripCode, x.customerName, x.loadingLocation, x.unloadingLocation].some((v) =>
            v.toLowerCase().includes(this.search().trim().toLowerCase()),
          )),
    ),
  );
  readonly completedCount = computed(() => this.records().filter((x) => x.tripStatus === 3).length);
  readonly form = new FormGroup({
    customerId: new FormControl(0, { nonNullable: true, validators: [Validators.min(1)] }),
    tripStatus: new FormControl(1, { nonNullable: true }),
    cargoType: new FormControl(1, { nonNullable: true }),
    weight: new FormControl(0, { nonNullable: true, validators: [Validators.min(0.01)] }),
    loadingLocation: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.maxLength(200)],
    }),
    unloadingLocation: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.maxLength(200)],
    }),
    notes: new FormControl('', { nonNullable: true, validators: [Validators.maxLength(500)] }),
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
    this.form.reset({ customerId: 0, tripStatus: 1, cargoType: 1, weight: 0 });
    this.visible.set(true);
  }
  openEdit(x: InlandTransport): void {
    this.editingId.set(x.id);
    this.form.reset({ ...x, notes: x.notes ?? '' });
    this.visible.set(true);
  }
  showDetails(x: InlandTransport): void {
    this.details.set(x);
  }
  label(options: readonly { value: number; label: string }[], value: number): string {
    return optionLabel(options, value);
  }
  save(): void {
    if (this.saving()) return;
    this.form.markAllAsTouched();
    const raw = this.form.getRawValue();
    if (this.form.invalid || !raw.loadingLocation.trim() || !raw.unloadingLocation.trim()) {
      this.result.show('Validation', 'Complete all required fields.', 'error');
      return;
    }
    const dto: InlandTransportSave = {
      ...raw,
      loadingLocation: raw.loadingLocation.trim(),
      unloadingLocation: raw.unloadingLocation.trim(),
      notes: raw.notes.trim() || null,
    };
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
          this.result.show('Success', 'Inland transport saved.', 'success');
          this.load();
        },
        error: (e) => this.result.show('Error', this.error(e), 'error'),
      });
  }
  delete(x: InlandTransport): void {
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
              this.result.show('Success', 'Inland transport deleted.', 'success');
              this.load();
            },
            error: (e) => this.result.show('Error', this.error(e), 'error'),
          });
      },
      'Delete Inland Transport',
      'Delete this inland transport record?',
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
