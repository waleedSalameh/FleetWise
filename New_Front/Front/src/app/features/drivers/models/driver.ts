export interface Driver {
  driverId: number;
  fullName: string;
  phone: string;
  licenseNumber: string;
  licenseExpiryDate: string;
  status: number;
  isDeleted: boolean;
}

export interface CreateDriver {
  fullName: string;
  phone: string;
  licenseNumber: string;
  licenseExpiryDate: string;
  status: number;
}

export interface UpdateDriver {
  fullName: string;
  phone: string;
  licenseNumber: string;
  licenseExpiryDate: string;
  status: number;
}

export enum DriverStatus {
  Active = 0,
  Inactive = 1,
  Suspended = 2
}
