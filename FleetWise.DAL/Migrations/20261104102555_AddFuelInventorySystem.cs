using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FleetWise.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddFuelInventorySystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FuelRecords_Date",
                table: "FuelRecords");

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "FuelRecords",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldPrecision: 10,
                oldScale: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "PricePerLiter",
                table: "FuelRecords",
                type: "decimal(18,3)",
                precision: 18,
                scale: 3,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,3)",
                oldPrecision: 10,
                oldScale: 3);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "FuelRecords",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddColumn<int>(
                name: "FuelSourceType",
                table: "FuelRecords",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "StationName",
                table: "FuelRecords",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalCost",
                table: "FuelRecords",
                type: "decimal(18,3)",
                precision: 18,
                scale: 3,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "FuelPurchases",
                columns: table => new
                {
                    FuelPurchaseId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PurchaseDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    RemainingQuantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PricePerLiter = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    SupplierName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    InvoiceNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FuelPurchases", x => x.FuelPurchaseId);
                    table.CheckConstraint("CK_FuelPurchases_PricePerLiter", "[PricePerLiter] >= 0");
                    table.CheckConstraint("CK_FuelPurchases_Quantity", "[Quantity] > 0");
                    table.CheckConstraint("CK_FuelPurchases_RemainingQuantity", "[RemainingQuantity] >= 0 AND [RemainingQuantity] <= [Quantity]");
                });

            migrationBuilder.CreateTable(
                name: "fuelRecordConsumptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FuelRecordId = table.Column<int>(type: "int", nullable: false),
                    FuelPurchaseId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PricePerLiter = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    TotalCost = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_fuelRecordConsumptions", x => x.Id);
                    table.CheckConstraint("CK_FuelRecordConsumptions_PricePerLiter", "[PricePerLiter] >= 0");
                    table.CheckConstraint("CK_FuelRecordConsumptions_Quantity", "[Quantity] > 0");
                    table.CheckConstraint("CK_FuelRecordConsumptions_TotalCost", "[TotalCost] >= 0");
                    table.ForeignKey(
                        name: "FK_fuelRecordConsumptions_FuelPurchases_FuelPurchaseId",
                        column: x => x.FuelPurchaseId,
                        principalTable: "FuelPurchases",
                        principalColumn: "FuelPurchaseId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_fuelRecordConsumptions_FuelRecords_FuelRecordId",
                        column: x => x.FuelRecordId,
                        principalTable: "FuelRecords",
                        principalColumn: "FuelRecordId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_fuelRecordConsumptions_FuelPurchaseId",
                table: "fuelRecordConsumptions",
                column: "FuelPurchaseId");

            migrationBuilder.CreateIndex(
                name: "IX_fuelRecordConsumptions_FuelRecordId_FuelPurchaseId",
                table: "fuelRecordConsumptions",
                columns: new[] { "FuelRecordId", "FuelPurchaseId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "fuelRecordConsumptions");

            migrationBuilder.DropTable(
                name: "FuelPurchases");

            migrationBuilder.DropColumn(
                name: "FuelSourceType",
                table: "FuelRecords");

            migrationBuilder.DropColumn(
                name: "StationName",
                table: "FuelRecords");

            migrationBuilder.DropColumn(
                name: "TotalCost",
                table: "FuelRecords");

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "FuelRecords",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "PricePerLiter",
                table: "FuelRecords",
                type: "decimal(10,3)",
                precision: 10,
                scale: 3,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,3)",
                oldPrecision: 18,
                oldScale: 3);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "FuelRecords",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_FuelRecords_Date",
                table: "FuelRecords",
                column: "Date");
        }
    }
}
