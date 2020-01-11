using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ReconNess.Data.Npgsql.Migrations
{
    public partial class AddLabelColors : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "Label",
                nullable: true);

            migrationBuilder.Sql("Delete Label");

            migrationBuilder.InsertData(
                table: "Label",
                columns: new[] { "Id", "Color", "CreatedAt", "Deleted", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("aadb4227-8350-4522-bba4-dc23e3bfa38f"), "#0000FF", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Checking", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("3c6466ae-dfb9-4246-9d2c-2b2bf4dacdda"), "#FF0000", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Vulnerable", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("927c6d1c-9c7f-40d8-87df-888f1c6d8adb"), "#FF8C00", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Interesting", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("86492805-95db-4262-8f9f-66f5b3bd5006"), "#008000", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Bounty", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("f51c45d7-1c54-4fe2-a525-289017092304"), "#A9A9A9", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Ignore", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Label",
                keyColumn: "Id",
                keyValue: new Guid("3c6466ae-dfb9-4246-9d2c-2b2bf4dacdda"));

            migrationBuilder.DeleteData(
                table: "Label",
                keyColumn: "Id",
                keyValue: new Guid("86492805-95db-4262-8f9f-66f5b3bd5006"));

            migrationBuilder.DeleteData(
                table: "Label",
                keyColumn: "Id",
                keyValue: new Guid("927c6d1c-9c7f-40d8-87df-888f1c6d8adb"));

            migrationBuilder.DeleteData(
                table: "Label",
                keyColumn: "Id",
                keyValue: new Guid("aadb4227-8350-4522-bba4-dc23e3bfa38f"));

            migrationBuilder.DeleteData(
                table: "Label",
                keyColumn: "Id",
                keyValue: new Guid("f51c45d7-1c54-4fe2-a525-289017092304"));

            migrationBuilder.DropColumn(
                name: "Color",
                table: "Label");
        }
    }
}
