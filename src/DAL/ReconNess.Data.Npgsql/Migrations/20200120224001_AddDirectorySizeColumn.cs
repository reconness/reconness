using Microsoft.EntityFrameworkCore.Migrations;

namespace ReconNess.Data.Npgsql.Migrations
{
    public partial class AddDirectorySizeColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<string>(
                name: "Size",
                table: "ServiceHttpDirectory",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Size",
                table: "ServiceHttpDirectory");
        }
    }
}
