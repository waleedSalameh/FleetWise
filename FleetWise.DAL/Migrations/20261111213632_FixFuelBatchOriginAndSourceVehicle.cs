using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FleetWise.DAL.Migrations
{
    /// <inheritdoc />
    public partial class FixFuelBatchOriginAndSourceVehicle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE [FuelPurchases] SET [BatchOrigin] = 1 WHERE [BatchOrigin] = 0");
            migrationBuilder.AlterColumn<int>(
                name: "BatchOrigin",
                table: "FuelPurchases",
                type: "int",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_FuelPurchases_SourceVehicleId",
                table: "FuelPurchases",
                column: "SourceVehicleId");

            migrationBuilder.AddForeignKey(
                name: "FK_FuelPurchases_Vehicles_SourceVehicleId",
                table: "FuelPurchases",
                column: "SourceVehicleId",
                principalTable: "Vehicles",
                principalColumn: "VehicleId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FuelPurchases_Vehicles_SourceVehicleId",
                table: "FuelPurchases");

            migrationBuilder.DropIndex(
                name: "IX_FuelPurchases_SourceVehicleId",
                table: "FuelPurchases");

            migrationBuilder.AlterColumn<int>(
                name: "BatchOrigin",
                table: "FuelPurchases",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 1);
        }
    }
}
