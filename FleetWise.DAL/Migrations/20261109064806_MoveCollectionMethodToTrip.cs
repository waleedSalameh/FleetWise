
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FleetWise.DAL.Migrations
{
    public partial class MoveCollectionMethodToTrip : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CollectionMethod",
                table: "Trips",
                type: "int",
                nullable: true);

            migrationBuilder.DropColumn(
                name: "CollectionMethod",
                table: "InternationalIncomings");

            migrationBuilder.DropColumn(
                name: "CollectionMethod",
                table: "InternationalExports");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CollectionMethod",
                table: "InternationalIncomings",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "CollectionMethod",
                table: "InternationalExports",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.DropColumn(
                name: "CollectionMethod",
                table: "Trips");
        }
    }
}
