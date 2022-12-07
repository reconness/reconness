using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace ReconNess.Infrastructure.DataAccess.Migrations
{
    public partial class AddDirectoriesAndScreenshotAgain : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ServiceHttpId",
                table: "Subdomains",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ServicesHttp",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false),
                    ScreenshotHttpPNGBase64 = table.Column<string>(nullable: true),
                    ScreenshotHttpsPNGBase64 = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServicesHttp", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceHttpDirectory",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false),
                    Directory = table.Column<string>(nullable: true),
                    StatusCode = table.Column<string>(nullable: true),
                    Method = table.Column<string>(nullable: true),
                    ServiceHttpId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceHttpDirectory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceHttpDirectory_ServicesHttp_ServiceHttpId",
                        column: x => x.ServiceHttpId,
                        principalTable: "ServicesHttp",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Subdomains_ServiceHttpId",
                table: "Subdomains",
                column: "ServiceHttpId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceHttpDirectory_ServiceHttpId",
                table: "ServiceHttpDirectory",
                column: "ServiceHttpId");

            migrationBuilder.AddForeignKey(
                name: "FK_Subdomains_ServicesHttp_ServiceHttpId",
                table: "Subdomains",
                column: "ServiceHttpId",
                principalTable: "ServicesHttp",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subdomains_ServicesHttp_ServiceHttpId",
                table: "Subdomains");

            migrationBuilder.DropTable(
                name: "ServiceHttpDirectory");

            migrationBuilder.DropTable(
                name: "ServicesHttp");

            migrationBuilder.DropIndex(
                name: "IX_Subdomains_ServiceHttpId",
                table: "Subdomains");

            migrationBuilder.DropColumn(
                name: "ServiceHttpId",
                table: "Subdomains");
        }
    }
}
