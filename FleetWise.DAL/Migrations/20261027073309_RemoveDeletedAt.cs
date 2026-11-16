using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FleetWise.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDeletedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "VehicleAssignments");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "MaintenanceRecords");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "FuelRecords");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Drivers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Vehicles",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "VehicleAssignments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "MaintenanceRecords",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "FuelRecords",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Drivers",
                type: "datetime2",
                nullable: true);
        }
    }
}
