using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReconNess.Data.Npgsql.Migrations
{
    public partial class AddColumnServerCountAgentsSetting : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AgentServerCount",
                table: "AgentsSettings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AgentsSettings",
                keyColumn: "Id",
                keyValue: new Guid("ade752b1-af9e-4ba8-5706-35ad1c1e94ee"),
                column: "AgentServerCount",
                value: 1);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AgentServerCount",
                table: "AgentsSettings");
        }
    }
}
