import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslatePipe } from '../../../shared/i18n/translate.pipe';
import { LanguageService } from '../../../shared/i18n/language.service';
import { ConfirmDialogService } from '../../../shared/services/confirm-dialog';
import { ResultDialogService } from '../../../shared/services/result-dialog';
import { RolesService } from '../../../features/admin/services/roles.service';
import { RolePermissionStatus, RoleSummary } from '../../../features/admin/models/admin.models';

@Component({ selector: 'app-roles-page', standalone: true, imports: [CommonModule, FormsModule, TranslatePipe], templateUrl: './roles.html', styleUrl: './roles.scss' })
export class RolesPage implements OnInit {
  private readonly api = inject(RolesService); private readonly confirm = inject(ConfirmDialogService); private readonly result = inject(ResultDialogService); private readonly language = inject(LanguageService);
  private readonly cdr = inject(ChangeDetectorRef);
  roles: RoleSummary[] = []; permissions: RolePermissionStatus[] = []; selected: RoleSummary | null = null; selectedCodes = new Set<string>(); name = ''; showCreate = false;
  isLoading = false;
  get isProtectedAdmin(): boolean { return this.selected?.name.toLowerCase() === 'admin'; }
  ngOnInit(): void { this.load(); }
  load(): void { this.isLoading = true; this.api.getAll().subscribe({ next: x => { this.roles = x; this.isLoading = false; this.cdr.detectChanges(); }, error: e => { this.isLoading = false; this.fail(e); } }); }
  create(): void { const name = this.name.trim(); if (!name) return; this.api.create({ name }).subscribe({ next: () => { this.name = ''; this.showCreate = false; this.done('admin.roles.created'); this.cdr.detectChanges(); this.load(); }, error: e => this.fail(e) }); }
  select(role: RoleSummary): void { this.selected = role; this.permissions = []; this.cdr.detectChanges(); this.api.getPermissions(role.id).subscribe({ next: x => { this.permissions = x; this.selectedCodes = new Set(x.filter(p => p.isAssigned).map(p => p.permissionCode)); this.cdr.detectChanges(); }, error: e => this.fail(e) }); }
  toggle(code: string, checked: boolean): void { checked ? this.selectedCodes.add(code) : this.selectedCodes.delete(code); }
  savePermissions(): void { if (!this.selected || this.isProtectedAdmin) return; this.api.replacePermissions(this.selected.id, { permissionCodes: [...this.selectedCodes] }).subscribe({ next: () => { this.done('admin.roles.permissionsUpdated'); this.cdr.detectChanges(); this.select(this.selected!); }, error: e => this.fail(e) }); }
  renameRole(role: RoleSummary): void { if (role.name.toLowerCase() === 'admin') return; const name = window.prompt(this.language.text('admin.roles.newName'), role.name)?.trim(); if (!name || name === role.name) return; this.api.update(role.id, { name }).subscribe({ next: () => { this.done('admin.roles.updated'); this.cdr.detectChanges(); this.load(); }, error: e => this.fail(e) }); }
  deleteRole(role: RoleSummary): void { if (role.name.toLowerCase() === 'admin') return; this.confirm.show(() => this.api.delete(role.id).subscribe({ next: () => { this.done('admin.roles.deleted'); this.selected = null; this.cdr.detectChanges(); this.load(); }, error: e => this.fail(e) }), this.language.text('admin.roles.deleteTitle'), this.language.text('admin.roles.confirmDelete', { name: role.name }), { actionLabel: this.language.text('admin.common.delete'), variant: 'danger' }); }
  private done(message: string): void { this.result.show(this.language.text('admin.common.success'), this.language.text(message), 'success'); }
  private fail(error: any): void { this.result.show(this.language.text('admin.common.requestFailed'), error?.error?.message ?? error?.message ?? this.language.text('admin.common.requestFailedMessage'), 'error'); this.cdr.detectChanges(); }
}
