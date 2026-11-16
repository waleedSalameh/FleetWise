using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FleetWise.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddSoftDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Vehicles_PlateNumber",
                table: "Vehicles");

            migrationBuilder.RenameColumn(
                name: "Yaer",
                table: "Vehicles",
                newName: "Year");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Vehicles",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Vehicles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "VehicleAssignments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "VehicleAssignments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "MaintenanceRecords",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "MaintenanceRecords",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "FuelRecords",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "FuelRecords",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Drivers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Drivers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_Year",
                table: "Vehicles",
                column: "Year",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Vehicles_Year",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "VehicleAssignments");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "VehicleAssignments");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "MaintenanceRecords");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "MaintenanceRecords");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "FuelRecords");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "FuelRecords");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Drivers");

            migrationBuilder.RenameColumn(
                name: "Year",
                table: "Vehicles",
                newName: "Yaer");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_PlateNumber",
                table: "Vehicles",
                column: "PlateNumber",
                unique: true);
        }
    }
}
