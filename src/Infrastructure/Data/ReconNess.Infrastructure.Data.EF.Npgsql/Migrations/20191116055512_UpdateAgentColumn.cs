using Microsoft.EntityFrameworkCore.Migrations;

namespace ReconNess.Data.Npgsql.Migrations
{
    public partial class UpdateAgentColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Agents");

            migrationBuilder.AddColumn<bool>(
                name: "IsBySubdomain",
                table: "Agents",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBySubdomain",
                table: "Agents");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Agents",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
