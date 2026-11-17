import { Routes } from '@angular/router';
import { Layout } from './layout/layout/layout';
import { authGuard } from './features/auth/guards/auth.guard';
import { guestGuard } from './features/auth/guards/guest.guard';
import { permissionGuard } from './features/auth/guards/permission.guard';

export const routes: Routes = [

  {
    path: 'login',
    canActivate: [guestGuard],
    loadComponent: () =>
      import('./pages/auth/login/login')
        .then(m => m.LoginPage)
  },
  {
    path: '',
    component: Layout,
    canActivate: [authGuard],
    canActivateChild: [authGuard],
    children: [

      {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full'
      },

      {
        path: 'forbidden',
        loadComponent: () =>
          import('./pages/forbidden/forbidden').then(m => m.ForbiddenPage)
      },

      {
        path: 'dashboard',
        canActivate: [permissionGuard],
        data: { permissions: 'Dashboard.View' },
        loadComponent: () =>
          import('./pages/dashboard/dashboard')
            .then(m => m.Dashboard)
      },

      {
        path: 'vehicles/company',
        canActivate: [permissionGuard],
        data: { permissions: 'Vehicles.View' },
        loadComponent: () =>
          import('./pages/vehicles/company-vehicles/company-vehicles')
            .then(m => m.CompanyVehicles)
      },

      {
        path: 'vehicles/external',
        canActivate: [permissionGuard],
        data: { permissions: 'ExternalVehicles.View' },
        loadComponent: () =>
          import('./pages/vehicles/external-vehicles/external-vehicles')
            .then(m => m.ExternalVehicles)
      },

      {
        path: 'vehicles/trailers',
        canActivate: [permissionGuard],
        data: { permissions: 'Trailers.View' },
        loadComponent: () =>
          import('./pages/vehicles/trailers/trailers')
            .then(m => m.TrailerComponent)
      },

      {
        path: 'vehicles/assignments',
        canActivate: [permissionGuard],
        data: { permissions: 'VehicleAssignments.View' },
        loadComponent: () =>
          import('./pages/vehicles/assignments/vehicle-assignment/vehicle-assignment')
            .then(m => m.VehicleAssignmentComponent)
      },

      {
        path: 'drivers',
        canActivate: [permissionGuard],
        data: { permissions: 'Drivers.View' },
        loadComponent: () =>
          import('./pages/drivers/driver/driver')
            .then(m => m.DriverComponent)
      },

      {
        path: 'fuel/records',
        canActivate: [permissionGuard],
        data: { permissions: 'FuelRecords.View' },
        loadComponent: () =>
          import('./pages/fuel/fueling-records/fueling-records')
            .then(m => m.FuelingRecordsComponent)
      },

      {
        path: 'fuel/inventory',
        canActivate: [permissionGuard],
        data: {
          permissions: ['FuelPurchases.View', 'FuelTransfers.View'],
          permissionMatch: 'all'
        },
        loadComponent: () =>
          import('./pages/fuel/fuel-inventory/fuel-inventory')
            .then(m => m.FuelInventoryComponent)
      },

      {
        path: 'fuel/reports',
        canActivate: [permissionGuard],
        data: { permissions: 'FuelReports.View' },
        loadComponent: () =>
          import('./pages/fuel/monthly-fuel-reports/monthly-fuel-reports')
            .then(m => m.MonthlyFuelReportsComponent)
      },

      {
        path: 'maintenance/records',
        canActivate: [permissionGuard],
        data: { permissions: 'Maintenance.View' },
        loadComponent: () =>
          import('./pages/maintenance/maintenance-records/maintenance-records')
            .then(m => m.MaintenanceRecords)
      },

      {
        path: 'maintenance/oils',
        canActivate: [permissionGuard],
        data: { permissions: 'Oils.View' },
        loadComponent: () =>
          import('./pages/maintenance/oil-inventory/oil-inventory')
            .then(m => m.OilInventoryComponent)
      },

      {
        path: 'maintenance/movements/spare-parts',
        canActivate: [permissionGuard],
        data: { permissions: 'SparePartMovements.View' },
        loadComponent: () =>
          import('./pages/maintenance/movements/spare-part-movements/spare-part-movements')
            .then(m => m.SparePartMovementsComponent)
      },
      {
        path: 'maintenance/movements/oils',
        canActivate: [permissionGuard],
        data: { permissions: 'OilMovements.View' },
        loadComponent: () =>
          import('./pages/maintenance/movements/oil-movements/oil-movements')
            .then(m => m.OilMovementsComponent)
      },

      {
        path: 'maintenance/spare-parts',
        canActivate: [permissionGuard],
        data: { permissions: 'SpareParts.View' },
        loadComponent: () =>
          import('./pages/maintenance/spare-parts-inventory/spare-parts-inventory')
            .then(m => m.SparePartsInventoryComponent)
      },
      {
        path: 'customers',
        canActivate: [permissionGuard],
        data: { permissions: 'Customers.View' },
        loadComponent: () => import('./pages/customers/customers/customers').then(m => m.CustomersPage)
      },
      {
        path: 'customers/shipping-lines',
        canActivate: [permissionGuard],
        data: { permissions: 'ShippingLines.View' },
        loadComponent: () => import('./pages/customers/shipping-lines/shipping-lines').then(m => m.ShippingLinesPage)
      },
      {
        path: 'customers/delivery-locations',
        canActivate: [permissionGuard],
        data: { permissions: 'DeliveryLocations.View' },
        loadComponent: () => import('./pages/customers/delivery-locations/delivery-locations').then(m => m.DeliveryLocationsPage)
      },


      { path: 'trips', canActivate: [permissionGuard], data: { permissions: 'Trips.View' }, loadComponent: () => import('./pages/trips/trips').then(m => m.TripsPage) },

      { path: 'containers/import/aqaba', canActivate: [permissionGuard], data: { permissions: ['IncomingAqabaBills.View', 'IncomingAqabaContainers.View'], permissionMatch: 'all' }, loadComponent: () => import('./pages/containers/incoming-aqaba/incoming-aqaba').then(m => m.IncomingAqabaPage) },
      { path: 'containers/import/external', canActivate: [permissionGuard], data: { permissions: 'InternationalIncoming.View' }, loadComponent: () => import('./pages/containers/international-incoming/international-incoming').then(m => m.InternationalIncomingPage) },
      { path: 'containers/export/aqaba', canActivate: [permissionGuard], data: { permissions: ['ExportAqabaBookings.View', 'ExportAqabaContainers.View'], permissionMatch: 'all' }, loadComponent: () => import('./pages/containers/export-aqaba/export-aqaba').then(m => m.ExportAqabaPage) },
      { path: 'containers/export/external', canActivate: [permissionGuard], data: { permissions: 'InternationalExports.View' }, loadComponent: () => import('./pages/containers/international-export/international-export').then(m => m.InternationalExportPage) },
      { path: 'containers/internal', canActivate: [permissionGuard], data: { permissions: 'InlandTransport.View' }, loadComponent: () => import('./pages/containers/inland-transport/inland-transport').then(m => m.InlandTransportPage) },

      { path: 'admin/users', canActivate: [permissionGuard], data: { permissions: 'Users.View' }, loadComponent: () => import('./pages/admin/users/users').then(m => m.UsersPage) },
      { path: 'admin/roles', canActivate: [permissionGuard], data: { permissions: 'Roles.Manage' }, loadComponent: () => import('./pages/admin/roles/roles').then(m => m.RolesPage) },
      { path: 'admin/permissions', canActivate: [permissionGuard], data: { permissions: 'Permissions.Manage' }, loadComponent: () => import('./pages/admin/permissions/permissions').then(m => m.PermissionsPage) },

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
