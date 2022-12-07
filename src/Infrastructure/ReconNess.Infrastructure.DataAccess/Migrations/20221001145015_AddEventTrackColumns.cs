using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReconNess.Infrastructure.DataAccess.Migrations
{
    public partial class AddEventTrackColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Data",
                table: "EventTracks",
                newName: "Description");

            migrationBuilder.AddColumn<bool>(
                name: "Read",
                table: "EventTracks",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "EventTracks",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Read",
                table: "EventTracks");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "EventTracks");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "EventTracks",
                newName: "Data");
        }
    }
}
