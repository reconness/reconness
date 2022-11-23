using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReconNess.Infrastructure.Data.EF.Npgsql.Migrations
{
    public partial class SetNullEventTrack : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventTracks_Agents_AgentId",
                table: "EventTracks");

            migrationBuilder.DropForeignKey(
                name: "FK_EventTracks_RootDomains_RootDomainId",
                table: "EventTracks");

            migrationBuilder.DropForeignKey(
                name: "FK_EventTracks_Subdomains_SubdomainId",
                table: "EventTracks");

            migrationBuilder.DropForeignKey(
                name: "FK_EventTracks_Targets_TargetId",
                table: "EventTracks");

            migrationBuilder.AddForeignKey(
                name: "FK_EventTracks_Agents_AgentId",
                table: "EventTracks",
                column: "AgentId",
                principalTable: "Agents",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_EventTracks_RootDomains_RootDomainId",
                table: "EventTracks",
                column: "RootDomainId",
                principalTable: "RootDomains",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_EventTracks_Subdomains_SubdomainId",
                table: "EventTracks",
                column: "SubdomainId",
                principalTable: "Subdomains",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_EventTracks_Targets_TargetId",
                table: "EventTracks",
                column: "TargetId",
                principalTable: "Targets",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventTracks_Agents_AgentId",
                table: "EventTracks");

            migrationBuilder.DropForeignKey(
                name: "FK_EventTracks_RootDomains_RootDomainId",
                table: "EventTracks");

            migrationBuilder.DropForeignKey(
                name: "FK_EventTracks_Subdomains_SubdomainId",
                table: "EventTracks");

            migrationBuilder.DropForeignKey(
                name: "FK_EventTracks_Targets_TargetId",
                table: "EventTracks");

            migrationBuilder.AddForeignKey(
                name: "FK_EventTracks_Agents_AgentId",
                table: "EventTracks",
                column: "AgentId",
                principalTable: "Agents",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EventTracks_RootDomains_RootDomainId",
                table: "EventTracks",
                column: "RootDomainId",
                principalTable: "RootDomains",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EventTracks_Subdomains_SubdomainId",
                table: "EventTracks",
                column: "SubdomainId",
                principalTable: "Subdomains",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EventTracks_Targets_TargetId",
                table: "EventTracks",
                column: "TargetId",
                principalTable: "Targets",
                principalColumn: "Id");
        }
    }
}
