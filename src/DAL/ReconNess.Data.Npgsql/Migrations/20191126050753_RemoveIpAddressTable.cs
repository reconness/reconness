using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ReconNess.Data.Npgsql.Migrations
{
    public partial class RemoveIpAddressTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subdomains_IpAddress_IpAddressId",
                table: "Subdomains");

            migrationBuilder.DropTable(
                name: "IpAddress");

            migrationBuilder.DropIndex(
                name: "IX_Subdomains_IpAddressId",
                table: "Subdomains");

            migrationBuilder.DropColumn(
                name: "IpAddressId",
                table: "Subdomains");

            migrationBuilder.AddColumn<string>(
                name: "IpAddress",
                table: "Subdomains",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IpAddress",
                table: "Subdomains");

            migrationBuilder.AddColumn<Guid>(
                name: "IpAddressId",
                table: "Subdomains",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "IpAddress",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CIDR = table.Column<string>(type: "text", nullable: true),
                    Country = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false),
                    Ip = table.Column<string>(type: "text", nullable: true),
                    IsAlive = table.Column<bool>(type: "boolean", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IpAddress", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Subdomains_IpAddressId",
                table: "Subdomains",
                column: "IpAddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_Subdomains_IpAddress_IpAddressId",
                table: "Subdomains",
                column: "IpAddressId",
                principalTable: "IpAddress",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
