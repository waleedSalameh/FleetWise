using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FleetWise.DAL.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_trips_VehicleAssignments_VehicleAssignmentAssignmentId",
                table: "trips");

            migrationBuilder.DropPrimaryKey(
                name: "PK_trips",
                table: "trips");

            migrationBuilder.RenameTable(
                name: "trips",
                newName: "Trips");

            migrationBuilder.RenameIndex(
                name: "IX_trips_VehicleAssignmentAssignmentId",
                table: "Trips",
                newName: "IX_Trips_VehicleAssignmentAssignmentId");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Trips",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Trips",
                table: "Trips",
                column: "TripId");

            migrationBuilder.AddForeignKey(
                name: "FK_Trips_VehicleAssignments_VehicleAssignmentAssignmentId",
                table: "Trips",
                column: "VehicleAssignmentAssignmentId",
                principalTable: "VehicleAssignments",
                principalColumn: "AssignmentId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trips_VehicleAssignments_VehicleAssignmentAssignmentId",
                table: "Trips");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Trips",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Trips");

            migrationBuilder.RenameTable(
                name: "Trips",
                newName: "trips");

            migrationBuilder.RenameIndex(
                name: "IX_Trips_VehicleAssignmentAssignmentId",
                table: "trips",
                newName: "IX_trips_VehicleAssignmentAssignmentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_trips",
                table: "trips",
                column: "TripId");

            migrationBuilder.AddForeignKey(
                name: "FK_trips_VehicleAssignments_VehicleAssignmentAssignmentId",
                table: "trips",
                column: "VehicleAssignmentAssignmentId",
                principalTable: "VehicleAssignments",
                principalColumn: "AssignmentId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
