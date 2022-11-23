using Microsoft.EntityFrameworkCore.Migrations;

namespace ReconNess.Infrastructure.Data.EF.Npgsql.Migrations
{
    public partial class AddSkipIfRanBeforeAgentColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "SkipIfRanBefore",
                table: "Agents",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SkipIfRanBefore",
                table: "Agents");
        }
    }
}
