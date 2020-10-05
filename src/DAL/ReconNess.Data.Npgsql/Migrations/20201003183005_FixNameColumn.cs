using Microsoft.EntityFrameworkCore.Migrations;

namespace ReconNess.Data.Npgsql.Migrations
{
    public partial class FixNameColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AgentsRawBefore",
                table: "Targets");

            migrationBuilder.DropColumn(
                name: "AgentsRawBefore",
                table: "Subdomains");

            migrationBuilder.DropColumn(
                name: "AgentsRawBefore",
                table: "RootDomains");

            migrationBuilder.AddColumn<string>(
                name: "AgentsRanBefore",
                table: "Targets",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AgentsRanBefore",
                table: "Subdomains",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AgentsRanBefore",
                table: "RootDomains",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AgentsRanBefore",
                table: "Targets");

            migrationBuilder.DropColumn(
                name: "AgentsRanBefore",
                table: "Subdomains");

            migrationBuilder.DropColumn(
                name: "AgentsRanBefore",
                table: "RootDomains");

            migrationBuilder.AddColumn<string>(
                name: "AgentsRawBefore",
                table: "Targets",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AgentsRawBefore",
                table: "Subdomains",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AgentsRawBefore",
                table: "RootDomains",
                type: "text",
                nullable: true);
        }
    }
}
