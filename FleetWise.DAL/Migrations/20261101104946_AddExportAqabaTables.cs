using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FleetWise.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddExportAqabaTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExportAqabaBookings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ContainerCount = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExportAqabaBookings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExportAqabaContainers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingId = table.Column<int>(type: "int", nullable: false),
                    ContainerNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ContainerSize = table.Column<int>(type: "int", nullable: false),
                    ContainerType = table.Column<int>(type: "int", nullable: false),
                    LoadingLocationId = table.Column<int>(type: "int", nullable: false),
                    RouteStatus = table.Column<int>(type: "int", nullable: false),
                    WithdrawalDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LoadingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeliveryLocationId = table.Column<int>(type: "int", nullable: false),
                    WeightInTons = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExportAqabaContainers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExportAqabaContainers_DeliveryLocations_DeliveryLocationId",
                        column: x => x.DeliveryLocationId,
                        principalTable: "DeliveryLocations",
                        principalColumn: "DeliveryLocationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExportAqabaContainers_DeliveryLocations_LoadingLocationId",
                        column: x => x.LoadingLocationId,
                        principalTable: "DeliveryLocations",
                        principalColumn: "DeliveryLocationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExportAqabaContainers_ExportAqabaBookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "ExportAqabaBookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExportAqabaBookings_BookingNumber",
                table: "ExportAqabaBookings",
                column: "BookingNumber",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_ExportAqabaContainers_BookingId",
                table: "ExportAqabaContainers",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_ExportAqabaContainers_ContainerNumber",
                table: "ExportAqabaContainers",
                column: "ContainerNumber",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_ExportAqabaContainers_DeliveryLocationId",
                table: "ExportAqabaContainers",
                column: "DeliveryLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_ExportAqabaContainers_LoadingLocationId",
                table: "ExportAqabaContainers",
                column: "LoadingLocationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExportAqabaContainers");

            migrationBuilder.DropTable(
                name: "ExportAqabaBookings");
        }
    }
}
