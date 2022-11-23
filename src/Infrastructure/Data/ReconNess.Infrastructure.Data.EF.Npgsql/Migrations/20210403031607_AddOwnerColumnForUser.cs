using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace ReconNess.Infrastructure.Data.EF.Npgsql.Migrations
{
    public partial class AddOwnerColumnForUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Owner",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("0de752b1-1f3e-4aa8-571a-15ae1c1e94e5"),
                column: "ConcurrencyStamp",
                value: "0de752b1-1f3e-4aa8-571a-15ae1c1e94e5");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("ade752b1-af9e-4ba8-5706-35ad1c1e94ee"),
                column: "ConcurrencyStamp",
                value: "ade752b1-af9e-4ba8-5706-35ad1c1e94ee");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Owner",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("0de752b1-1f3e-4aa8-571a-15ae1c1e94e5"),
                column: "ConcurrencyStamp",
                value: "5cd61deb-d7ab-4be0-8d4e-1aa2b129e16b");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("ade752b1-af9e-4ba8-5706-35ad1c1e94ee"),
                column: "ConcurrencyStamp",
                value: "5400dc80-3ad9-49da-8d67-cd25b9e1732d");
        }
    }
}
