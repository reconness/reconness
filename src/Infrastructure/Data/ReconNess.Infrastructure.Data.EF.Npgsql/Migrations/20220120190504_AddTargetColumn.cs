using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReconNess.Infrastructure.Data.EF.Npgsql.Migrations
{
    public partial class AddTargetColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Target",
                table: "Agents",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Target",
                table: "Agents");
        }
    }
}
