using Microsoft.EntityFrameworkCore.Migrations;

namespace ReconNess.Infrastructure.Data.EF.Npgsql.Migrations
{
    public partial class AddAgentTriggerOptions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OnlyIfHasHttpOpen",
                table: "AgentTriggers");

            migrationBuilder.DropColumn(
                name: "OnlyIfIsAlive",
                table: "AgentTriggers");

            migrationBuilder.DropColumn(
                name: "SkipIfRanBefore",
                table: "AgentTriggers");

            migrationBuilder.AddColumn<string>(
                name: "Technology",
                table: "Subdomains",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RootdomainHasBounty",
                table: "AgentTriggers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "RootdomainIncExcName",
                table: "AgentTriggers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RootdomainName",
                table: "AgentTriggers",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SkipIfRunBefore",
                table: "AgentTriggers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SubdomainHasHttpOrHttpsOpen",
                table: "AgentTriggers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SubdomainIP",
                table: "AgentTriggers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubdomainIncExcIP",
                table: "AgentTriggers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubdomainIncExcLabel",
                table: "AgentTriggers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubdomainIncExcName",
                table: "AgentTriggers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubdomainIncExcServicePort",
                table: "AgentTriggers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubdomainIncExcTechnology",
                table: "AgentTriggers",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SubdomainIsAlive",
                table: "AgentTriggers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SubdomainIsMainPortal",
                table: "AgentTriggers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SubdomainLabel",
                table: "AgentTriggers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubdomainName",
                table: "AgentTriggers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubdomainServicePort",
                table: "AgentTriggers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubdomainTechnology",
                table: "AgentTriggers",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "TargetHasBounty",
                table: "AgentTriggers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "TargetIncExcName",
                table: "AgentTriggers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TargetName",
                table: "AgentTriggers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Technology",
                table: "Subdomains");

            migrationBuilder.DropColumn(
                name: "RootdomainHasBounty",
                table: "AgentTriggers");

            migrationBuilder.DropColumn(
                name: "RootdomainIncExcName",
                table: "AgentTriggers");

            migrationBuilder.DropColumn(
                name: "RootdomainName",
                table: "AgentTriggers");

            migrationBuilder.DropColumn(
                name: "SkipIfRunBefore",
                table: "AgentTriggers");

            migrationBuilder.DropColumn(
                name: "SubdomainHasHttpOrHttpsOpen",
                table: "AgentTriggers");

            migrationBuilder.DropColumn(
                name: "SubdomainIP",
                table: "AgentTriggers");

            migrationBuilder.DropColumn(
                name: "SubdomainIncExcIP",
                table: "AgentTriggers");

            migrationBuilder.DropColumn(
                name: "SubdomainIncExcLabel",
                table: "AgentTriggers");

            migrationBuilder.DropColumn(
                name: "SubdomainIncExcName",
                table: "AgentTriggers");

            migrationBuilder.DropColumn(
                name: "SubdomainIncExcServicePort",
                table: "AgentTriggers");

            migrationBuilder.DropColumn(
                name: "SubdomainIncExcTechnology",
                table: "AgentTriggers");

            migrationBuilder.DropColumn(
                name: "SubdomainIsAlive",
                table: "AgentTriggers");

            migrationBuilder.DropColumn(
                name: "SubdomainIsMainPortal",
                table: "AgentTriggers");

            migrationBuilder.DropColumn(
                name: "SubdomainLabel",
                table: "AgentTriggers");

            migrationBuilder.DropColumn(
                name: "SubdomainName",
                table: "AgentTriggers");

            migrationBuilder.DropColumn(
                name: "SubdomainServicePort",
                table: "AgentTriggers");

            migrationBuilder.DropColumn(
                name: "SubdomainTechnology",
                table: "AgentTriggers");

            migrationBuilder.DropColumn(
                name: "TargetHasBounty",
                table: "AgentTriggers");

            migrationBuilder.DropColumn(
                name: "TargetIncExcName",
                table: "AgentTriggers");

            migrationBuilder.DropColumn(
                name: "TargetName",
                table: "AgentTriggers");

            migrationBuilder.AddColumn<bool>(
                name: "OnlyIfHasHttpOpen",
                table: "AgentTriggers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "OnlyIfIsAlive",
                table: "AgentTriggers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SkipIfRanBefore",
                table: "AgentTriggers",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
