using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace ReconNess.Infrastructure.Data.EF.Npgsql.Migrations
{
    public partial class FixAgentTypeRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AgentType");

            migrationBuilder.DropTable(
                name: "Types");

            migrationBuilder.DropColumn(
                name: "FromAgents",
                table: "Subdomains");

            migrationBuilder.AddColumn<string>(
                name: "AgentsRawBefore",
                table: "Targets",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AgentsRawBefore",
                table: "Subdomains",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AgentsRawBefore",
                table: "RootDomains",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AgentTypeId",
                table: "Agents",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AgentTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgentTypes", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "AgentTypes",
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
                name: "FK_Agents_AgentTypes_AgentTypeId",
                table: "Agents",
                column: "AgentTypeId",
                principalTable: "AgentTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agents_AgentTypes_AgentTypeId",
                table: "Agents");

            migrationBuilder.DropTable(
                name: "AgentTypes");

            migrationBuilder.DropIndex(
                name: "IX_Agents_AgentTypeId",
                table: "Agents");

            migrationBuilder.DropColumn(
                name: "AgentsRawBefore",
                table: "Targets");

            migrationBuilder.DropColumn(
                name: "AgentsRawBefore",
                table: "Subdomains");

            migrationBuilder.DropColumn(
                name: "AgentsRawBefore",
                table: "RootDomains");

            migrationBuilder.DropColumn(
                name: "AgentTypeId",
                table: "Agents");

            migrationBuilder.AddColumn<string>(
                name: "FromAgents",
                table: "Subdomains",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Types",
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
                    table.PrimaryKey("PK_Types", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AgentType",
                columns: table => new
                {
                    AgentId = table.Column<Guid>(type: "uuid", nullable: false),
                    TypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgentType", x => new { x.AgentId, x.TypeId });
                    table.ForeignKey(
                        name: "FK_AgentType_Agents_AgentId",
                        column: x => x.AgentId,
                        principalTable: "Agents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AgentType_Types_TypeId",
                        column: x => x.TypeId,
                        principalTable: "Types",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Types",
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
                name: "IX_AgentType_TypeId",
                table: "AgentType",
                column: "TypeId");
        }
    }
}
