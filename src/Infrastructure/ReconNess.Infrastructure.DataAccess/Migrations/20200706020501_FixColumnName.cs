using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace ReconNess.Infrastructure.DataAccess.Migrations
{
    public partial class FixColumnName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subdomains_RootDomains_DomainId",
                table: "Subdomains");

            migrationBuilder.DropIndex(
                name: "IX_Subdomains_DomainId",
                table: "Subdomains");

            migrationBuilder.DropColumn(
                name: "DomainId",
                table: "Subdomains");

            migrationBuilder.AddColumn<Guid>(
                name: "RootDomainId",
                table: "Subdomains",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Subdomains_RootDomainId",
                table: "Subdomains",
                column: "RootDomainId");

            migrationBuilder.AddForeignKey(
                name: "FK_Subdomains_RootDomains_RootDomainId",
                table: "Subdomains",
                column: "RootDomainId",
                principalTable: "RootDomains",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subdomains_RootDomains_RootDomainId",
                table: "Subdomains");

            migrationBuilder.DropIndex(
                name: "IX_Subdomains_RootDomainId",
                table: "Subdomains");

            migrationBuilder.DropColumn(
                name: "RootDomainId",
                table: "Subdomains");

            migrationBuilder.AddColumn<Guid>(
                name: "DomainId",
                table: "Subdomains",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Subdomains_DomainId",
                table: "Subdomains",
                column: "DomainId");

            migrationBuilder.AddForeignKey(
                name: "FK_Subdomains_RootDomains_DomainId",
                table: "Subdomains",
                column: "DomainId",
                principalTable: "RootDomains",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
