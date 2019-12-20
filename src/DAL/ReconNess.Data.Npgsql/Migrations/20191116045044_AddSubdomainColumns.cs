using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ReconNess.Data.Npgsql.Migrations
{
    public partial class AddSubdomainColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TargetName",
                table: "Targets");

            migrationBuilder.DropColumn(
                name: "Domain",
                table: "Subdomains");

            migrationBuilder.DropColumn(
                name: "ServiceName",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "Cmd",
                table: "Agents");

            migrationBuilder.DropColumn(
                name: "ScriptEngine",
                table: "Agents");

            migrationBuilder.DropColumn(
                name: "ScriptToParserOutput",
                table: "Agents");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Targets",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Subdomains",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Services",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Command",
                table: "Agents",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Script",
                table: "Agents",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Agents",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Label",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Label", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubdomainNote",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false),
                    Note = table.Column<string>(nullable: true),
                    SubdomainId = table.Column<Guid>(nullable: true)
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
                name: "SubdomainLabel",
                columns: table => new
                {
                    SubdomainId = table.Column<Guid>(nullable: false),
                    LabelId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubdomainLabel", x => new { x.SubdomainId, x.LabelId });
                    table.ForeignKey(
                        name: "FK_SubdomainLabel_Label_LabelId",
                        column: x => x.LabelId,
                        principalTable: "Label",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubdomainLabel_Subdomains_SubdomainId",
                        column: x => x.SubdomainId,
                        principalTable: "Subdomains",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Label",
                columns: new[] { "Id", "CreatedAt", "Deleted", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("396adbc5-b2be-4fbf-ad71-1da0858c0cc3"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Ignore", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("433dd1c5-dfb7-4f64-bdaf-df9d41e54788"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "ToCheck", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("44803ec6-1444-4f94-bd90-ba0fc977bf4b"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Checking", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("27d1283c-dd62-46a8-b48c-7a7be67d7caa"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Bounty", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubdomainLabel_LabelId",
                table: "SubdomainLabel",
                column: "LabelId");

            migrationBuilder.CreateIndex(
                name: "IX_SubdomainNote_SubdomainId",
                table: "SubdomainNote",
                column: "SubdomainId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubdomainLabel");

            migrationBuilder.DropTable(
                name: "SubdomainNote");

            migrationBuilder.DropTable(
                name: "Label");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Targets");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Subdomains");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "Command",
                table: "Agents");

            migrationBuilder.DropColumn(
                name: "Script",
                table: "Agents");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Agents");

            migrationBuilder.AddColumn<string>(
                name: "TargetName",
                table: "Targets",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Domain",
                table: "Subdomains",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ServiceName",
                table: "Services",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Cmd",
                table: "Agents",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ScriptEngine",
                table: "Agents",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ScriptToParserOutput",
                table: "Agents",
                type: "text",
                nullable: true);
        }
    }
}
