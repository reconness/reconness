using Microsoft.EntityFrameworkCore.Migrations;

namespace ReconNess.Infrastructure.DataAccess.Migrations
{
    public partial class RemoveScreenshotColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ScreenshotHttpPNGBase64",
                table: "Subdomains");

            migrationBuilder.DropColumn(
                name: "ScreenshotHttpsPNGBase64",
                table: "Subdomains");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ScreenshotHttpPNGBase64",
                table: "Subdomains",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ScreenshotHttpsPNGBase64",
                table: "Subdomains",
                type: "text",
                nullable: true);
        }
    }
}
