using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ReconNess.Data.Npgsql.Migrations
{
    public partial class UpdateNoteTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubdomainNote");

            migrationBuilder.DropTable(
                name: "TargetNote");

            migrationBuilder.CreateTable(
                name: "Note",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false),
                    Notes = table.Column<string>(nullable: true),
                    TargetRef = table.Column<Guid>(nullable: false),
                    SubdomainRef = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Note", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Note_Subdomains_SubdomainRef",
                        column: x => x.SubdomainRef,
                        principalTable: "Subdomains",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Note_Targets_TargetRef",
                        column: x => x.TargetRef,
                        principalTable: "Targets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Note_SubdomainRef",
                table: "Note",
                column: "SubdomainRef",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Note_TargetRef",
                table: "Note",
                column: "TargetRef",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Note");

            migrationBuilder.CreateTable(
                name: "SubdomainNote",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false),
                    Note = table.Column<string>(type: "text", nullable: true),
                    SubdomainId = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubdomainNote", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubdomainNote_Subdomains_SubdomainId",
                        column: x => x.SubdomainId,
                        principalTable: "Subdomains",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TargetNote",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false),
                    Note = table.Column<string>(type: "text", nullable: true),
                    TargetId = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TargetNote", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TargetNote_Targets_TargetId",
                        column: x => x.TargetId,
                        principalTable: "Targets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubdomainNote_SubdomainId",
                table: "SubdomainNote",
                column: "SubdomainId");

            migrationBuilder.CreateIndex(
                name: "IX_TargetNote_TargetId",
                table: "TargetNote",
                column: "TargetId");
        }
    }
}
