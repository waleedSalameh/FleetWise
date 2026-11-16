
using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FleetWise.DAL.Migrations
{
    public partial class AddInternaitoan : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "ExportAqabaBookings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "IncomingAqabaBills",
                type: "int",
                nullable: true);

            migrationBuilder.Sql(@"
                INSERT INTO Customers (CustomerName, IsDeleted)
                SELECT DISTINCT LTRIM(RTRIM(CustomerName)), 0
                FROM ExportAqabaBookings
                WHERE CustomerName IS NOT NULL
                  AND LTRIM(RTRIM(CustomerName)) <> '';

                INSERT INTO Customers (CustomerName, IsDeleted)
                SELECT DISTINCT LTRIM(RTRIM(incoming.CustomerName)), 0
                FROM IncomingAqabaBills incoming
                WHERE incoming.CustomerName IS NOT NULL
                  AND LTRIM(RTRIM(incoming.CustomerName)) <> ''
                  AND NOT EXISTS (
                      SELECT 1
                      FROM Customers customer
                      WHERE LTRIM(RTRIM(customer.CustomerName)) = LTRIM(RTRIM(incoming.CustomerName))
                  );

                UPDATE booking
                SET CustomerId = customer.Id
                FROM ExportAqabaBookings booking
                INNER JOIN Customers customer
                    ON LTRIM(RTRIM(customer.CustomerName)) = LTRIM(RTRIM(booking.CustomerName));

                UPDATE incoming
                SET CustomerId = customer.Id
                FROM IncomingAqabaBills incoming
                INNER JOIN Customers customer
                    ON LTRIM(RTRIM(customer.CustomerName)) = LTRIM(RTRIM(incoming.CustomerName));
            ");

            migrationBuilder.AlterColumn<int>(
                name: "CustomerId",
                table: "ExportAqabaBookings",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CustomerId",
                table: "IncomingAqabaBills",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "InternationalIncomings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CargoNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CargoType = table.Column<int>(type: "int", nullable: false),
                    LoadingLocation = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FactoryName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Weight = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CustomsDeclarationNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TripDuration = table.Column<int>(type: "int", nullable: false),
                    UnloadingLocation = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Distance = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CollectionMethod = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IsPaid = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InternationalIncomings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InternationalIncomings_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IncomingAqabaBills_CustomerId",
                table: "IncomingAqabaBills",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_ExportAqabaBookings_CustomerId",
                table: "ExportAqabaBookings",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_PhoneNumber",
                table: "Customers",
                column: "PhoneNumber",
                unique: true,
                filter: "[IsDeleted] = 0 AND [PhoneNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_InternationalIncomings_CargoNumber",
                table: "InternationalIncomings",
                column: "CargoNumber",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_InternationalIncomings_CustomerId",
                table: "InternationalIncomings",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_InternationalIncomings_CustomsDeclarationNumber",
                table: "InternationalIncomings",
                column: "CustomsDeclarationNumber",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.AddForeignKey(
                name: "FK_ExportAqabaBookings_Customers_CustomerId",
                table: "ExportAqabaBookings",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IncomingAqabaBills_Customers_CustomerId",
                table: "IncomingAqabaBills",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExportAqabaBookings_Customers_CustomerId",
                table: "ExportAqabaBookings");

            migrationBuilder.DropForeignKey(
                name: "FK_IncomingAqabaBills_Customers_CustomerId",
                table: "IncomingAqabaBills");

            migrationBuilder.DropTable(
                name: "InternationalIncomings");

            migrationBuilder.DropIndex(
                name: "IX_IncomingAqabaBills_CustomerId",
                table: "IncomingAqabaBills");

            migrationBuilder.DropIndex(
                name: "IX_ExportAqabaBookings_CustomerId",
                table: "ExportAqabaBookings");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "IncomingAqabaBills");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "ExportAqabaBookings");

            migrationBuilder.DropTable(
                name: "Customers");
        }
    }
}
