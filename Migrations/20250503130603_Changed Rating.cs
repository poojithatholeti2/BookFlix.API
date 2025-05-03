using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BookFlix.API.Migrations
{
    /// <inheritdoc />
    public partial class ChangedRating : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Ratings",
                keyColumn: "Id",
                keyValue: new Guid("1003b2e0-fbee-4a48-86c8-209917e9fa69"));

            migrationBuilder.DeleteData(
                table: "Ratings",
                keyColumn: "Id",
                keyValue: new Guid("7a500d61-2f35-423e-a1e1-e6b58f6c0253"));

            migrationBuilder.DropColumn(
                name: "RatingValue",
                table: "Ratings");

            migrationBuilder.AddColumn<string>(
                name: "RatingName",
                table: "Ratings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Ratings",
                keyColumn: "Id",
                keyValue: new Guid("4bb3890e-2acc-4ebe-9e5f-e0527b4b33cb"),
                column: "RatingName",
                value: "Average");

            migrationBuilder.UpdateData(
                table: "Ratings",
                keyColumn: "Id",
                keyValue: new Guid("91f9aee4-d7d3-4ea1-b4ba-e6c11c37efe3"),
                column: "RatingName",
                value: "Bad");

            migrationBuilder.UpdateData(
                table: "Ratings",
                keyColumn: "Id",
                keyValue: new Guid("a3c7d69e-0c07-47db-a0fe-f7eb6160f568"),
                column: "RatingName",
                value: "Good");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RatingName",
                table: "Ratings");

            migrationBuilder.AddColumn<int>(
                name: "RatingValue",
                table: "Ratings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Ratings",
                keyColumn: "Id",
                keyValue: new Guid("4bb3890e-2acc-4ebe-9e5f-e0527b4b33cb"),
                column: "RatingValue",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Ratings",
                keyColumn: "Id",
                keyValue: new Guid("91f9aee4-d7d3-4ea1-b4ba-e6c11c37efe3"),
                column: "RatingValue",
                value: 3);

            migrationBuilder.UpdateData(
                table: "Ratings",
                keyColumn: "Id",
                keyValue: new Guid("a3c7d69e-0c07-47db-a0fe-f7eb6160f568"),
                column: "RatingValue",
                value: 1);

            migrationBuilder.InsertData(
                table: "Ratings",
                columns: new[] { "Id", "RatingValue" },
                values: new object[,]
                {
                    { new Guid("1003b2e0-fbee-4a48-86c8-209917e9fa69"), 5 },
                    { new Guid("7a500d61-2f35-423e-a1e1-e6b58f6c0253"), 4 }
                });
        }
    }
}
