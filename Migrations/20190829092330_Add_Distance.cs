using Microsoft.EntityFrameworkCore.Migrations;

namespace SecureAfrica.Migrations
{
    public partial class Add_Distance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Distance",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Distance",
                table: "AspNetUsers");
        }
    }
}
