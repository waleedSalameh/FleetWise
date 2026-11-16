using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FleetWise.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddMaintenanceInventorySystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaintenanceLocation",
                table: "MaintenanceRecords",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Oils",
                columns: table => new
                {
                    OilId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OilName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Brand = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Viscosity = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Unit = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false, defaultValue: "Liter"),
                    PurchasePrice = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    MinimumQuantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Oils", x => x.OilId);
                });

            migrationBuilder.CreateTable(
                name: "SpareParts",
                columns: table => new
                {
                    SparePartId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PartNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PartName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Brand = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Unit = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false, defaultValue: "Piece"),
                    PurchasePrice = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    MinimumQuantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpareParts", x => x.SparePartId);
                });

            migrationBuilder.CreateTable(
                name: "OilStockMovements",
                columns: table => new
                {
                    OilStockMovementId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OilId = table.Column<int>(type: "int", nullable: false),
                    MovementType = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MaintenanceRecordId = table.Column<int>(type: "int", nullable: true),
                    ReferenceNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OilStockMovements", x => x.OilStockMovementId);
                    table.ForeignKey(
                        name: "FK_OilStockMovements_MaintenanceRecords_MaintenanceRecordId",
                        column: x => x.MaintenanceRecordId,
                        principalTable: "MaintenanceRecords",
                        principalColumn: "MaintenanceRecordId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OilStockMovements_Oils_OilId",
                        column: x => x.OilId,
                        principalTable: "Oils",
                        principalColumn: "OilId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SparePartMovements",
                columns: table => new
                {
                    SparePartMovementId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SparePartId = table.Column<int>(type: "int", nullable: false),
                    MovementType = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MaintenanceRecordId = table.Column<int>(type: "int", nullable: true),
                    ReferenceNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SparePartMovements", x => x.SparePartMovementId);
                    table.ForeignKey(
                        name: "FK_SparePartMovements_MaintenanceRecords_MaintenanceRecordId",
                        column: x => x.MaintenanceRecordId,
                        principalTable: "MaintenanceRecords",
                        principalColumn: "MaintenanceRecordId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SparePartMovements_SpareParts_SparePartId",
                        column: x => x.SparePartId,
                        principalTable: "SpareParts",
                        principalColumn: "SparePartId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OilStockMovements_MaintenanceRecordId",
                table: "OilStockMovements",
                column: "MaintenanceRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_OilStockMovements_OilId",
                table: "OilStockMovements",
                column: "OilId");

            migrationBuilder.CreateIndex(
                name: "IX_SparePartMovements_MaintenanceRecordId",
                table: "SparePartMovements",
                column: "MaintenanceRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_SparePartMovements_SparePartId",
                table: "SparePartMovements",
                column: "SparePartId");

            migrationBuilder.CreateIndex(
                name: "IX_SpareParts_PartNumber",
                table: "SpareParts",
                column: "PartNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OilStockMovements");

            migrationBuilder.DropTable(
                name: "SparePartMovements");

            migrationBuilder.DropTable(
                name: "Oils");

            migrationBuilder.DropTable(
                name: "SpareParts");

            migrationBuilder.DropColumn(
                name: "MaintenanceLocation",
                table: "MaintenanceRecords");
        }
    }
}
