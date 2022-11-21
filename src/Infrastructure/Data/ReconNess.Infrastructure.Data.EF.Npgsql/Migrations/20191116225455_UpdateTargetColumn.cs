using Microsoft.EntityFrameworkCore.Migrations;

namespace ReconNess.Data.Npgsql.Migrations
{
    public partial class UpdateTargetColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BugBountyProgram",
                table: "Targets");

            migrationBuilder.AddColumn<string>(
                name: "BugBountyProgramUrl",
                table: "Targets",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BugBountyProgramUrl",
                table: "Targets");

            migrationBuilder.AddColumn<string>(
                name: "BugBountyProgram",
                table: "Targets",
                type: "text",
                nullable: true);
        }
    }
}
