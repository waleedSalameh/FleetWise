using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FleetWise.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddInternaitoan3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InternationalExports_Customers_CustomerId1",
                table: "InternationalExports");

            migrationBuilder.DropIndex(
                name: "IX_InternationalExports_CustomerId1",
                table: "InternationalExports");

            migrationBuilder.DropColumn(
                name: "CustomerId1",
                table: "InternationalExports");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CustomerId1",
                table: "InternationalExports",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InternationalExports_CustomerId1",
                table: "InternationalExports",
                column: "CustomerId1");

            migrationBuilder.AddForeignKey(
                name: "FK_InternationalExports_Customers_CustomerId1",
                table: "InternationalExports",
                column: "CustomerId1",
                principalTable: "Customers",
                principalColumn: "Id");
        }
    }
}
