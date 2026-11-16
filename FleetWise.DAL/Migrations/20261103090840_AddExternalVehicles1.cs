using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FleetWise.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddExternalVehicles1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ExternalVehicless",
                table: "ExternalVehicless");

            migrationBuilder.RenameTable(
                name: "ExternalVehicless",
                newName: "ExternalVehicles");

            migrationBuilder.RenameIndex(
                name: "IX_ExternalVehicless_PlateNumber",
                table: "ExternalVehicles",
                newName: "IX_ExternalVehicles_PlateNumber");

            migrationBuilder.RenameIndex(
                name: "IX_ExternalVehicless_PhoneNumber",
                table: "ExternalVehicles",
                newName: "IX_ExternalVehicles_PhoneNumber");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ExternalVehicles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExternalVehicles",
                table: "ExternalVehicles",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ExternalVehicles",
                table: "ExternalVehicles");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ExternalVehicles");

            migrationBuilder.RenameTable(
                name: "ExternalVehicles",
                newName: "ExternalVehicless");

            migrationBuilder.RenameIndex(
                name: "IX_ExternalVehicles_PlateNumber",
                table: "ExternalVehicless",
                newName: "IX_ExternalVehicless_PlateNumber");

            migrationBuilder.RenameIndex(
                name: "IX_ExternalVehicles_PhoneNumber",
                table: "ExternalVehicless",
                newName: "IX_ExternalVehicless_PhoneNumber");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExternalVehicless",
                table: "ExternalVehicless",
                column: "Id");
        }
    }
}
