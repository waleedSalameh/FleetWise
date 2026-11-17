/** Matches FleetWise.BLL.DTOs.Identity.LoginRequestDto. */
export interface LoginRequest {
  email: string;
  password: string;
}

/** Matches FleetWise.BLL.DTOs.Identity.LoginResponseDto. */
export interface LoginResponse {
  accessToken: string;
  expiresAtUtc: string;
}

/** Display-only identity parsed from the access token issued by FleetWise. */
export interface AuthenticatedUser {
  id: string | null;
  email: string | null;
  roles: string[];
}
