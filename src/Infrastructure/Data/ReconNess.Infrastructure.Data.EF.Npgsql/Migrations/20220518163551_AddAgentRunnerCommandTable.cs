using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReconNess.Infrastructure.Data.EF.Npgsql.Migrations
{
    public partial class AddAgentRunnerCommandTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AgentRunnerOutputs");

            migrationBuilder.AddColumn<bool>(
                name: "ActivateNotification",
                table: "AgentRuns",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "AgentRunnerType",
                table: "AgentRuns",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "AllowSkip",
                table: "AgentRuns",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Total",
                table: "AgentRuns",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "AgentRunnerCommands",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Command = table.Column<string>(type: "text", nullable: true),
                    Number = table.Column<int>(type: "integer", nullable: false),
                    Server = table.Column<int>(type: "integer", nullable: false),
                    AgentRunnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgentRunnerCommands", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AgentRunnerCommands_AgentRuns_AgentRunnerId",
                        column: x => x.AgentRunnerId,
                        principalTable: "AgentRuns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AgentRunnerCommandOutputs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Output = table.Column<string>(type: "text", nullable: true),
                    AgentRunnerCommandId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgentRunnerCommandOutputs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AgentRunnerCommandOutputs_AgentRunnerCommands_AgentRunnerCo~",
                        column: x => x.AgentRunnerCommandId,
                        principalTable: "AgentRunnerCommands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AgentRunnerCommandOutputs_AgentRunnerCommandId",
                table: "AgentRunnerCommandOutputs",
                column: "AgentRunnerCommandId");

            migrationBuilder.CreateIndex(
                name: "IX_AgentRunnerCommands_AgentRunnerId",
                table: "AgentRunnerCommands",
                column: "AgentRunnerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AgentRunnerCommandOutputs");

            migrationBuilder.DropTable(
                name: "AgentRunnerCommands");

            migrationBuilder.DropColumn(
                name: "ActivateNotification",
                table: "AgentRuns");

            migrationBuilder.DropColumn(
                name: "AgentRunnerType",
                table: "AgentRuns");

            migrationBuilder.DropColumn(
                name: "AllowSkip",
                table: "AgentRuns");

            migrationBuilder.DropColumn(
                name: "Total",
                table: "AgentRuns");

            migrationBuilder.CreateTable(
                name: "AgentRunnerOutputs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AgentRunnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false),
                    Output = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgentRunnerOutputs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AgentRunnerOutputs_AgentRuns_AgentRunnerId",
                        column: x => x.AgentRunnerId,
                        principalTable: "AgentRuns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AgentRunnerOutputs_AgentRunnerId",
                table: "AgentRunnerOutputs",
                column: "AgentRunnerId");
        }
    }
}
