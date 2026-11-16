using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FleetWise.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMaintenance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MaintenanceRecords_Date",
                table: "MaintenanceRecords");

            migrationBuilder.DropColumn(
                name: "Cost",
                table: "MaintenanceRecords");

            migrationBuilder.DropColumn(
                name: "NextMaintenanceDate",
                table: "MaintenanceRecords");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "MaintenanceRecords",
                newName: "MaintenanceDate");

            migrationBuilder.RenameColumn(
                name: "MaintenanceId",
                table: "MaintenanceRecords",
                newName: "MaintenanceRecordId");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "MaintenanceRecords",
                type: "int",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "MaintenanceRecords",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "LaborCost",
                table: "MaintenanceRecords",
                type: "decimal(18,3)",
                precision: 18,
                scale: 3,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PartsCost",
                table: "MaintenanceRecords",
                type: "decimal(18,3)",
                precision: 18,
                scale: 3,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "TechnicianName",
                table: "MaintenanceRecords",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalCost",
                table: "MaintenanceRecords",
                type: "decimal(18,3)",
                precision: 18,
                scale: 3,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "VehicleId1",
                table: "MaintenanceRecords",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WorkshopName",
                table: "MaintenanceRecords",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceRecords_VehicleId1",
                table: "MaintenanceRecords",
                column: "VehicleId1");

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceRecords_Vehicles_VehicleId1",
                table: "MaintenanceRecords",
                column: "VehicleId1",
                principalTable: "Vehicles",
                principalColumn: "VehicleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceRecords_Vehicles_VehicleId1",
                table: "MaintenanceRecords");

            migrationBuilder.DropIndex(
                name: "IX_MaintenanceRecords_VehicleId1",
                table: "MaintenanceRecords");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "MaintenanceRecords");

            migrationBuilder.DropColumn(
                name: "LaborCost",
                table: "MaintenanceRecords");

            migrationBuilder.DropColumn(
                name: "PartsCost",
                table: "MaintenanceRecords");

            migrationBuilder.DropColumn(
                name: "TechnicianName",
                table: "MaintenanceRecords");

            migrationBuilder.DropColumn(
                name: "TotalCost",
                table: "MaintenanceRecords");

            migrationBuilder.DropColumn(
                name: "VehicleId1",
                table: "MaintenanceRecords");

            migrationBuilder.DropColumn(
                name: "WorkshopName",
                table: "MaintenanceRecords");

            migrationBuilder.RenameColumn(
                name: "MaintenanceDate",
                table: "MaintenanceRecords",
                newName: "Date");

            migrationBuilder.RenameColumn(
                name: "MaintenanceRecordId",
                table: "MaintenanceRecords",
                newName: "MaintenanceId");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "MaintenanceRecords",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 1);

            migrationBuilder.AddColumn<decimal>(
                name: "Cost",
                table: "MaintenanceRecords",
                type: "decimal(12,2)",
                precision: 12,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "NextMaintenanceDate",
                table: "MaintenanceRecords",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceRecords_Date",
                table: "MaintenanceRecords",
                column: "Date");
        }
    }
}
