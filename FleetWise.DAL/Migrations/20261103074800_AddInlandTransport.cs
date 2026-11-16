using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FleetWise.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddInlandTransport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InlandTransports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TripCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    TripStatus = table.Column<int>(type: "int", nullable: false),
                    CargoType = table.Column<int>(type: "int", nullable: false),
                    Weight = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LoadingLocation = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    UnloadingLocation = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InlandTransports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InlandTransports_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InlandTransports_CustomerId",
                table: "InlandTransports",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_InlandTransports_TripCode",
                table: "InlandTransports",
                column: "TripCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InlandTransports");
        }
    }
}
