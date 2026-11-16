import { Routes } from '@angular/router';
import { Layout } from './layout/layout/layout';

export const routes: Routes = [

  {
    path: '',
    component: Layout,
    children: [

      {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full'
      },

      {
        path: 'dashboard',
        loadComponent: () =>
          import('./pages/dashboard/dashboard')
            .then(m => m.Dashboard)
      },

      {
        path: 'vehicles/company',
        loadComponent: () =>
          import('./pages/vehicles/company-vehicles/company-vehicles')
            .then(m => m.CompanyVehicles)
      },

      {
        path: 'vehicles/external',
        loadComponent: () =>
          import('./pages/vehicles/external-vehicles/external-vehicles')
            .then(m => m.ExternalVehicles)
      },

      {
        path: 'vehicles/trailers',
        loadComponent: () =>
          import('./pages/vehicles/trailers/trailers')
            .then(m => m.TrailerComponent)
      },

      {
        path: 'vehicles/assignments',
        loadComponent: () =>
          import('./pages/vehicles/assignments/vehicle-assignment/vehicle-assignment')
            .then(m => m.VehicleAssignmentComponent)
      },

      {
        path: 'drivers',
        loadComponent: () =>
          import('./pages/drivers/driver/driver')
            .then(m => m.DriverComponent)
      },

      {
        path: 'fuel/records',
        loadComponent: () =>
          import('./pages/fuel/fueling-records/fueling-records')
            .then(m => m.FuelingRecordsComponent)
      },

      {
        path: 'fuel/inventory',
        loadComponent: () =>
          import('./pages/fuel/fuel-inventory/fuel-inventory')
            .then(m => m.FuelInventoryComponent)
      },

      {
        path: 'fuel/reports',
        loadComponent: () =>
          import('./pages/fuel/monthly-fuel-reports/monthly-fuel-reports')
            .then(m => m.MonthlyFuelReportsComponent)
      },

      {
        path: 'maintenance/records',
        loadComponent: () =>
          import('./pages/maintenance/maintenance-records/maintenance-records')
            .then(m => m.MaintenanceRecords)
      },

      {
        path: 'maintenance/oils',
        loadComponent: () =>
          import('./pages/maintenance/oil-inventory/oil-inventory')
            .then(m => m.OilInventoryComponent)
      },

      {
        path: 'maintenance/movements/spare-parts',
        loadComponent: () =>
          import('./pages/maintenance/movements/spare-part-movements/spare-part-movements')
            .then(m => m.SparePartMovementsComponent)
      },
      {
        path: 'maintenance/movements/oils',
        loadComponent: () =>
          import('./pages/maintenance/movements/oil-movements/oil-movements')
            .then(m => m.OilMovementsComponent)
      },

      {
        path: 'maintenance/spare-parts',
        loadComponent: () =>
          import('./pages/maintenance/spare-parts-inventory/spare-parts-inventory')
            .then(m => m.SparePartsInventoryComponent)
      },
      {
        path: 'customers',
        loadComponent: () => import('./pages/customers/customers/customers').then(m => m.CustomersPage)
      },
      {
        path: 'customers/shipping-lines',
        loadComponent: () => import('./pages/customers/shipping-lines/shipping-lines').then(m => m.ShippingLinesPage)
      },
      {
        path: 'customers/delivery-locations',
        loadComponent: () => import('./pages/customers/delivery-locations/delivery-locations').then(m => m.DeliveryLocationsPage)
      },


      { path: 'trips', loadComponent: () => import('./pages/trips/trips').then(m => m.TripsPage) },

      { path: 'containers/import/aqaba', loadComponent: () => import('./pages/containers/incoming-aqaba/incoming-aqaba').then(m => m.IncomingAqabaPage) },
      { path: 'containers/import/external', loadComponent: () => import('./pages/containers/international-incoming/international-incoming').then(m => m.InternationalIncomingPage) },
      { path: 'containers/export/aqaba', loadComponent: () => import('./pages/containers/export-aqaba/export-aqaba').then(m => m.ExportAqabaPage) },
      { path: 'containers/export/external', loadComponent: () => import('./pages/containers/international-export/international-export').then(m => m.InternationalExportPage) },
      { path: 'containers/internal', loadComponent: () => import('./pages/containers/inland-transport/inland-transport').then(m => m.InlandTransportPage) },

      // {
      //   path: 'customers',
      //   loadComponent: () =>
      //     import('./pages/customers/customers')
      //       .then(m => m.Customers)
      // }

    ]
  },

  {
    path: '**',
    redirectTo: 'dashboard'
  }

];
