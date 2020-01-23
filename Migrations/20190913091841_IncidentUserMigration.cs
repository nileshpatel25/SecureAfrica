using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SecureAfrica.Migrations
{
    public partial class IncidentUserMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IncidentUserMessages",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    IncidentId = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    StatusMessage = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncidentUserMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NotifiedUsers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    IncidentId = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotifiedUsers", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IncidentUserMessages");

            migrationBuilder.DropTable(
                name: "NotifiedUsers");
        }
    }
}
