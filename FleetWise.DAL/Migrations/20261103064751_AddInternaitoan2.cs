using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FleetWise.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddInternaitoan2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InternationalExports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CargoNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CargoType = table.Column<int>(type: "int", nullable: false),
                    LoadingLocation = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FactoryName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Weight = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CustomsDeclarationNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TripDuration = table.Column<int>(type: "int", nullable: false),
                    UnloadingLocation = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Distance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CollectionMethod = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IsPaid = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CustomerId1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InternationalExports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InternationalExports_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InternationalExports_Customers_CustomerId1",
                        column: x => x.CustomerId1,
                        principalTable: "Customers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_InternationalExports_CargoNumber",
                table: "InternationalExports",
                column: "CargoNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InternationalExports_CustomerId",
                table: "InternationalExports",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_InternationalExports_CustomerId1",
                table: "InternationalExports",
                column: "CustomerId1");

            migrationBuilder.CreateIndex(
                name: "IX_InternationalExports_CustomsDeclarationNumber",
                table: "InternationalExports",
                column: "CustomsDeclarationNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InternationalExports");
        }
    }
}
