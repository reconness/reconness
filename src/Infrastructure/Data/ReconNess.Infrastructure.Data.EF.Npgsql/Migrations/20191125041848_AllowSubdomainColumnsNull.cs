using Microsoft.EntityFrameworkCore.Migrations;

namespace ReconNess.Infrastructure.Data.EF.Npgsql.Migrations
{
    public partial class AllowSubdomainColumnsNull : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsMainPortal",
                table: "Subdomains",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<bool>(
                name: "HasHttpOpen",
                table: "Subdomains",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "boolean");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsMainPortal",
                table: "Subdomains",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "HasHttpOpen",
                table: "Subdomains",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);
        }
    }
}
