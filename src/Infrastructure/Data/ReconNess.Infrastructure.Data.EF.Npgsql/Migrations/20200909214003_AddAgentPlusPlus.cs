using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace ReconNess.Infrastructure.Data.EF.Npgsql.Migrations
{
    public partial class AddAgentPlusPlus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBySubdomain",
                table: "Agents");

            migrationBuilder.DropColumn(
                name: "NotifyIfAgentDone",
                table: "Agents");

            migrationBuilder.DropColumn(
                name: "NotifyNewFound",
                table: "Agents");

            migrationBuilder.DropColumn(
                name: "OnlyIfHasHttpOpen",
                table: "Agents");

            migrationBuilder.DropColumn(
                name: "OnlyIfIsAlive",
                table: "Agents");

            migrationBuilder.DropColumn(
                name: "SkipIfRanBefore",
                table: "Agents");

            migrationBuilder.AddColumn<Guid>(
                name: "TargetId",
                table: "Subdomains",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastRun",
                table: "Agents",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.CreateTable(
                name: "AgentHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false),
                    ChangeType = table.Column<string>(nullable: true),
                    Username = table.Column<string>(nullable: true),
                    AgentId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgentHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AgentHistories_Agents_AgentId",
                        column: x => x.AgentId,
                        principalTable: "Agents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AgentRuns",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Stage = table.Column<int>(nullable: false),
                    TerminalOutput = table.Column<string>(nullable: true),
                    Logs = table.Column<string>(nullable: true),
                    AgentId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgentRuns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AgentRuns_Agents_AgentId",
                        column: x => x.AgentId,
                        principalTable: "Agents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AgentTriggers",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false),
                    OnlyIfIsAlive = table.Column<bool>(nullable: false),
                    OnlyIfHasHttpOpen = table.Column<bool>(nullable: false),
                    SkipIfRanBefore = table.Column<bool>(nullable: false),
                    AgentId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgentTriggers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AgentTriggers_Agents_AgentId",
                        column: x => x.AgentId,
                        principalTable: "Agents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Types",
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
                    table.PrimaryKey("PK_Types", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AgentType",
                columns: table => new
                {
                    AgentId = table.Column<Guid>(nullable: false),
                    TypeId = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false)
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
                name: "IX_Subdomains_TargetId",
                table: "Subdomains",
                column: "TargetId");

            migrationBuilder.CreateIndex(
                name: "IX_AgentHistories_AgentId",
                table: "AgentHistories",
                column: "AgentId");

            migrationBuilder.CreateIndex(
                name: "IX_AgentRuns_AgentId",
                table: "AgentRuns",
                column: "AgentId");

            migrationBuilder.CreateIndex(
                name: "IX_AgentTriggers_AgentId",
                table: "AgentTriggers",
                column: "AgentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AgentType_TypeId",
                table: "AgentType",
                column: "TypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Subdomains_Targets_TargetId",
                table: "Subdomains",
                column: "TargetId",
                principalTable: "Targets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subdomains_Targets_TargetId",
                table: "Subdomains");

            migrationBuilder.DropTable(
                name: "AgentHistories");

            migrationBuilder.DropTable(
                name: "AgentRuns");

            migrationBuilder.DropTable(
                name: "AgentTriggers");

            migrationBuilder.DropTable(
                name: "AgentType");

            migrationBuilder.DropTable(
                name: "Types");

            migrationBuilder.DropIndex(
                name: "IX_Subdomains_TargetId",
                table: "Subdomains");

            migrationBuilder.DropColumn(
                name: "TargetId",
                table: "Subdomains");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastRun",
                table: "Agents",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsBySubdomain",
                table: "Agents",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "NotifyIfAgentDone",
                table: "Agents",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "NotifyNewFound",
                table: "Agents",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "OnlyIfHasHttpOpen",
                table: "Agents",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "OnlyIfIsAlive",
                table: "Agents",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SkipIfRanBefore",
                table: "Agents",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
