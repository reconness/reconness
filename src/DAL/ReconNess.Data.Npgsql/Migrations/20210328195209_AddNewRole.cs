using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ReconNess.Data.Npgsql.Migrations
{
    public partial class AddNewRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "CreatedAt", "Deleted", "Name", "NormalizedName", "UpdatedAt" },
                values: new object[] { new Guid("0de752b1-1f3e-4aa8-571a-15ae1c1e94e5"), "5cd61deb-d7ab-4be0-8d4e-1aa2b129e16b", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Member", "MEMBER", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("0de752b1-1f3e-4aa8-571a-15ae1c1e94e5"));
        }
    }
}
