using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace ReconNess.Data.Npgsql.Migrations
{
    public partial class FixDirectoryRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<string>(
                name: "ScreenshotHttpPNGBase64",
                table: "Subdomains",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ScreenshotHttpsPNGBase64",
                table: "Subdomains",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Directories",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false),
                    Uri = table.Column<string>(nullable: true),
                    StatusCode = table.Column<string>(nullable: true),
                    Size = table.Column<string>(nullable: true),
                    Method = table.Column<string>(nullable: true),
                    SubdomainId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Directories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Directories_Subdomains_SubdomainId",
                        column: x => x.SubdomainId,
                        principalTable: "Subdomains",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Directories_SubdomainId",
                table: "Directories",
                column: "SubdomainId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Directories");

            migrationBuilder.DropColumn(
                name: "ScreenshotHttpPNGBase64",
                table: "Subdomains");

            migrationBuilder.DropColumn(
                name: "ScreenshotHttpsPNGBase64",
                table: "Subdomains");

            migrationBuilder.AddColumn<Guid>(
                name: "ServiceHttpId",
                table: "Subdomains",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ServicesHttp",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false),
                    ScreenshotHttpPNGBase64 = table.Column<string>(type: "text", nullable: true),
                    ScreenshotHttpsPNGBase64 = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServicesHttp", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceHttpDirectory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false),
                    Directory = table.Column<string>(type: "text", nullable: true),
                    Method = table.Column<string>(type: "text", nullable: true),
                    ServiceHttpId = table.Column<Guid>(type: "uuid", nullable: false),
                    Size = table.Column<string>(type: "text", nullable: true),
                    StatusCode = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceHttpDirectory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceHttpDirectory_ServicesHttp_ServiceHttpId",
                        column: x => x.ServiceHttpId,
                        principalTable: "ServicesHttp",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                onDelete: ReferentialAction.SetNull);
        }
    }
}
