using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReconNess.Infrastructure.DataAccess.Migrations
{
    public partial class AddCreatedByColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EntitySource",
                table: "Agents");

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Agents",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Agents");

            migrationBuilder.AddColumn<int>(
                name: "EntitySource",
                table: "Agents",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
