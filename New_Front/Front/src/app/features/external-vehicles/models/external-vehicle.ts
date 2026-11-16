export interface ExternalVehicle {
  id: number;
  driverName: string;
  plateNumber: number;
  phoneNumber: string;
}

export interface CreateExternalVehicle {
  driverName: string;
  plateNumber: number;
  phoneNumber: string;
}
