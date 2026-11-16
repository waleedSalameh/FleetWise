export interface VehicleAssignment {
  assignmentId: number;
  vehicleId: number;
  trailerId: number;
  startDate: string;
  endDate: string | null;
  plateNumber: number;
  isDeleted: boolean;
}

export interface CreateVehicleAssignment {
  vehicleId: number;
  trailerId: number;
  startDate: string;
  endDate: string | null;
}

export interface UpdateVehicleAssignment {
  vehicleId: number;
  trailerId: number;
  startDate: string;
  endDate: string | null;
}
