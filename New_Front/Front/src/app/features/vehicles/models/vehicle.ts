export enum VehicleStatus {
  Available = 0,
  InUse = 1,
  UnderMaintenance = 2,
  Inactive = 3
}

export interface Vehicle {
  vehicleId: number;
  plateNumber: number;
  make: string;
  year: number;
  model: string;
  currentOdometer: number;
  fuelTankCapacity: number;
  status: VehicleStatus;
  isDeleted: boolean;
}

export interface CreateVehicle {
  plateNumber: number;
  make: string;
  year: number;
  model: string;
  currentOdometer: number;
  fuelTankCapacity: number;
  status: VehicleStatus;
}

export interface UpdateVehicle {
  plateNumber: number;
  make: string;
  year: number;
  model: string;
  currentOdometer: number;
  fuelTankCapacity: number;
  status: VehicleStatus;
}
