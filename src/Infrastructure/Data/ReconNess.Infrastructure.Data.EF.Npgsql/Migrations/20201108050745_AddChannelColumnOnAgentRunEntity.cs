using Microsoft.EntityFrameworkCore.Migrations;

namespace ReconNess.Infrastructure.Data.EF.Npgsql.Migrations
{
    public partial class AddChannelColumnOnAgentRunEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Channel",
                table: "AgentRuns",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Channel",
                table: "AgentRuns");
        }
    }
}
