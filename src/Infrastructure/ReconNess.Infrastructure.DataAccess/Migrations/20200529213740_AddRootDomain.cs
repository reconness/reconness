using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace ReconNess.Infrastructure.DataAccess.Migrations
{
    public partial class AddRootDomain : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Note_Targets_TargetId",
                table: "Note");

            migrationBuilder.DropForeignKey(
                name: "FK_Subdomains_Targets_TargetId",
                table: "Subdomains");

            migrationBuilder.DropIndex(
                name: "IX_Subdomains_TargetId",
                table: "Subdomains");

            migrationBuilder.DropColumn(
                name: "RootDomain",
                table: "Targets");

            migrationBuilder.DropColumn(
                name: "TargetId",
                table: "Subdomains");

            migrationBuilder.AddColumn<Guid>(
                name: "DomainId",
                table: "Subdomains",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RootDomains",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    TargetId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RootDomains", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RootDomains_Targets_TargetId",
                        column: x => x.TargetId,
                        principalTable: "Targets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Subdomains_DomainId",
                table: "Subdomains",
                column: "DomainId");

            migrationBuilder.CreateIndex(
                name: "IX_RootDomains_TargetId",
                table: "RootDomains",
                column: "TargetId");

            migrationBuilder.AddForeignKey(
                name: "FK_Note_RootDomains_TargetId",
                table: "Note",
                column: "TargetId",
                principalTable: "RootDomains",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Subdomains_RootDomains_DomainId",
                table: "Subdomains",
                column: "DomainId",
                principalTable: "RootDomains",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Note_RootDomains_TargetId",
                table: "Note");

            migrationBuilder.DropForeignKey(
                name: "FK_Subdomains_RootDomains_DomainId",
                table: "Subdomains");

            migrationBuilder.DropTable(
                name: "RootDomains");

            migrationBuilder.DropIndex(
                name: "IX_Subdomains_DomainId",
                table: "Subdomains");

            migrationBuilder.DropColumn(
                name: "DomainId",
                table: "Subdomains");

            migrationBuilder.AddColumn<string>(
                name: "RootDomain",
                table: "Targets",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TargetId",
                table: "Subdomains",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Subdomains_TargetId",
                table: "Subdomains",
                column: "TargetId");

            migrationBuilder.AddForeignKey(
                name: "FK_Note_Targets_TargetId",
                table: "Note",
                column: "TargetId",
                principalTable: "Targets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Subdomains_Targets_TargetId",
                table: "Subdomains",
                column: "TargetId",
                principalTable: "Targets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
