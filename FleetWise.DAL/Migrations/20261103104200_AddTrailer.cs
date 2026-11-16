using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FleetWise.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddTrailer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VehicleAssignments_Drivers_DriverId",
                table: "VehicleAssignments");

            migrationBuilder.AlterColumn<int>(
                name: "DriverId",
                table: "VehicleAssignments",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "TrailerId",
                table: "VehicleAssignments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TrailerId1",
                table: "VehicleAssignments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Trailers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlateNumber = table.Column<int>(type: "int", nullable: false),
                    AxleCount = table.Column<int>(type: "int", nullable: false),
                    MaxLoad = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TrailerType = table.Column<int>(type: "int", nullable: false),
                    LicenseExpiryDate = table.Column<DateOnly>(type: "date", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trailers", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VehicleAssignments_TrailerId",
                table: "VehicleAssignments",
                column: "TrailerId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleAssignments_TrailerId1",
                table: "VehicleAssignments",
                column: "TrailerId1");

            migrationBuilder.CreateIndex(
                name: "IX_Trailers_PlateNumber",
                table: "Trailers",
                column: "PlateNumber",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_VehicleAssignments_Drivers_DriverId",
                table: "VehicleAssignments",
                column: "DriverId",
                principalTable: "Drivers",
                principalColumn: "DriverId");

            migrationBuilder.AddForeignKey(
                name: "FK_VehicleAssignments_Trailers_TrailerId",
                table: "VehicleAssignments",
                column: "TrailerId",
                principalTable: "Trailers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VehicleAssignments_Trailers_TrailerId1",
                table: "VehicleAssignments",
                column: "TrailerId1",
                principalTable: "Trailers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VehicleAssignments_Drivers_DriverId",
                table: "VehicleAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_VehicleAssignments_Trailers_TrailerId",
                table: "VehicleAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_VehicleAssignments_Trailers_TrailerId1",
                table: "VehicleAssignments");

            migrationBuilder.DropTable(
                name: "Trailers");

            migrationBuilder.DropIndex(
                name: "IX_VehicleAssignments_TrailerId",
                table: "VehicleAssignments");

            migrationBuilder.DropIndex(
                name: "IX_VehicleAssignments_TrailerId1",
                table: "VehicleAssignments");

            migrationBuilder.DropColumn(
                name: "TrailerId",
                table: "VehicleAssignments");

            migrationBuilder.DropColumn(
                name: "TrailerId1",
                table: "VehicleAssignments");

            migrationBuilder.AlterColumn<int>(
                name: "DriverId",
                table: "VehicleAssignments",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_VehicleAssignments_Drivers_DriverId",
                table: "VehicleAssignments",
                column: "DriverId",
                principalTable: "Drivers",
                principalColumn: "DriverId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
