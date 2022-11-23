using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace ReconNess.Infrastructure.Data.EF.Npgsql.Migrations
{
    public partial class AddDeleteCascadeNotes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Note_RootDomains_TargetId",
                table: "Note");

            migrationBuilder.DropIndex(
                name: "IX_Note_TargetId",
                table: "Note");

            migrationBuilder.DropColumn(
                name: "TargetId",
                table: "Note");

            migrationBuilder.AddColumn<Guid>(
                name: "RootDomainId",
                table: "Note",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Note_RootDomainId",
                table: "Note",
                column: "RootDomainId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Note_RootDomains_RootDomainId",
                table: "Note",
                column: "RootDomainId",
                principalTable: "RootDomains",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Note_RootDomains_RootDomainId",
                table: "Note");

            migrationBuilder.DropIndex(
                name: "IX_Note_RootDomainId",
                table: "Note");

            migrationBuilder.DropColumn(
                name: "RootDomainId",
                table: "Note");

            migrationBuilder.AddColumn<Guid>(
                name: "TargetId",
                table: "Note",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Note_TargetId",
                table: "Note",
                column: "TargetId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Note_RootDomains_TargetId",
                table: "Note",
                column: "TargetId",
                principalTable: "RootDomains",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
