using Microsoft.EntityFrameworkCore.Migrations;

namespace ReconNess.Infrastructure.DataAccess.Migrations
{
    public partial class RemoveLogColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Logs",
                table: "AgentRuns");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Logs",
                table: "AgentRuns",
                type: "text",
                nullable: true);
        }
    }
}
