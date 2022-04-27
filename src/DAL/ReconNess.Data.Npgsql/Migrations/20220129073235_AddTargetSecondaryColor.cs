using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReconNess.Data.Npgsql.Migrations
{
    public partial class AddTargetSecondaryColor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SecondaryColor",
                table: "Targets",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SecondaryColor",
                table: "Targets");
        }
    }
}
