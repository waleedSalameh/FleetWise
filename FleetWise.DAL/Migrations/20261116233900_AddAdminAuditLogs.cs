using FleetWise.DAL.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FleetWise.DAL.Migrations;

public partial class AddAdminAuditLogs : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "AdminAuditLogs",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                ActorUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                Action = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                TargetType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                TargetId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                ChangeSummary = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                OccurredAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
            },
            constraints: table => table.PrimaryKey("PK_AdminAuditLogs", entry => entry.Id)
        );

        migrationBuilder.CreateIndex(
            name: "IX_AdminAuditLogs_OccurredAtUtc",
            table: "AdminAuditLogs",
            column: "OccurredAtUtc"
        );

        migrationBuilder.CreateIndex(
            name: "IX_AdminAuditLogs_TargetType_TargetId",
            table: "AdminAuditLogs",
            columns: new[] { "TargetType", "TargetId" }
        );
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "AdminAuditLogs");
    }
}
