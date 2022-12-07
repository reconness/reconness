using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReconNess.Infrastructure.DataAccess.Migrations
{
    public partial class ChangeNotesRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Note_RootDomainId",
                table: "Note");

            migrationBuilder.DropIndex(
                name: "IX_Note_SubdomainId",
                table: "Note");

            migrationBuilder.RenameColumn(
                name: "Notes",
                table: "Note",
                newName: "CreatedBy");

            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "Note",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TargetId",
                table: "Note",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Note_RootDomainId",
                table: "Note",
                column: "RootDomainId");

            migrationBuilder.CreateIndex(
                name: "IX_Note_SubdomainId",
                table: "Note",
                column: "SubdomainId");

            migrationBuilder.CreateIndex(
                name: "IX_Note_TargetId",
                table: "Note",
                column: "TargetId");

            migrationBuilder.AddForeignKey(
                name: "FK_Note_Targets_TargetId",
                table: "Note",
                column: "TargetId",
                principalTable: "Targets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Note_Targets_TargetId",
                table: "Note");

            migrationBuilder.DropIndex(
                name: "IX_Note_RootDomainId",
                table: "Note");

            migrationBuilder.DropIndex(
                name: "IX_Note_SubdomainId",
                table: "Note");

            migrationBuilder.DropIndex(
                name: "IX_Note_TargetId",
                table: "Note");

            migrationBuilder.DropColumn(
                name: "Comment",
                table: "Note");

            migrationBuilder.DropColumn(
                name: "TargetId",
                table: "Note");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "Note",
                newName: "Notes");

            migrationBuilder.CreateIndex(
                name: "IX_Note_RootDomainId",
                table: "Note",
                column: "RootDomainId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Note_SubdomainId",
                table: "Note",
                column: "SubdomainId",
                unique: true);
        }
    }
}
