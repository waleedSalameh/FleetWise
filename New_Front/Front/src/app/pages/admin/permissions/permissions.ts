import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslatePipe } from '../../../shared/i18n/translate.pipe';
import { LanguageService } from '../../../shared/i18n/language.service';
import { ResultDialogService } from '../../../shared/services/result-dialog';
import { AdminPermissionsService } from '../../../features/admin/services/permissions.service';
import { UsersService } from '../../../features/admin/services/users.service';
import { UserPermissionStatus, UserSummary } from '../../../features/admin/models/admin.models';

@Component({ selector: 'app-permissions-page', standalone: true, imports: [CommonModule, FormsModule, TranslatePipe], templateUrl: './permissions.html', styleUrl: './permissions.scss' })
export class PermissionsPage implements OnInit {
  private readonly usersApi = inject(UsersService); private readonly api = inject(AdminPermissionsService); private readonly result = inject(ResultDialogService); private readonly language = inject(LanguageService); private readonly cdr = inject(ChangeDetectorRef);
  users: UserSummary[] = []; userId = ''; permissions: UserPermissionStatus[] = []; grouped: [string, UserPermissionStatus[]][] = [];
  isLoadingUsers = false; isLoadingPermissions = false;
  ngOnInit(): void { this.isLoadingUsers = true; this.usersApi.getAll(1, 100).subscribe({ next: users => { this.users = users.filter(user => user.isActive); this.isLoadingUsers = false; this.cdr.detectChanges(); }, error: e => { this.isLoadingUsers = false; this.fail(e); } }); }
  load(): void {
    if (!this.userId) { this.permissions = []; this.grouped = []; this.isLoadingPermissions = false; this.cdr.detectChanges(); return; }
    this.isLoadingPermissions = true;
    this.api.getUserPermissions(this.userId).subscribe({
      next: rows => { this.permissions = rows; const groups = new Map<string, UserPermissionStatus[]>(); rows.forEach(row => groups.set(row.module, [...(groups.get(row.module) ?? []), row])); this.grouped = [...groups.entries()]; this.isLoadingPermissions = false; this.cdr.detectChanges(); },
      error: e => { this.isLoadingPermissions = false; this.fail(e); },
    });
  }
  set(row: UserPermissionStatus, value: 'allow' | 'deny' | 'inherit'): void {
    if (value === 'inherit') this.api.removeOverride(this.userId, row.permissionCode).subscribe({ next: () => { this.done('admin.permissions.overrideRemoved'); this.cdr.detectChanges(); this.load(); }, error: e => this.fail(e) });
    else this.api.setOverride(this.userId, { permissionCode: row.permissionCode, isAllowed: value === 'allow' }).subscribe({ next: () => { this.done(value === 'allow' ? 'admin.permissions.allowedSaved' : 'admin.permissions.deniedSaved'); this.cdr.detectChanges(); this.load(); }, error: e => this.fail(e) });
  }
  private done(message: string): void { this.result.show(this.language.text('admin.common.success'), this.language.text(message), 'success'); }
  private fail(error: any): void { this.result.show(this.language.text('admin.common.requestFailed'), error?.error?.message ?? error?.message ?? this.language.text('admin.common.requestFailedMessage'), 'error'); this.cdr.detectChanges(); }
}
