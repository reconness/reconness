using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace ReconNess.Data.Npgsql.Migrations
{
    public partial class FixManyToManyRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AgentCategory_Agents_AgentId",
                table: "AgentCategory");

            migrationBuilder.DropForeignKey(
                name: "FK_AgentCategory_Categories_CategoryId",
                table: "AgentCategory");

            migrationBuilder.DropTable(
                name: "SubdomainLabel");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "AgentCategory");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "AgentCategory");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "AgentCategory");

            migrationBuilder.RenameColumn(
                name: "CategoryId",
                table: "AgentCategory",
                newName: "CategoriesId");

            migrationBuilder.RenameColumn(
                name: "AgentId",
                table: "AgentCategory",
                newName: "AgentsId");

            migrationBuilder.RenameIndex(
                name: "IX_AgentCategory_CategoryId",
                table: "AgentCategory",
                newName: "IX_AgentCategory_CategoriesId");

            migrationBuilder.CreateTable(
                name: "LabelSubdomain",
                columns: table => new
                {
                    LabelsId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubdomainsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabelSubdomain", x => new { x.LabelsId, x.SubdomainsId });
                    table.ForeignKey(
                        name: "FK_LabelSubdomain_Label_LabelsId",
                        column: x => x.LabelsId,
                        principalTable: "Label",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LabelSubdomain_Subdomains_SubdomainsId",
                        column: x => x.SubdomainsId,
                        principalTable: "Subdomains",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LabelSubdomain_SubdomainsId",
                table: "LabelSubdomain",
                column: "SubdomainsId");

            migrationBuilder.AddForeignKey(
                name: "FK_AgentCategory_Agents_AgentsId",
                table: "AgentCategory",
                column: "AgentsId",
                principalTable: "Agents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AgentCategory_Categories_CategoriesId",
                table: "AgentCategory",
                column: "CategoriesId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AgentCategory_Agents_AgentsId",
                table: "AgentCategory");

            migrationBuilder.DropForeignKey(
                name: "FK_AgentCategory_Categories_CategoriesId",
                table: "AgentCategory");

            migrationBuilder.DropTable(
                name: "LabelSubdomain");

            migrationBuilder.RenameColumn(
                name: "CategoriesId",
                table: "AgentCategory",
                newName: "CategoryId");

            migrationBuilder.RenameColumn(
                name: "AgentsId",
                table: "AgentCategory",
                newName: "AgentId");

            migrationBuilder.RenameIndex(
                name: "IX_AgentCategory_CategoriesId",
                table: "AgentCategory",
                newName: "IX_AgentCategory_CategoryId");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "AgentCategory",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "AgentCategory",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "AgentCategory",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "SubdomainLabel",
                columns: table => new
                {
                    SubdomainId = table.Column<Guid>(type: "uuid", nullable: false),
                    LabelId = table.Column<Guid>(type: "uuid", nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_SubdomainLabel_LabelId",
                table: "SubdomainLabel",
                column: "LabelId");

            migrationBuilder.AddForeignKey(
                name: "FK_AgentCategory_Agents_AgentId",
                table: "AgentCategory",
                column: "AgentId",
                principalTable: "Agents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AgentCategory_Categories_CategoryId",
                table: "AgentCategory",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
