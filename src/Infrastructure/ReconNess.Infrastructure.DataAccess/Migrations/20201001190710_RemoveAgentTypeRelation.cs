using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace ReconNess.Infrastructure.DataAccess.Migrations
{
    public partial class RemoveAgentTypeRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agents_AgentType_AgentTypeId",
                table: "Agents");

            migrationBuilder.DropTable(
                name: "AgentType");

            migrationBuilder.DropIndex(
                name: "IX_Agents_AgentTypeId",
                table: "Agents");

            migrationBuilder.DropColumn(
                name: "AgentTypeId",
                table: "Agents");

            migrationBuilder.AddColumn<bool>(
                name: "HasBounty",
                table: "Targets",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasBounty",
                table: "Subdomains",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasBounty",
                table: "RootDomains",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SubdomainHasBounty",
                table: "AgentTriggers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "AgentType",
                table: "Agents",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasBounty",
                table: "Targets");

            migrationBuilder.DropColumn(
                name: "HasBounty",
                table: "Subdomains");

            migrationBuilder.DropColumn(
                name: "HasBounty",
                table: "RootDomains");

            migrationBuilder.DropColumn(
                name: "SubdomainHasBounty",
                table: "AgentTriggers");

            migrationBuilder.DropColumn(
                name: "AgentType",
                table: "Agents");

            migrationBuilder.AddColumn<Guid>(
                name: "AgentTypeId",
                table: "Agents",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AgentType",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgentType", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "AgentType",
                columns: new[] { "Id", "CreatedAt", "Deleted", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("cde752b1-3f9e-4ba8-8706-35ad1c1e94ee"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Target", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("1cad5d54-5764-4366-bb2c-cdabc06b29dc"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "RootDomain", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("e8942a1a-a535-41cd-941b-67bcc89fe5cd"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Subdomain", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("cd4eb533-4c67-44df-826f-8786c0146721"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Directory", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("a0d15eac-ec24-4a10-9c3f-007e66f313fd"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Resource", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Agents_AgentTypeId",
                table: "Agents",
                column: "AgentTypeId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Agents_AgentType_AgentTypeId",
                table: "Agents",
                column: "AgentTypeId",
                principalTable: "AgentType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
