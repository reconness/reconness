using Microsoft.EntityFrameworkCore.Migrations;

namespace ReconNess.Infrastructure.DataAccess.Migrations
{
    public partial class AddSubdomainTechnologyColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TechnologyPayload",
                table: "Notifications",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TechnologyPayload",
                table: "Notifications");
        }
    }
}
