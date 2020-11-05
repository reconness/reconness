using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace ReconNess.Data.Npgsql.Migrations
{
    public partial class RemoveSubdomainTargetRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subdomains_Targets_TargetId",
                table: "Subdomains");

            migrationBuilder.DropIndex(
                name: "IX_Subdomains_TargetId",
                table: "Subdomains");

            migrationBuilder.DropColumn(
                name: "TargetId",
                table: "Subdomains");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
                name: "FK_Subdomains_Targets_TargetId",
                table: "Subdomains",
                column: "TargetId",
                principalTable: "Targets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
