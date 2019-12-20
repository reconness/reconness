using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ReconNess.Data.Npgsql.Migrations
{
    public partial class AddManyToManyCategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agent_AgentCategory_CategoryId",
                table: "Agent");

            migrationBuilder.DropForeignKey(
                name: "FK_AgentArgs_Agent_AgentId",
                table: "AgentArgs");

            migrationBuilder.DropForeignKey(
                name: "FK_Service_Subdomain_SubdomainId",
                table: "Service");

            migrationBuilder.DropForeignKey(
                name: "FK_Subdomain_IpAddress_IpAddressId",
                table: "Subdomain");

            migrationBuilder.DropForeignKey(
                name: "FK_Subdomain_Target_TargetId",
                table: "Subdomain");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AgentCategory",
                table: "AgentCategory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Target",
                table: "Target");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Subdomain",
                table: "Subdomain");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Service",
                table: "Service");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Agent",
                table: "Agent");

            migrationBuilder.DropIndex(
                name: "IX_Agent_CategoryId",
                table: "Agent");

            migrationBuilder.DeleteData(
                table: "AgentCategory",
                keyColumn: "Id",
                keyValue: new Guid("1ebd0113-d785-49e6-987f-101c1bb4a3b8"));

            migrationBuilder.DeleteData(
                table: "AgentCategory",
                keyColumn: "Id",
                keyValue: new Guid("795e3c29-6fb1-45e9-a462-04c511b78aba"));

            migrationBuilder.DeleteData(
                table: "AgentCategory",
                keyColumn: "Id",
                keyValue: new Guid("ef4de40a-4893-474e-a18d-1ee8eefe04f1"));

            migrationBuilder.DropColumn(
                name: "Id",
                table: "AgentCategory");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "AgentCategory");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Agent");

            migrationBuilder.DropColumn(
                name: "CmdName",
                table: "Agent");

            migrationBuilder.RenameTable(
                name: "Target",
                newName: "Targets");

            migrationBuilder.RenameTable(
                name: "Subdomain",
                newName: "Subdomains");

            migrationBuilder.RenameTable(
                name: "Service",
                newName: "Services");

            migrationBuilder.RenameTable(
                name: "Agent",
                newName: "Agents");

            migrationBuilder.RenameIndex(
                name: "IX_Subdomain_TargetId",
                table: "Subdomains",
                newName: "IX_Subdomains_TargetId");

            migrationBuilder.RenameIndex(
                name: "IX_Subdomain_IpAddressId",
                table: "Subdomains",
                newName: "IX_Subdomains_IpAddressId");

            migrationBuilder.RenameIndex(
                name: "IX_Service_SubdomainId",
                table: "Services",
                newName: "IX_Services_SubdomainId");

            migrationBuilder.AddColumn<Guid>(
                name: "AgentId",
                table: "AgentCategory",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId",
                table: "AgentCategory",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Cmd",
                table: "Agents",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Agents",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AgentCategory",
                table: "AgentCategory",
                columns: new[] { "AgentId", "CategoryId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Targets",
                table: "Targets",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Subdomains",
                table: "Subdomains",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Services",
                table: "Services",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Agents",
                table: "Agents",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Categories",
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
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedAt", "Deleted", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("0ddfce66-458c-4894-bb1e-16206de35371"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Enum Subdomain Active", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("47ddb6f9-2321-4c7c-b4f0-ae2b65d77fc1"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Enum Subdomain Passive", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("a2f7b386-6ae6-493e-b0e1-63410faa665d"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Enum Subdomain Brute Force", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AgentCategory_CategoryId",
                table: "AgentCategory",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_AgentArgs_Agents_AgentId",
                table: "AgentArgs",
                column: "AgentId",
                principalTable: "Agents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AgentCategory_Agents_AgentId",
                table: "AgentCategory",
                column: "AgentId",
                principalTable: "Agents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AgentCategory_Categories_CategoryId",
                table: "AgentCategory",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Services_Subdomains_SubdomainId",
                table: "Services",
                column: "SubdomainId",
                principalTable: "Subdomains",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Subdomains_IpAddress_IpAddressId",
                table: "Subdomains",
                column: "IpAddressId",
                principalTable: "IpAddress",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

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
                name: "FK_AgentArgs_Agents_AgentId",
                table: "AgentArgs");

            migrationBuilder.DropForeignKey(
                name: "FK_AgentCategory_Agents_AgentId",
                table: "AgentCategory");

            migrationBuilder.DropForeignKey(
                name: "FK_AgentCategory_Categories_CategoryId",
                table: "AgentCategory");

            migrationBuilder.DropForeignKey(
                name: "FK_Services_Subdomains_SubdomainId",
                table: "Services");

            migrationBuilder.DropForeignKey(
                name: "FK_Subdomains_IpAddress_IpAddressId",
                table: "Subdomains");

            migrationBuilder.DropForeignKey(
                name: "FK_Subdomains_Targets_TargetId",
                table: "Subdomains");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AgentCategory",
                table: "AgentCategory");

            migrationBuilder.DropIndex(
                name: "IX_AgentCategory_CategoryId",
                table: "AgentCategory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Targets",
                table: "Targets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Subdomains",
                table: "Subdomains");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Services",
                table: "Services");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Agents",
                table: "Agents");

            migrationBuilder.DropColumn(
                name: "AgentId",
                table: "AgentCategory");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "AgentCategory");

            migrationBuilder.DropColumn(
                name: "Cmd",
                table: "Agents");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Agents");

            migrationBuilder.RenameTable(
                name: "Targets",
                newName: "Target");

            migrationBuilder.RenameTable(
                name: "Subdomains",
                newName: "Subdomain");

            migrationBuilder.RenameTable(
                name: "Services",
                newName: "Service");

            migrationBuilder.RenameTable(
                name: "Agents",
                newName: "Agent");

            migrationBuilder.RenameIndex(
                name: "IX_Subdomains_TargetId",
                table: "Subdomain",
                newName: "IX_Subdomain_TargetId");

            migrationBuilder.RenameIndex(
                name: "IX_Subdomains_IpAddressId",
                table: "Subdomain",
                newName: "IX_Subdomain_IpAddressId");

            migrationBuilder.RenameIndex(
                name: "IX_Services_SubdomainId",
                table: "Service",
                newName: "IX_Service_SubdomainId");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "AgentCategory",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "AgentCategory",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId",
                table: "Agent",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CmdName",
                table: "Agent",
                type: "text",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AgentCategory",
                table: "AgentCategory",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Target",
                table: "Target",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Subdomain",
                table: "Subdomain",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Service",
                table: "Service",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Agent",
                table: "Agent",
                column: "Id");

            migrationBuilder.InsertData(
                table: "AgentCategory",
                columns: new[] { "Id", "Category", "CreatedAt", "Deleted", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("1ebd0113-d785-49e6-987f-101c1bb4a3b8"), "Enum Subdomain Active", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("ef4de40a-4893-474e-a18d-1ee8eefe04f1"), "Enum Subdomain Passive", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("795e3c29-6fb1-45e9-a462-04c511b78aba"), "Enum Subdomain Brute Force", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Agent_CategoryId",
                table: "Agent",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Agent_AgentCategory_CategoryId",
                table: "Agent",
                column: "CategoryId",
                principalTable: "AgentCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AgentArgs_Agent_AgentId",
                table: "AgentArgs",
                column: "AgentId",
                principalTable: "Agent",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Service_Subdomain_SubdomainId",
                table: "Service",
                column: "SubdomainId",
                principalTable: "Subdomain",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Subdomain_IpAddress_IpAddressId",
                table: "Subdomain",
                column: "IpAddressId",
                principalTable: "IpAddress",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Subdomain_Target_TargetId",
                table: "Subdomain",
                column: "TargetId",
                principalTable: "Target",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
