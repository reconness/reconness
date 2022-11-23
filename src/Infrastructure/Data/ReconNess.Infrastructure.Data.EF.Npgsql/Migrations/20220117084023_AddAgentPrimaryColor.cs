using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReconNess.Infrastructure.Data.EF.Npgsql.Migrations
{
    public partial class AddAgentPrimaryColor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PrimaryColor",
                table: "Agents",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrimaryColor",
                table: "Agents");
        }
    }
}
