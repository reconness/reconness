using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace ReconNess.Data.Npgsql.Migrations
{
    public partial class ChangeAgentNotification : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NotificationPayload",
                table: "Agents");

            migrationBuilder.CreateTable(
                name: "AgentNotification",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false),
                    SubdomainPayload = table.Column<string>(nullable: true),
                    IpAddressPayload = table.Column<string>(nullable: true),
                    IsAlivePayload = table.Column<string>(nullable: true),
                    HasHttpOpenPayload = table.Column<string>(nullable: true),
                    TakeoverPayload = table.Column<string>(nullable: true),
                    DirectoryPayload = table.Column<string>(nullable: true),
                    ServicePayload = table.Column<string>(nullable: true),
                    NotePayload = table.Column<string>(nullable: true),
                    AgentRef = table.Column<Guid>(nullable: false)
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AgentNotification");

            migrationBuilder.AddColumn<string>(
                name: "NotificationPayload",
                table: "Agents",
                type: "text",
                nullable: true);
        }
    }
}
