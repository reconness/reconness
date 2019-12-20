using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ReconNess.Data.Npgsql.Migrations
{
    public partial class AddAgentCategorySeeding : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AgentCategory",
                columns: new[] { "Id", "Category", "CreatedAt", "Deleted", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("1ebd0113-d785-49e6-987f-101c1bb4a3b8"), "Enum Subdomain Active", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("ef4de40a-4893-474e-a18d-1ee8eefe04f1"), "Enum Subdomain Passive", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("795e3c29-6fb1-45e9-a462-04c511b78aba"), "Enum Subdomain Brute Force", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
        }
    }
}
