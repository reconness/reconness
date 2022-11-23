using Microsoft.EntityFrameworkCore.Migrations;

namespace ReconNess.Infrastructure.Data.EF.Npgsql.Migrations
{
    public partial class RemoveScreenshotColumnSubdomain : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasScreenshot",
                table: "Subdomains");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasScreenshot",
                table: "Subdomains",
                type: "boolean",
                nullable: true);
        }
    }
}
