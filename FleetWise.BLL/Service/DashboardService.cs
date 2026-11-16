using FleetWise.BLL.DTOs.Dashboard;
using FleetWise.DAL.Data;
using FleetWise.DAL.Entities;
using FleetWise.DAL.Entities.Vehicles;
using Microsoft.EntityFrameworkCore;
using static FleetWise.DAL.Entities.Trip;

namespace FleetWise.BLL.Services
{
    public class DashboardService
    {
        private readonly FleetWiseDbContext _context;

        public DashboardService(FleetWiseDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardDto> GetDashboardAsync()
        {
            var dashboard = new DashboardDto();


            dashboard.TotalVehicles = await _context.Vehicles
                .CountAsync(v => !v.IsDeleted);

            dashboard.TotalDrivers = await _context.Drivers
                .CountAsync(d => !d.IsDeleted);

            dashboard.TotalTrips = await _context.Trips
                .CountAsync(t => !t.IsDeleted);

            dashboard.TotalAssignments = await _context.VehicleAssignments
                .CountAsync(a => !a.IsDeleted);



            dashboard.AvailableVehicles = await _context.Vehicles
                .CountAsync(v => !v.IsDeleted &&
                    v.Status == VehicleStatus.Available);

            dashboard.InUseVehicles = await _context.Vehicles
                .CountAsync(v => !v.IsDeleted &&
                    v.Status == VehicleStatus.InUse);

            dashboard.VehiclesUnderMaintenance = await _context.Vehicles
                .CountAsync(v => !v.IsDeleted &&
                    v.Status == VehicleStatus.UnderMaintenance);

            dashboard.InactiveVehicles = await _context.Vehicles
                .CountAsync(v => !v.IsDeleted &&
                    v.Status == VehicleStatus.Inactive);


         
            dashboard.ActiveDrivers = await _context.Drivers
                .CountAsync(d => !d.IsDeleted &&
                    d.Status == DriverStatus.Active);

            dashboard.InactiveDrivers = await _context.Drivers
                .CountAsync(d => !d.IsDeleted &&
                    d.Status == DriverStatus.Inactive);

            dashboard.SuspendedDrivers = await _context.Drivers
                .CountAsync(d => !d.IsDeleted &&
                    d.Status == DriverStatus.Suspended);
            dashboard.PlannedTrips = await _context.Trips
           .CountAsync(t => !t.IsDeleted &&
                 t.Status == TripStatus.Planned);




            dashboard.InProgressTrips = await _context.Trips
                .CountAsync(t => !t.IsDeleted &&
                    t.Status == TripStatus.InProgress);

            dashboard.CompletedTrips = await _context.Trips
                .CountAsync(t => !t.IsDeleted &&
                    t.Status == TripStatus.Completed);

            dashboard.CancelledTrips = await _context.Trips
                .CountAsync(t => !t.IsDeleted &&
                    t.Status == TripStatus.Cancelled);


         

            var fuelRecords = _context.FuelRecords
                .Where(f => !f.IsDeleted);

            dashboard.TotalFuelQuantity = await fuelRecords
                .SumAsync(f => (decimal?)f.Quantity) ?? 0;

            dashboard.TotalFuelCost = await fuelRecords
                .SumAsync(f => (decimal?)(f.Quantity * f.PricePerLiter)) ?? 0;



            var maintenanceRecords = _context.MaintenanceRecords
                .Where(m => !m.IsDeleted);

            dashboard.TotalMaintenanceRecords =
                await maintenanceRecords.CountAsync();

            

            return dashboard;
        }
    }
}