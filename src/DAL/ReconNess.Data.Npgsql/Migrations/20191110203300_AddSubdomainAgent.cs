using Microsoft.EntityFrameworkCore.Migrations;

namespace ReconNess.Data.Npgsql.Migrations
{
    public partial class AddSubdomainAgent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FromAgents",
                table: "Subdomains",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FromAgents",
                table: "Subdomains");
        }
    }
}
