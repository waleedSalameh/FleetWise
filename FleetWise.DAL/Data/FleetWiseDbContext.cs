using FleetWise.DAL.Configurations.Vehicles;
using FleetWise.DAL.Entities;
using FleetWise.DAL.Entities.Containers;
using FleetWise.DAL.Entities.Containers.ExportAqaba;
using FleetWise.DAL.Entities.Containers.IncomingAqaba;
using FleetWise.DAL.Entities.Containers.InlandTransport;
using FleetWise.DAL.Entities.Containers.InternationalExport;
using FleetWise.DAL.Entities.Containers.InternationalIncoming;
using FleetWise.DAL.Entities.Customers;
using FleetWise.DAL.Entities.Fuel;
using FleetWise.DAL.Entities.Maintenance;
using FleetWise.DAL.Entities.Maintenance.FleetWise.DAL.Entities.Maintenance;
using FleetWise.DAL.Entities.Vehicles;
using Microsoft.EntityFrameworkCore;

namespace FleetWise.DAL.Data
{
    public class FleetWiseDbContext:DbContext
    {
        public FleetWiseDbContext(DbContextOptions<FleetWiseDbContext> options): base(options)
        {
        }

        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<VehicleAssignment> VehicleAssignments { get; set; }
        public DbSet<MaintenanceRecord> MaintenanceRecords { get; set; }
        public DbSet<FuelRecord> FuelRecords { get; set; }
        public DbSet<Trip> Trips { get; set; }
        public DbSet<IncomingAqabaBill> IncomingAqabaBills { get; set; }
        public DbSet<IncomingAqabaContainer> IncomingAqabaContainers { get; set; }
        public DbSet<ShippingLine> ShippingLines { get; set; }
        public DbSet<DeliveryLocation> DeliveryLocations { get; set; }
        public DbSet<ExportAqabaBooking> ExportAqabaBookings { get; set; }
        public DbSet<ExportAqabaContainer> ExportAqabaContainers { get; set; }
        public DbSet<InternationalIncoming>InternationalIncomings { get; set; }
        public DbSet<Customer>Customers { get; set; }
        public DbSet<InternationalExport> InternationalExports { get; set; }
        public DbSet<InlandTransport> InlandTransports { get; set; }
        public DbSet<ExternalVehicles> ExternalVehicles { get; set; }
        public DbSet<Trailer> Trailers { get; set; }
        public DbSet<FuelPurchase> FuelPurchases { get; set; }
        public DbSet<FuelRecordConsumption> fuelRecordConsumptions { get; set; }
        public DbSet<FuelTransfer> FuelTransfers { get; set; }
        public DbSet<FuelTransferDetail> FuelTransferDetails { get; set; }
        public DbSet<VehicleFuelLedgerEntry> VehicleFuelLedgerEntries { get; set; }
        public DbSet<VehicleFuelMeasurement> VehicleFuelMeasurements { get; set; }
        public DbSet<MonthlyFuelClosing> MonthlyFuelClosings { get; set; }
        public DbSet<Oil> Oils { get; set; }
        public DbSet<OilStockMovement> OilStockMovements { get; set; }
        public DbSet<SparePart> SpareParts { get; set; }
        public DbSet<SparePartMovement> SparePartMovements { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(VehicleConfiguration).Assembly);

            // Detect overlapping updates consistently across persisted FleetWise entities.
            foreach (var entityType in modelBuilder.Model.GetEntityTypes()
                         .Where(entityType =>
                             !entityType.IsOwned() &&
                             !entityType.HasSharedClrType &&
                             entityType.FindPrimaryKey() is not null))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .Property<byte[]>("RowVersion")
                    .IsRequired()
                    .IsRowVersion();
            }

           
        }

      
    }

}

