using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace ReconNess.Infrastructure.DataAccess.Migrations
{
    public partial class AddDeleteCascade : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Note_Subdomains_SubdomainId",
                table: "Note");

            migrationBuilder.DropForeignKey(
                name: "FK_RootDomains_Targets_TargetId",
                table: "RootDomains");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceHttpDirectory_ServicesHttp_ServiceHttpId",
                table: "ServiceHttpDirectory");

            migrationBuilder.DropForeignKey(
                name: "FK_Services_Subdomains_SubdomainId",
                table: "Services");

            migrationBuilder.DropForeignKey(
                name: "FK_Subdomains_RootDomains_RootDomainId",
                table: "Subdomains");

            migrationBuilder.DropForeignKey(
                name: "FK_Subdomains_ServicesHttp_ServiceHttpId",
                table: "Subdomains");

            migrationBuilder.AlterColumn<Guid>(
                name: "RootDomainId",
                table: "Subdomains",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "SubdomainId",
                table: "Services",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ServiceHttpId",
                table: "ServiceHttpDirectory",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "TargetId",
                table: "RootDomains",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Note_Subdomains_SubdomainId",
                table: "Note",
                column: "SubdomainId",
                principalTable: "Subdomains",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RootDomains_Targets_TargetId",
                table: "RootDomains",
                column: "TargetId",
                principalTable: "Targets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceHttpDirectory_ServicesHttp_ServiceHttpId",
                table: "ServiceHttpDirectory",
                column: "ServiceHttpId",
                principalTable: "ServicesHttp",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Services_Subdomains_SubdomainId",
                table: "Services",
                column: "SubdomainId",
                principalTable: "Subdomains",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Subdomains_RootDomains_RootDomainId",
                table: "Subdomains",
                column: "RootDomainId",
                principalTable: "RootDomains",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Subdomains_ServicesHttp_ServiceHttpId",
                table: "Subdomains",
                column: "ServiceHttpId",
                principalTable: "ServicesHttp",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Note_Subdomains_SubdomainId",
                table: "Note");

            migrationBuilder.DropForeignKey(
                name: "FK_RootDomains_Targets_TargetId",
                table: "RootDomains");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceHttpDirectory_ServicesHttp_ServiceHttpId",
                table: "ServiceHttpDirectory");

            migrationBuilder.DropForeignKey(
                name: "FK_Services_Subdomains_SubdomainId",
                table: "Services");

            migrationBuilder.DropForeignKey(
                name: "FK_Subdomains_RootDomains_RootDomainId",
                table: "Subdomains");

            migrationBuilder.DropForeignKey(
                name: "FK_Subdomains_ServicesHttp_ServiceHttpId",
                table: "Subdomains");

            migrationBuilder.AlterColumn<Guid>(
                name: "RootDomainId",
                table: "Subdomains",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<Guid>(
                name: "SubdomainId",
                table: "Services",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<Guid>(
                name: "ServiceHttpId",
                table: "ServiceHttpDirectory",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<Guid>(
                name: "TargetId",
                table: "RootDomains",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AddForeignKey(
                name: "FK_Note_Subdomains_SubdomainId",
                table: "Note",
                column: "SubdomainId",
                principalTable: "Subdomains",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RootDomains_Targets_TargetId",
                table: "RootDomains",
                column: "TargetId",
                principalTable: "Targets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceHttpDirectory_ServicesHttp_ServiceHttpId",
                table: "ServiceHttpDirectory",
                column: "ServiceHttpId",
                principalTable: "ServicesHttp",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Services_Subdomains_SubdomainId",
                table: "Services",
                column: "SubdomainId",
                principalTable: "Subdomains",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Subdomains_RootDomains_RootDomainId",
                table: "Subdomains",
                column: "RootDomainId",
                principalTable: "RootDomains",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Subdomains_ServicesHttp_ServiceHttpId",
                table: "Subdomains",
                column: "ServiceHttpId",
                principalTable: "ServicesHttp",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
