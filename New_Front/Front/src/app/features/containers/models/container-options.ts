export const billStatuses = [{ value: 1, label: 'New' }, { value: 2, label: 'In Progress' }, { value: 3, label: 'Completed' }, { value: 4, label: 'Cancelled' }];
export const orderStatuses = billStatuses;
export const inlandStatuses = billStatuses;
export const containerTypes = [{ value: 1, label: 'Refrigerated' }, { value: 2, label: 'Dry' }, { value: 3, label: 'Open Top' }, { value: 4, label: 'Flat Rack' }];
export const containerSizes = [{ value: 1, label: '20 ft' }, { value: 2, label: '40 ft' }, { value: 3, label: '45 ft' }];
export const cargoTypes = [{ value: 1, label: 'Flatbed' }, { value: 2, label: 'Refrigerated' }, { value: 3, label: 'Sides' }, { value: 4, label: 'Curtainsider' }];
export const aqabaStatuses = [{ value: 1, label: 'Released' }, { value: 2, label: 'Inspection' }, { value: 3, label: 'Amman Customs' }, { value: 4, label: 'Aqaba Customs' }, { value: 5, label: 'Free Zone' }, { value: 6, label: 'Inspection Report' }];
export const routeStatuses = [{ value: 1, label: 'Red' }, { value: 2, label: 'Yellow' }, { value: 3, label: 'Green' }];
export function optionLabel(options: readonly {value: number; label: string}[], value: number): string { return options.find(option => option.value === value)?.label ?? String(value); }
