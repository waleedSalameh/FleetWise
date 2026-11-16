export enum FuelTransferDestinationType { Vehicle = 1, Inventory = 2 }
export enum FuelMeasurementType { Opening = 1, Closing = 2, Correction = 3 }
export enum FuelApprovalStatus { Draft = 1, Approved = 2, Reversed = 3 }

export interface FuelTransferDestinationInput { destinationType: FuelTransferDestinationType; destinationVehicleId: number | null; quantity: number; }
export interface CreateFuelTransfer { sourceVehicleId: number; occurredAt: string; notes: string | null; destinations: FuelTransferDestinationInput[]; }
export interface FuelTransferDetail { id: number; destinationType: FuelTransferDestinationType; destinationVehicleId: number | null; destinationPlateNumber: number | null; quantity: number; unitCostJod: number; totalCostJod: number; returnedFuelPurchaseId: number | null; }
export interface FuelTransfer { id: number; sourceVehicleId: number; sourcePlateNumber: number; occurredAt: string; notes: string | null; destinations: FuelTransferDetail[]; }

export interface CreateFuelMeasurement { vehicleId: number; measuredAt: string; measuredQuantity: number; measuredValueJod: number | null; measurementType: FuelMeasurementType; notes: string | null; }
export interface FuelMeasurement extends CreateFuelMeasurement { id: number; plateNumber: number; status: FuelApprovalStatus; approvedBy: string | null; approvedAt: string | null; }

export interface MonthlyFuelReport { vehicleId: number; plateNumber: number; periodStart: string; periodEnd: string; isComplete: boolean; incompleteReason: string | null; openingFuelQuantity: number; openingFuelValueJod: number; closingFuelQuantity: number; closingFuelValueJod: number; totalFuelingQuantity: number; totalFuelingCostJod: number; internalFuelingQuantity: number; internalFuelingCostJod: number; externalFuelingQuantity: number; externalFuelingCostJod: number; incomingTransferQuantity: number; incomingTransferValueJod: number; outgoingTransferQuantity: number; outgoingTransferValueJod: number; actualConsumptionLiters: number; actualConsumptionCostJod: number; netFuelValueAllocatedJod: number; internalFuelingCount: number; externalFuelingCount: number; incomingTransferCount: number; outgoingTransferCount: number; totalFuelingCount: number; totalTransferCount: number; }
export interface FuelTransaction { transactionId: string; transactionType: string; date: string; source: string; destination: string; quantity: number; unitCostJod: number; totalCostJod: number; relatedVehicleId: number | null; fuelPurchaseId: number | null; }
