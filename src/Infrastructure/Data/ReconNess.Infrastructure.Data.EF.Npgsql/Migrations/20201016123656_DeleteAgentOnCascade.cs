using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace ReconNess.Data.Npgsql.Migrations
{
    public partial class DeleteAgentOnCascade : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AgentHistories_Agents_AgentId",
                table: "AgentHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_AgentRuns_Agents_AgentId",
                table: "AgentRuns");

            migrationBuilder.AlterColumn<Guid>(
                name: "AgentId",
                table: "AgentRuns",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "AgentId",
                table: "AgentHistories",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AgentHistories_Agents_AgentId",
                table: "AgentHistories",
                column: "AgentId",
                principalTable: "Agents",
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AgentHistories_Agents_AgentId",
                table: "AgentHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_AgentRuns_Agents_AgentId",
                table: "AgentRuns");

            migrationBuilder.AlterColumn<Guid>(
                name: "AgentId",
                table: "AgentRuns",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<Guid>(
                name: "AgentId",
                table: "AgentHistories",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AddForeignKey(
                name: "FK_AgentHistories_Agents_AgentId",
                table: "AgentHistories",
                column: "AgentId",
                principalTable: "Agents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AgentRuns_Agents_AgentId",
                table: "AgentRuns",
                column: "AgentId",
                principalTable: "Agents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
