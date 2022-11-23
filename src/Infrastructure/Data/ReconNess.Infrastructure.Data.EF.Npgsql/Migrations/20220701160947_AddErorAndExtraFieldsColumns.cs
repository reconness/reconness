using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReconNess.Infrastructure.Data.EF.Npgsql.Migrations
{
    public partial class AddErorAndExtraFieldsColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExtraFields",
                table: "Subdomains",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Error",
                table: "AgentRunnerCommands",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExtraFields",
                table: "Subdomains");

            migrationBuilder.DropColumn(
                name: "Error",
                table: "AgentRunnerCommands");
        }
    }
}
