using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReconNess.Infrastructure.DataAccess.Migrations
{
    public partial class RemoveAgentTypeColumnFromAgentRunnerTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AgentRunnerType",
                table: "AgentRunners");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AgentRunnerType",
                table: "AgentRunners",
                type: "text",
                nullable: true);
        }
    }
}
