using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReconNess.Infrastructure.Data.EF.Npgsql.Migrations
{
    public partial class AddAgentImageName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageName",
                table: "Agents",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageName",
                table: "Agents");
        }
    }
}
