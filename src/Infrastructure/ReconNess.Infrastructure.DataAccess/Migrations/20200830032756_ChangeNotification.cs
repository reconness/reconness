using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace ReconNess.Infrastructure.DataAccess.Migrations
{
    public partial class ChangeNotification : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AgentNotification");

            migrationBuilder.AddColumn<string>(
                name: "DirectoryPayload",
                table: "Notifications",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HasHttpOpenPayload",
                table: "Notifications",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IpAddressPayload",
                table: "Notifications",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IsAlivePayload",
                table: "Notifications",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NotePayload",
                table: "Notifications",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RootDomainPayload",
                table: "Notifications",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ScreenshotPayload",
                table: "Notifications",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ServicePayload",
                table: "Notifications",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubdomainPayload",
                table: "Notifications",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TakeoverPayload",
                table: "Notifications",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DirectoryPayload",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "HasHttpOpenPayload",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "IpAddressPayload",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "IsAlivePayload",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "NotePayload",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "RootDomainPayload",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "ScreenshotPayload",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "ServicePayload",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "SubdomainPayload",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "TakeoverPayload",
                table: "Notifications");

            migrationBuilder.CreateTable(
                name: "AgentNotification",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AgentRef = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false),
                    DirectoryPayload = table.Column<string>(type: "text", nullable: true),
                    HasHttpOpenPayload = table.Column<string>(type: "text", nullable: true),
                    IpAddressPayload = table.Column<string>(type: "text", nullable: true),
                    IsAlivePayload = table.Column<string>(type: "text", nullable: true),
                    NotePayload = table.Column<string>(type: "text", nullable: true),
                    ServicePayload = table.Column<string>(type: "text", nullable: true),
                    SubdomainPayload = table.Column<string>(type: "text", nullable: true),
                    TakeoverPayload = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgentNotification", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AgentNotification_Agents_AgentRef",
                        column: x => x.AgentRef,
                        principalTable: "Agents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AgentNotification_AgentRef",
                table: "AgentNotification",
                column: "AgentRef",
                unique: true);
        }
    }
}
