using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FleetWise.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddNotesToIncomingAqabaContainer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "IncomingAqabaContainers",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Notes",
                table: "IncomingAqabaContainers");
        }
    }
}