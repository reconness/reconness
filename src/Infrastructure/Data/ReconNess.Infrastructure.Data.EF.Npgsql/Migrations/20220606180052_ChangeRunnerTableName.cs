using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReconNess.Infrastructure.Data.EF.Npgsql.Migrations
{
    public partial class ChangeRunnerTableName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AgentRunnerCommands_AgentRuns_AgentRunnerId",
                table: "AgentRunnerCommands");

            migrationBuilder.DropForeignKey(
                name: "FK_AgentRuns_Agents_AgentId",
                table: "AgentRuns");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AgentRuns",
                table: "AgentRuns");

            migrationBuilder.RenameTable(
                name: "AgentRuns",
                newName: "AgentRunners");

            migrationBuilder.RenameIndex(
                name: "IX_AgentRuns_AgentId",
                table: "AgentRunners",
                newName: "IX_AgentRunners_AgentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AgentRunners",
                table: "AgentRunners",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AgentRunnerCommands_AgentRunners_AgentRunnerId",
                table: "AgentRunnerCommands",
                column: "AgentRunnerId",
                principalTable: "AgentRunners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AgentRunners_Agents_AgentId",
                table: "AgentRunners",
                column: "AgentId",
                principalTable: "Agents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AgentRunnerCommands_AgentRunners_AgentRunnerId",
                table: "AgentRunnerCommands");

            migrationBuilder.DropForeignKey(
                name: "FK_AgentRunners_Agents_AgentId",
                table: "AgentRunners");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AgentRunners",
                table: "AgentRunners");

            migrationBuilder.RenameTable(
                name: "AgentRunners",
                newName: "AgentRuns");

            migrationBuilder.RenameIndex(
                name: "IX_AgentRunners_AgentId",
                table: "AgentRuns",
                newName: "IX_AgentRuns_AgentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AgentRuns",
                table: "AgentRuns",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AgentRunnerCommands_AgentRuns_AgentRunnerId",
                table: "AgentRunnerCommands",
                column: "AgentRunnerId",
                principalTable: "AgentRuns",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AgentRuns_Agents_AgentId",
                table: "AgentRuns",
                column: "AgentId",
                principalTable: "Agents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
