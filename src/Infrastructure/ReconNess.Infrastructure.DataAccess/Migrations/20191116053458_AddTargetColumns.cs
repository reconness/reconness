using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace ReconNess.Infrastructure.DataAccess.Migrations
{
    public partial class AddTargetColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BugBountyProgram",
                table: "Targets",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InScope",
                table: "Targets",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPrivate",
                table: "Targets",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "OutOfScope",
                table: "Targets",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TargetNote",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false),
                    Note = table.Column<string>(nullable: true),
                    TargetId = table.Column<Guid>(nullable: true)
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
                name: "IX_TargetNote_TargetId",
                table: "TargetNote",
                column: "TargetId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TargetNote");

            migrationBuilder.DropColumn(
                name: "BugBountyProgram",
                table: "Targets");

            migrationBuilder.DropColumn(
                name: "InScope",
                table: "Targets");

            migrationBuilder.DropColumn(
                name: "IsPrivate",
                table: "Targets");

            migrationBuilder.DropColumn(
                name: "OutOfScope",
                table: "Targets");
        }
    }
}
