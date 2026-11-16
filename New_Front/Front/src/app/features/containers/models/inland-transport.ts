export interface InlandTransportSave { customerId: number; tripStatus: number; cargoType: number; weight: number; loadingLocation: string; unloadingLocation: string; notes: string | null; }
export interface InlandTransport extends InlandTransportSave { id: number; tripCode: string; customerName: string; createdAt: string; }
