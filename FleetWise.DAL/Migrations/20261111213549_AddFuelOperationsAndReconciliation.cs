using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FleetWise.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddFuelOperationsAndReconciliation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BatchOrigin",
                table: "FuelPurchases",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SourceVehicleId",
                table: "FuelPurchases",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FuelTransfers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SourceVehicleId = table.Column<int>(type: "int", nullable: false),
                    OccurredAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReversesTransferId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FuelTransfers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FuelTransfers_FuelTransfers_ReversesTransferId",
                        column: x => x.ReversesTransferId,
                        principalTable: "FuelTransfers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FuelTransfers_Vehicles_SourceVehicleId",
                        column: x => x.SourceVehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "VehicleId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VehicleFuelMeasurements",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VehicleId = table.Column<int>(type: "int", nullable: false),
                    MeasuredAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MeasuredQuantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MeasuredValueJod = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: true),
                    MeasurementType = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleFuelMeasurements", x => x.Id);
                    table.CheckConstraint("CK_VehicleFuelMeasurements_Quantity", "[MeasuredQuantity] >= 0");
                    table.CheckConstraint("CK_VehicleFuelMeasurements_Value", "[MeasuredValueJod] IS NULL OR [MeasuredValueJod] >= 0");
                    table.ForeignKey(
                        name: "FK_VehicleFuelMeasurements_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "VehicleId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FuelTransferDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FuelTransferId = table.Column<int>(type: "int", nullable: false),
                    DestinationType = table.Column<int>(type: "int", nullable: false),
                    DestinationVehicleId = table.Column<int>(type: "int", nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    UnitCostJod = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    TotalCostJod = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    ReturnedFuelPurchaseId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FuelTransferDetails", x => x.Id);
                    table.CheckConstraint("CK_FuelTransferDetails_Quantity", "[Quantity] > 0");
                    table.ForeignKey(
                        name: "FK_FuelTransferDetails_FuelPurchases_ReturnedFuelPurchaseId",
                        column: x => x.ReturnedFuelPurchaseId,
                        principalTable: "FuelPurchases",
                        principalColumn: "FuelPurchaseId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FuelTransferDetails_FuelTransfers_FuelTransferId",
                        column: x => x.FuelTransferId,
                        principalTable: "FuelTransfers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FuelTransferDetails_Vehicles_DestinationVehicleId",
                        column: x => x.DestinationVehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "VehicleId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MonthlyFuelClosings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VehicleId = table.Column<int>(type: "int", nullable: false),
                    PeriodStart = table.Column<DateOnly>(type: "date", nullable: false),
                    PeriodEnd = table.Column<DateOnly>(type: "date", nullable: false),
                    OpeningMeasurementId = table.Column<long>(type: "bigint", nullable: false),
                    ClosingMeasurementId = table.Column<long>(type: "bigint", nullable: false),
                    OpeningQuantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    OpeningValueJod = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    TotalFuelingQuantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalFuelingValueJod = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    IncomingTransferQuantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    IncomingTransferValueJod = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    OutgoingTransferQuantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    OutgoingTransferValueJod = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    ClosingQuantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ClosingValueJod = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    ActualConsumptionQuantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ActualConsumptionValueJod = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    InternalFuelingCount = table.Column<int>(type: "int", nullable: false),
                    ExternalFuelingCount = table.Column<int>(type: "int", nullable: false),
                    IncomingTransferCount = table.Column<int>(type: "int", nullable: false),
                    OutgoingTransferCount = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ApprovedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonthlyFuelClosings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MonthlyFuelClosings_VehicleFuelMeasurements_ClosingMeasurementId",
                        column: x => x.ClosingMeasurementId,
                        principalTable: "VehicleFuelMeasurements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MonthlyFuelClosings_VehicleFuelMeasurements_OpeningMeasurementId",
                        column: x => x.OpeningMeasurementId,
                        principalTable: "VehicleFuelMeasurements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MonthlyFuelClosings_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "VehicleId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VehicleFuelLedgerEntries",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VehicleId = table.Column<int>(type: "int", nullable: false),
                    OccurredAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EntryType = table.Column<int>(type: "int", nullable: false),
                    QuantityDelta = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ValueDeltaJod = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    FuelRecordId = table.Column<int>(type: "int", nullable: true),
                    FuelTransferDetailId = table.Column<int>(type: "int", nullable: true),
                    MeasurementId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleFuelLedgerEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehicleFuelLedgerEntries_FuelRecords_FuelRecordId",
                        column: x => x.FuelRecordId,
                        principalTable: "FuelRecords",
                        principalColumn: "FuelRecordId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VehicleFuelLedgerEntries_FuelTransferDetails_FuelTransferDetailId",
                        column: x => x.FuelTransferDetailId,
                        principalTable: "FuelTransferDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VehicleFuelLedgerEntries_VehicleFuelMeasurements_MeasurementId",
                        column: x => x.MeasurementId,
                        principalTable: "VehicleFuelMeasurements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VehicleFuelLedgerEntries_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "VehicleId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FuelTransferDetails_DestinationVehicleId",
                table: "FuelTransferDetails",
                column: "DestinationVehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_FuelTransferDetails_FuelTransferId_DestinationType_DestinationVehicleId",
                table: "FuelTransferDetails",
                columns: new[] { "FuelTransferId", "DestinationType", "DestinationVehicleId" });

            migrationBuilder.CreateIndex(
                name: "IX_FuelTransferDetails_ReturnedFuelPurchaseId",
                table: "FuelTransferDetails",
                column: "ReturnedFuelPurchaseId",
                unique: true,
                filter: "[ReturnedFuelPurchaseId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_FuelTransfers_ReversesTransferId",
                table: "FuelTransfers",
                column: "ReversesTransferId",
                unique: true,
                filter: "[ReversesTransferId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_FuelTransfers_SourceVehicleId_OccurredAt",
                table: "FuelTransfers",
                columns: new[] { "SourceVehicleId", "OccurredAt" });

            migrationBuilder.CreateIndex(
                name: "IX_MonthlyFuelClosings_ClosingMeasurementId",
                table: "MonthlyFuelClosings",
                column: "ClosingMeasurementId");

            migrationBuilder.CreateIndex(
                name: "IX_MonthlyFuelClosings_OpeningMeasurementId",
                table: "MonthlyFuelClosings",
                column: "OpeningMeasurementId");

            migrationBuilder.CreateIndex(
                name: "IX_MonthlyFuelClosings_VehicleId_PeriodStart_PeriodEnd",
                table: "MonthlyFuelClosings",
                columns: new[] { "VehicleId", "PeriodStart", "PeriodEnd" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VehicleFuelLedgerEntries_FuelRecordId",
                table: "VehicleFuelLedgerEntries",
                column: "FuelRecordId",
                unique: true,
                filter: "[FuelRecordId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleFuelLedgerEntries_FuelTransferDetailId_VehicleId_EntryType",
                table: "VehicleFuelLedgerEntries",
                columns: new[] { "FuelTransferDetailId", "VehicleId", "EntryType" });

            migrationBuilder.CreateIndex(
                name: "IX_VehicleFuelLedgerEntries_MeasurementId",
                table: "VehicleFuelLedgerEntries",
                column: "MeasurementId",
                unique: true,
                filter: "[MeasurementId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleFuelLedgerEntries_VehicleId_OccurredAt_Id",
                table: "VehicleFuelLedgerEntries",
                columns: new[] { "VehicleId", "OccurredAt", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_VehicleFuelMeasurements_VehicleId",
                table: "VehicleFuelMeasurements",
                column: "VehicleId",
                unique: true,
                filter: "[MeasurementType] = 1 AND [Status] = 2");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleFuelMeasurements_VehicleId_MeasuredAt",
                table: "VehicleFuelMeasurements",
                columns: new[] { "VehicleId", "MeasuredAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MonthlyFuelClosings");

            migrationBuilder.DropTable(
                name: "VehicleFuelLedgerEntries");

            migrationBuilder.DropTable(
                name: "FuelTransferDetails");

            migrationBuilder.DropTable(
                name: "VehicleFuelMeasurements");

            migrationBuilder.DropTable(
                name: "FuelTransfers");

            migrationBuilder.DropColumn(
                name: "BatchOrigin",
                table: "FuelPurchases");

            migrationBuilder.DropColumn(
                name: "SourceVehicleId",
                table: "FuelPurchases");
        }
    }
}
