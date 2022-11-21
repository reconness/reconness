using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace ReconNess.Data.Npgsql.Migrations
{
    public partial class UpdateNoteTable2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Note_Subdomains_SubdomainRef",
                table: "Note");

            migrationBuilder.DropForeignKey(
                name: "FK_Note_Targets_TargetRef",
                table: "Note");

            migrationBuilder.DropIndex(
                name: "IX_Note_SubdomainRef",
                table: "Note");

            migrationBuilder.DropIndex(
                name: "IX_Note_TargetRef",
                table: "Note");

            migrationBuilder.DropColumn(
                name: "SubdomainRef",
                table: "Note");

            migrationBuilder.DropColumn(
                name: "TargetRef",
                table: "Note");

            migrationBuilder.AddColumn<Guid>(
                name: "SubdomainId",
                table: "Note",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TargetId",
                table: "Note",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Note_SubdomainId",
                table: "Note",
                column: "SubdomainId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Note_TargetId",
                table: "Note",
                column: "TargetId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Note_Subdomains_SubdomainId",
                table: "Note",
                column: "SubdomainId",
                principalTable: "Subdomains",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Note_Targets_TargetId",
                table: "Note",
                column: "TargetId",
                principalTable: "Targets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Note_Subdomains_SubdomainId",
                table: "Note");

            migrationBuilder.DropForeignKey(
                name: "FK_Note_Targets_TargetId",
                table: "Note");

            migrationBuilder.DropIndex(
                name: "IX_Note_SubdomainId",
                table: "Note");

            migrationBuilder.DropIndex(
                name: "IX_Note_TargetId",
                table: "Note");

            migrationBuilder.DropColumn(
                name: "SubdomainId",
                table: "Note");

            migrationBuilder.DropColumn(
                name: "TargetId",
                table: "Note");

            migrationBuilder.AddColumn<Guid>(
                name: "SubdomainRef",
                table: "Note",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TargetRef",
                table: "Note",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

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

            migrationBuilder.AddForeignKey(
                name: "FK_Note_Subdomains_SubdomainRef",
                table: "Note",
                column: "SubdomainRef",
                principalTable: "Subdomains",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Note_Targets_TargetRef",
                table: "Note",
                column: "TargetRef",
                principalTable: "Targets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
