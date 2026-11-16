using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FleetWise.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddIncomingAqabaTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DeliveryLocations",
                columns: table => new
                {
                    DeliveryLocationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryLocations", x => x.DeliveryLocationId);
                });

            migrationBuilder.CreateTable(
                name: "IncomingAqabaBills",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BillNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ContainerCount = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncomingAqabaBills", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShippingLines",
                columns: table => new
                {
                    ShippingLineId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingLines", x => x.ShippingLineId);
                });

            migrationBuilder.CreateTable(
                name: "IncomingAqabaContainers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BillId = table.Column<int>(type: "int", nullable: false),
                    ContainerNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ContainerType = table.Column<int>(type: "int", nullable: false),
                    ContainerSize = table.Column<int>(type: "int", nullable: false),
                    ShippingLineId = table.Column<int>(type: "int", nullable: false),
                    DeliveryLocationId = table.Column<int>(type: "int", nullable: false),
                    WeightInTons = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    VesselArrivalDate = table.Column<DateOnly>(type: "date", nullable: false),
                    PortFreeTimeEndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EmptyReturnFreeTimeEndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ContainerLoadingDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncomingAqabaContainers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IncomingAqabaContainers_DeliveryLocations_DeliveryLocationId",
                        column: x => x.DeliveryLocationId,
                        principalTable: "DeliveryLocations",
                        principalColumn: "DeliveryLocationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IncomingAqabaContainers_IncomingAqabaBills_BillId",
                        column: x => x.BillId,
                        principalTable: "IncomingAqabaBills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IncomingAqabaContainers_ShippingLines_ShippingLineId",
                        column: x => x.ShippingLineId,
                        principalTable: "ShippingLines",
                        principalColumn: "ShippingLineId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryLocations_Name",
                table: "DeliveryLocations",
                column: "Name",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_IncomingAqabaBills_BillNumber",
                table: "IncomingAqabaBills",
                column: "BillNumber",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_IncomingAqabaContainers_BillId",
                table: "IncomingAqabaContainers",
                column: "BillId");

            migrationBuilder.CreateIndex(
                name: "IX_IncomingAqabaContainers_ContainerNumber",
                table: "IncomingAqabaContainers",
                column: "ContainerNumber",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_IncomingAqabaContainers_DeliveryLocationId",
                table: "IncomingAqabaContainers",
                column: "DeliveryLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_IncomingAqabaContainers_ShippingLineId",
                table: "IncomingAqabaContainers",
                column: "ShippingLineId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingLines_Code",
                table: "ShippingLines",
                column: "Code",
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IncomingAqabaContainers");

            migrationBuilder.DropTable(
                name: "DeliveryLocations");

            migrationBuilder.DropTable(
                name: "IncomingAqabaBills");

            migrationBuilder.DropTable(
                name: "ShippingLines");
        }
    }
}
