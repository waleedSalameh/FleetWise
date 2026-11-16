export interface Customer {
  id: number;
  customerName: string;
  phoneNumber: string | null;
}

export interface SaveCustomer {
  customerName: string;
  phoneNumber: string;
}
