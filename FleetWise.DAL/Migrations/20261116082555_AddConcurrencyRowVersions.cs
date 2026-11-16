using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FleetWise.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddConcurrencyRowVersions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Vehicles",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "VehicleFuelMeasurements",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "VehicleFuelLedgerEntries",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "VehicleAssignments",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Trips",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Trailers",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "SpareParts",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "SparePartMovements",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "ShippingLines",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "OilStockMovements",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Oils",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "MonthlyFuelClosings",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "MaintenanceRecords",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "InternationalIncomings",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "InternationalExports",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "InlandTransports",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "IncomingAqabaContainers",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "IncomingAqabaBills",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "FuelTransfers",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "FuelTransferDetails",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "FuelRecords",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "fuelRecordConsumptions",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "FuelPurchases",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "ExternalVehicles",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "ExportAqabaContainers",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "ExportAqabaBookings",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Drivers",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "DeliveryLocations",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Customers",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "VehicleFuelMeasurements");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "VehicleFuelLedgerEntries");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "VehicleAssignments");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Trailers");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "SpareParts");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "SparePartMovements");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "ShippingLines");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "OilStockMovements");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Oils");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "MonthlyFuelClosings");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "MaintenanceRecords");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "InternationalIncomings");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "InternationalExports");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "InlandTransports");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "IncomingAqabaContainers");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "IncomingAqabaBills");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "FuelTransfers");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "FuelTransferDetails");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "FuelRecords");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "fuelRecordConsumptions");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "FuelPurchases");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "ExternalVehicles");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "ExportAqabaContainers");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "ExportAqabaBookings");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "DeliveryLocations");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Customers");
        }
    }
}
