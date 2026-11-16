export const tripStatuses = [
  { value: 0, label: 'Planned' },
  { value: 1, label: 'In Progress' },
  { value: 2, label: 'Completed' },
  { value: 3, label: 'Cancelled' },
] as const;

export const tripTypes = [
  { value: 1, label: 'Aqaba Import' },
  { value: 2, label: 'Aqaba Export' },
  { value: 3, label: 'International Import' },
  { value: 4, label: 'International Export' },
  { value: 5, label: 'Inland Transport' },
] as const;

export enum VehicleSourceType {
  Internal = 1,
  External = 2,
}

export const vehicleSources = [
  { value: VehicleSourceType.Internal, label: 'Company' },
  { value: VehicleSourceType.External, label: 'External' },
] as const;

export const collectionMethods = [
  { value: 1, label: 'With Driver' },
  { value: 2, label: 'Check' },
  { value: 3, label: 'Deferred' },
  { value: 4, label: 'Prepaid' },
] as const;

export function tripOptionLabel(
  options: readonly { value: number; label: string }[],
  value: number | null,
): string {
  return value === null
    ? '—'
    : (options.find((option) => option.value === value)?.label ?? String(value));
}

export interface TripSave {
  vehicleSourceType: number;
  assignmentId: number | null;
  driverId: number | null;
  externalVehicleId: number | null;
  tripType: number;
  collectionMethod: number | null;
  incomingAqabaBillId: number | null;
  incomingAqabaContainerId: number | null;
  exportAqabaBookingId: number | null;
  exportAqabaContainerId: number | null;
  internationalIncomingId: number | null;
  internationalExportId: number | null;
  inlandTransportId: number | null;
  startLocation: string;
  destination: string;
  loadingDate: string;
  unloadingDate: string;
  distance: number;
  driverTip: number | null;
  dieselCost: number | null;
  leakageCost: number | null;
  otherExpenses: number | null;
  purchasePrice: number | null;
  salePrice: number | null;
  advancePayment: number | null;
  notes: string | null;
}

export interface UpdateTrip extends TripSave {
  status: number;
}

export interface Trip extends UpdateTrip {
  tripId: number;
  driverName: string | null;
  externalVehiclePlateNumber: number | null;
  externalDriverName: string | null;
  billNumber: string | null;
  incomingContainerNumber: string | null;
  bookingNumber: string | null;
  exportContainerNumber: string | null;
  internationalIncomingCargoNumber: string | null;
  internationalExportCargoNumber: string | null;
  inlandTransportTripCode: string | null;
}
