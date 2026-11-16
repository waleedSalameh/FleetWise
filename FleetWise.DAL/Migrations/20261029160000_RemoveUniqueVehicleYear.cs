using FleetWise.DAL.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FleetWise.DAL.Migrations
{
    [DbContext(typeof(FleetWiseDbContext))]
    [Migration("20261029160000_RemoveUniqueVehicleYear")]
    public partial class RemoveUniqueVehicleYear : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Vehicles_Year",
                table: "Vehicles");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_Year",
                table: "Vehicles",
                column: "Year",
                unique: true);
        }
    }
}