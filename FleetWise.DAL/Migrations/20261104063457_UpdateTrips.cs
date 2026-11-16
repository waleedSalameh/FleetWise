using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FleetWise.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTrips : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trips_VehicleAssignments_VehicleAssignmentAssignmentId",
                table: "Trips");

            migrationBuilder.DropIndex(
                name: "IX_Trips_VehicleAssignmentAssignmentId",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "EndLocation",
                table: "Trips");

            migrationBuilder.RenameColumn(
                name: "VehicleAssignmentAssignmentId",
                table: "Trips",
                newName: "VehicleSourceType");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Trips",
                newName: "UnloadingDate");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Trips",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "StartLocation",
                table: "Trips",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Trips",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "Destination",
                table: "Trips",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "AssignmentId",
                table: "Trips",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<decimal>(
                name: "AdvancePayment",
                table: "Trips",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DieselCost",
                table: "Trips",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Distance",
                table: "Trips",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "DriverId",
                table: "Trips",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DriverTip",
                table: "Trips",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExportAqabaBookingId",
                table: "Trips",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExportAqabaContainerId",
                table: "Trips",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExternalVehicleId",
                table: "Trips",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IncomingAqabaBillId",
                table: "Trips",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IncomingAqabaContainerId",
                table: "Trips",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InlandTransportId",
                table: "Trips",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InternationalExportId",
                table: "Trips",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InternationalIncomingId",
                table: "Trips",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "LeakageCost",
                table: "Trips",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LoadingDate",
                table: "Trips",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Trips",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OtherExpenses",
                table: "Trips",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PurchasePrice",
                table: "Trips",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SalePrice",
                table: "Trips",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TripType",
                table: "Trips",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "VehicleId",
                table: "Trips",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Trips_AssignmentId",
                table: "Trips",
                column: "AssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Trips_DriverId",
                table: "Trips",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_Trips_ExportAqabaBookingId",
                table: "Trips",
                column: "ExportAqabaBookingId");

            migrationBuilder.CreateIndex(
                name: "IX_Trips_ExportAqabaContainerId",
                table: "Trips",
                column: "ExportAqabaContainerId");

            migrationBuilder.CreateIndex(
                name: "IX_Trips_ExternalVehicleId",
                table: "Trips",
                column: "ExternalVehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_Trips_IncomingAqabaBillId",
                table: "Trips",
                column: "IncomingAqabaBillId");

            migrationBuilder.CreateIndex(
                name: "IX_Trips_IncomingAqabaContainerId",
                table: "Trips",
                column: "IncomingAqabaContainerId");

            migrationBuilder.CreateIndex(
                name: "IX_Trips_InlandTransportId",
                table: "Trips",
                column: "InlandTransportId");

            migrationBuilder.CreateIndex(
                name: "IX_Trips_InternationalExportId",
                table: "Trips",
                column: "InternationalExportId");

            migrationBuilder.CreateIndex(
                name: "IX_Trips_InternationalIncomingId",
                table: "Trips",
                column: "InternationalIncomingId");

            migrationBuilder.CreateIndex(
                name: "IX_Trips_TripType_Status_IsDeleted",
                table: "Trips",
                columns: new[] { "TripType", "Status", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_Trips_VehicleId",
                table: "Trips",
                column: "VehicleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Trips_Drivers_DriverId",
                table: "Trips",
                column: "DriverId",
                principalTable: "Drivers",
                principalColumn: "DriverId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Trips_ExportAqabaBookings_ExportAqabaBookingId",
                table: "Trips",
                column: "ExportAqabaBookingId",
                principalTable: "ExportAqabaBookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Trips_ExportAqabaContainers_ExportAqabaContainerId",
                table: "Trips",
                column: "ExportAqabaContainerId",
                principalTable: "ExportAqabaContainers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Trips_ExternalVehicles_ExternalVehicleId",
                table: "Trips",
                column: "ExternalVehicleId",
                principalTable: "ExternalVehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Trips_IncomingAqabaBills_IncomingAqabaBillId",
                table: "Trips",
                column: "IncomingAqabaBillId",
                principalTable: "IncomingAqabaBills",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Trips_IncomingAqabaContainers_IncomingAqabaContainerId",
                table: "Trips",
                column: "IncomingAqabaContainerId",
                principalTable: "IncomingAqabaContainers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Trips_InlandTransports_InlandTransportId",
                table: "Trips",
                column: "InlandTransportId",
                principalTable: "InlandTransports",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Trips_InternationalExports_InternationalExportId",
                table: "Trips",
                column: "InternationalExportId",
                principalTable: "InternationalExports",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Trips_InternationalIncomings_InternationalIncomingId",
                table: "Trips",
                column: "InternationalIncomingId",
                principalTable: "InternationalIncomings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Trips_VehicleAssignments_AssignmentId",
                table: "Trips",
                column: "AssignmentId",
                principalTable: "VehicleAssignments",
                principalColumn: "AssignmentId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Trips_Vehicles_VehicleId",
                table: "Trips",
                column: "VehicleId",
                principalTable: "Vehicles",
                principalColumn: "VehicleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trips_Drivers_DriverId",
                table: "Trips");

            migrationBuilder.DropForeignKey(
                name: "FK_Trips_ExportAqabaBookings_ExportAqabaBookingId",
                table: "Trips");

            migrationBuilder.DropForeignKey(
                name: "FK_Trips_ExportAqabaContainers_ExportAqabaContainerId",
                table: "Trips");

            migrationBuilder.DropForeignKey(
                name: "FK_Trips_ExternalVehicles_ExternalVehicleId",
                table: "Trips");

            migrationBuilder.DropForeignKey(
                name: "FK_Trips_IncomingAqabaBills_IncomingAqabaBillId",
                table: "Trips");

            migrationBuilder.DropForeignKey(
                name: "FK_Trips_IncomingAqabaContainers_IncomingAqabaContainerId",
                table: "Trips");

            migrationBuilder.DropForeignKey(
                name: "FK_Trips_InlandTransports_InlandTransportId",
                table: "Trips");

            migrationBuilder.DropForeignKey(
                name: "FK_Trips_InternationalExports_InternationalExportId",
                table: "Trips");

            migrationBuilder.DropForeignKey(
                name: "FK_Trips_InternationalIncomings_InternationalIncomingId",
                table: "Trips");

            migrationBuilder.DropForeignKey(
                name: "FK_Trips_VehicleAssignments_AssignmentId",
                table: "Trips");

            migrationBuilder.DropForeignKey(
                name: "FK_Trips_Vehicles_VehicleId",
                table: "Trips");

            migrationBuilder.DropIndex(
                name: "IX_Trips_AssignmentId",
                table: "Trips");

            migrationBuilder.DropIndex(
                name: "IX_Trips_DriverId",
                table: "Trips");

            migrationBuilder.DropIndex(
                name: "IX_Trips_ExportAqabaBookingId",
                table: "Trips");

            migrationBuilder.DropIndex(
                name: "IX_Trips_ExportAqabaContainerId",
                table: "Trips");

            migrationBuilder.DropIndex(
                name: "IX_Trips_ExternalVehicleId",
                table: "Trips");

            migrationBuilder.DropIndex(
                name: "IX_Trips_IncomingAqabaBillId",
                table: "Trips");

            migrationBuilder.DropIndex(
                name: "IX_Trips_IncomingAqabaContainerId",
                table: "Trips");

            migrationBuilder.DropIndex(
                name: "IX_Trips_InlandTransportId",
                table: "Trips");

            migrationBuilder.DropIndex(
                name: "IX_Trips_InternationalExportId",
                table: "Trips");

            migrationBuilder.DropIndex(
                name: "IX_Trips_InternationalIncomingId",
                table: "Trips");

            migrationBuilder.DropIndex(
                name: "IX_Trips_TripType_Status_IsDeleted",
                table: "Trips");

            migrationBuilder.DropIndex(
                name: "IX_Trips_VehicleId",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "AdvancePayment",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "DieselCost",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "Distance",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "DriverId",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "DriverTip",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "ExportAqabaBookingId",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "ExportAqabaContainerId",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "ExternalVehicleId",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "IncomingAqabaBillId",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "IncomingAqabaContainerId",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "InlandTransportId",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "InternationalExportId",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "InternationalIncomingId",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "LeakageCost",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "LoadingDate",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "OtherExpenses",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "PurchasePrice",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "SalePrice",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "TripType",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "VehicleId",
                table: "Trips");

            migrationBuilder.RenameColumn(
                name: "VehicleSourceType",
                table: "Trips",
                newName: "VehicleAssignmentAssignmentId");

            migrationBuilder.RenameColumn(
                name: "UnloadingDate",
                table: "Trips",
                newName: "Date");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Trips",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "StartLocation",
                table: "Trips",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Trips",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Destination",
                table: "Trips",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<int>(
                name: "AssignmentId",
                table: "Trips",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EndLocation",
                table: "Trips",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Trips_VehicleAssignmentAssignmentId",
                table: "Trips",
                column: "VehicleAssignmentAssignmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Trips_VehicleAssignments_VehicleAssignmentAssignmentId",
                table: "Trips",
                column: "VehicleAssignmentAssignmentId",
                principalTable: "VehicleAssignments",
                principalColumn: "AssignmentId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
