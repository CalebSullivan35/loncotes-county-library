using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace loncotes_county_library.Migrations
{
    public partial class updatecreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Checkouts",
                columns: new[] { "Id", "CheckoutDate", "MaterialId", "PatronId", "ReturnDate" },
                values: new object[,]
                {
                    { 1, new DateTime(2023, 9, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1, null },
                    { 2, new DateTime(2023, 9, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, 1, null },
                    { 3, new DateTime(2023, 9, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 2, null }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Checkouts",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Checkouts",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Checkouts",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
