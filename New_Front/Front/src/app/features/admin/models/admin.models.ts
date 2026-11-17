/** Matches FleetWise.BLL.DTOs.Identity.UserSummaryDto. */
export interface UserSummary {
  id: string;
  email: string | null;
  isActive: boolean;
}

/** Matches FleetWise.BLL.DTOs.Identity.CreateUserDto. */
export interface CreateUserRequest {
  email: string;
  temporaryPassword: string;
}

/** Matches FleetWise.BLL.DTOs.Identity.ResetUserPasswordDto. */
export interface ResetUserPasswordRequest {
  newPassword: string;
}

/** Matches FleetWise.BLL.DTOs.Identity.RoleSummaryDto. */
export interface RoleSummary {
  id: string;
  name: string;
  userCount: number;
  permissionCount: number;
}

/** Matches FleetWise.BLL.DTOs.Identity.CreateRoleDto. */
export interface CreateRoleRequest {
  name: string;
}

/** Matches FleetWise.BLL.DTOs.Identity.UpdateRoleDto. */
export interface UpdateRoleRequest {
  name: string;
}

/** Matches FleetWise.BLL.DTOs.Identity.RolePermissionStatusDto. */
export interface RolePermissionStatus {
  permissionCode: string;
  name: string;
  module: string;
  isAssigned: boolean;
}

/** Matches FleetWise.BLL.DTOs.Identity.ReplaceRolePermissionsDto. */
export interface ReplaceRolePermissionsRequest {
  permissionCodes: string[];
}

/** Matches FleetWise.BLL.DTOs.Identity.ReplaceUserRolesDto. */
export interface ReplaceUserRolesRequest {
  roleIds: string[];
}

/** Matches FleetWise.BLL.DTOs.Identity.UserPermissionStatusDto. */
export interface UserPermissionStatus {
  permissionCode: string;
  name: string;
  module: string;
  /** null means this permission is inherited from one of the user's roles. */
  userOverride: boolean | null;
  isEffectivelyAllowed: boolean;
}

/** Matches FleetWise.BLL.DTOs.Identity.SetUserPermissionOverrideDto. */
export interface SetUserPermissionOverrideRequest {
  permissionCode: string;
  isAllowed: boolean;
}

