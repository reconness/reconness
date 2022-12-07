using Microsoft.EntityFrameworkCore.Migrations;

namespace ReconNess.Infrastructure.DataAccess.Migrations
{
    public partial class AddAgentOnlyIfIsAliveColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "OnlyIfIsAlive",
                table: "Agents",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OnlyIfIsAlive",
                table: "Agents");
        }
    }
}
