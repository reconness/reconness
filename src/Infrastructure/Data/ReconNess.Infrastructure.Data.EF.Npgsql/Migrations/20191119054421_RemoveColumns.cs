using Microsoft.EntityFrameworkCore.Migrations;

namespace ReconNess.Infrastructure.Data.EF.Npgsql.Migrations
{
    public partial class RemoveColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Services");

            migrationBuilder.AddColumn<bool>(
                name: "IsAlive",
                table: "IpAddress",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAlive",
                table: "IpAddress");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Services",
                type: "text",
                nullable: true);
        }
    }
}
