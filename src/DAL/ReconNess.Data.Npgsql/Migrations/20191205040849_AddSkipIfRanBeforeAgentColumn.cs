using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ReconNess.Data.Npgsql.Migrations
{
    public partial class AddSkipIfRanBeforeAgentColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("a4fd9387-d51e-4e0d-8053-a5c6a3778a9a"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("c346ffdb-db7d-4c55-920f-86c2f890397d"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("c3d962e5-eeb4-44be-bca9-0076ca9c0613"));

            migrationBuilder.DeleteData(
                table: "Label",
                keyColumn: "Id",
                keyValue: new Guid("6420c610-cde3-48fb-a0d8-3d9d56896c0a"));

            migrationBuilder.DeleteData(
                table: "Label",
                keyColumn: "Id",
                keyValue: new Guid("71869056-34c9-4a56-9935-b01c1808feb9"));

            migrationBuilder.DeleteData(
                table: "Label",
                keyColumn: "Id",
                keyValue: new Guid("92cd1726-d987-406e-899d-e63d7778374c"));

            migrationBuilder.DeleteData(
                table: "Label",
                keyColumn: "Id",
                keyValue: new Guid("df80e098-83c7-4c44-87fd-aad75dc7e35a"));

            migrationBuilder.AddColumn<bool>(
                name: "SkipIfRanBefore",
                table: "Agents",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SkipIfRanBefore",
                table: "Agents");

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedAt", "Deleted", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("c346ffdb-db7d-4c55-920f-86c2f890397d"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Enum Subdomain Active", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("c3d962e5-eeb4-44be-bca9-0076ca9c0613"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Enum Subdomain Passive", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("a4fd9387-d51e-4e0d-8053-a5c6a3778a9a"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Enum Subdomain Brute Force", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Label",
                columns: new[] { "Id", "CreatedAt", "Deleted", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("92cd1726-d987-406e-899d-e63d7778374c"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Ignore", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("71869056-34c9-4a56-9935-b01c1808feb9"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "ToCheck", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("df80e098-83c7-4c44-87fd-aad75dc7e35a"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Checking", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("6420c610-cde3-48fb-a0d8-3d9d56896c0a"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Bounty", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });
        }
    }
}
