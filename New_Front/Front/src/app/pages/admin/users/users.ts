import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslatePipe } from '../../../shared/i18n/translate.pipe';
import { LanguageService } from '../../../shared/i18n/language.service';
import { ConfirmDialogService } from '../../../shared/services/confirm-dialog';
import { ResultDialogService } from '../../../shared/services/result-dialog';
import { PermissionService } from '../../../features/auth/services/permission.service';
import { AuthService } from '../../../features/auth/services/auth.service';
import { UsersService } from '../../../features/admin/services/users.service';
import { RolesService } from '../../../features/admin/services/roles.service';
import { RoleSummary, UserSummary } from '../../../features/admin/models/admin.models';

@Component({ selector: 'app-users-page', standalone: true, imports: [CommonModule, FormsModule, TranslatePipe], templateUrl: './users.html', styleUrl: './users.scss' })
export class UsersPage implements OnInit {
  private readonly usersApi = inject(UsersService);
  private readonly rolesApi = inject(RolesService);
  private readonly confirm = inject(ConfirmDialogService);
  private readonly result = inject(ResultDialogService);
  private readonly auth = inject(AuthService);
  private readonly cdr = inject(ChangeDetectorRef);
  private readonly language = inject(LanguageService);
  readonly permissions = inject(PermissionService);
  users: UserSummary[] = []; roles: RoleSummary[] = []; filtered: UserSummary[] = [];
  pageNumber = 1; readonly pageSize = 100; hasNextPage = false;
  isLoading = false;
  search = ''; selected: UserSummary | null = null; assignedRoleIds = new Set<string>();
  showCreate = false; showPassword = false; showTemporaryPassword = false; email = ''; password = ''; newPassword = ''; busy = false;
  get isEmailValid(): boolean { return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(this.email.trim()); }
  get assignableRoles(): RoleSummary[] {
    const isAdmin = this.auth.currentUser()?.roles.some(role => role.toLowerCase() === 'admin') ?? false;
    return isAdmin ? this.roles : this.roles.filter(role => role.name.toLowerCase() !== 'admin');
  }

  ngOnInit(): void { this.load(); this.rolesApi.getAll().subscribe({ next: x => { this.roles = x; this.cdr.detectChanges(); }, error: e => this.fail(e) }); }
  load(): void {
    this.isLoading = true;
    this.usersApi.getAll(this.pageNumber, this.pageSize).subscribe({
      next: x => { this.users = x; this.hasNextPage = x.length === this.pageSize; this.filter(); this.isLoading = false; this.cdr.detectChanges(); },
      error: e => { this.isLoading = false; this.fail(e); },
    });
  }
  previousPage(): void { if (this.pageNumber > 1) { this.pageNumber--; this.load(); } }
  nextPage(): void { if (this.hasNextPage) { this.pageNumber++; this.load(); } }
  filter(): void { const q = this.search.trim().toLowerCase(); this.filtered = this.users.filter(u => (u.email ?? '').toLowerCase().includes(q)); }
  openCreateDialog(): void { this.email = ''; this.password = ''; this.showTemporaryPassword = false; this.showCreate = true; }
  closeCreateDialog(): void { if (this.busy) return; this.showCreate = false; this.email = ''; this.password = ''; this.showTemporaryPassword = false; }
  create(): void {
    if (!this.isEmailValid || this.password.length < 12 || this.busy) return;
    this.busy = true; this.usersApi.create({ email: this.email.trim(), temporaryPassword: this.password }).subscribe({
      next: () => { this.busy = false; this.closeCreateDialog(); this.done('admin.users.created'); this.cdr.detectChanges(); this.load(); }, error: e => { this.busy = false; this.fail(e); }
    });
  }
  toggleActive(user: UserSummary): void {
    const activate = !user.isActive;
    this.confirm.show(() => (activate ? this.usersApi.activate(user.id) : this.usersApi.disable(user.id)).subscribe({ next: () => { this.done(activate ? 'admin.users.activated' : 'admin.users.disabled'); this.load(); }, error: e => this.fail(e) }), this.language.text(activate ? 'admin.users.activateTitle' : 'admin.users.disableTitle'), this.language.text('admin.users.confirmAccountAction', { email: user.email ?? '' }), { actionLabel: this.language.text(activate ? 'admin.users.activate' : 'admin.users.disable'), variant: activate ? 'primary' : 'danger' });
  }
  choose(user: UserSummary): void {
    this.selected = user; this.showPassword = false;
    this.usersApi.getRoles(user.id).subscribe({ next: current => { this.assignedRoleIds = new Set(current.map(x => x.id)); this.cdr.detectChanges(); }, error: e => this.fail(e) });
  }
  changeRole(roleId: string, checked: boolean): void { checked ? this.assignedRoleIds.add(roleId) : this.assignedRoleIds.delete(roleId); }
  saveRoles(): void { if (!this.selected) return; this.usersApi.replaceRoles(this.selected.id, { roleIds: [...this.assignedRoleIds] }).subscribe({ next: () => { this.done('admin.users.rolesUpdated'); this.cdr.detectChanges(); }, error: e => this.fail(e) }); }
  resetPassword(): void { if (!this.selected || this.newPassword.length < 12) return; this.usersApi.resetPassword(this.selected.id, { newPassword: this.newPassword }).subscribe({ next: () => { this.newPassword = ''; this.showPassword = false; this.done('admin.users.passwordReset'); this.cdr.detectChanges(); }, error: e => this.fail(e) }); }
  private done(messageKey: string): void { this.result.show(this.language.text('admin.common.success'), this.language.text(messageKey), 'success'); }
  private fail(error: any): void { this.result.show(this.language.text('admin.common.requestFailed'), error?.error?.message ?? error?.message ?? this.language.text('admin.common.requestFailedMessage'), 'error'); this.cdr.detectChanges(); }
}
