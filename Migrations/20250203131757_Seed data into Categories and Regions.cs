using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BookFlix.API.Migrations
{
    /// <inheritdoc />
    public partial class SeeddataintoCategoriesandRegions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReleaseTimes");

            migrationBuilder.DropColumn(
                name: "ReleaseTime",
                table: "Books");

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Title" },
                values: new object[,]
                {
                    { new Guid("17ea39ed-3066-44f6-a0c1-d97be6b15de9"), "Literature" },
                    { new Guid("2fa56e3d-d9fb-4453-824a-9094580e5d52"), "Finance" },
                    { new Guid("3edb015b-ec4e-45df-8390-b9c66281ab3f"), "History" },
                    { new Guid("bc54bb0c-1d77-48c2-95ea-fd55f691db4c"), "Fiction" },
                    { new Guid("f0e0164e-a932-4a8b-ba41-291df0d439d5"), "Technology" }
                });

            migrationBuilder.InsertData(
                table: "Ratings",
                columns: new[] { "Id", "RatingValue" },
                values: new object[,]
                {
                    { new Guid("1003b2e0-fbee-4a48-86c8-209917e9fa69"), 5 },
                    { new Guid("4bb3890e-2acc-4ebe-9e5f-e0527b4b33cb"), 2 },
                    { new Guid("7a500d61-2f35-423e-a1e1-e6b58f6c0253"), 4 },
                    { new Guid("91f9aee4-d7d3-4ea1-b4ba-e6c11c37efe3"), 3 },
                    { new Guid("a3c7d69e-0c07-47db-a0fe-f7eb6160f568"), 1 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("17ea39ed-3066-44f6-a0c1-d97be6b15de9"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("2fa56e3d-d9fb-4453-824a-9094580e5d52"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("3edb015b-ec4e-45df-8390-b9c66281ab3f"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("bc54bb0c-1d77-48c2-95ea-fd55f691db4c"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("f0e0164e-a932-4a8b-ba41-291df0d439d5"));

            migrationBuilder.DeleteData(
                table: "Ratings",
                keyColumn: "Id",
                keyValue: new Guid("1003b2e0-fbee-4a48-86c8-209917e9fa69"));

            migrationBuilder.DeleteData(
                table: "Ratings",
                keyColumn: "Id",
                keyValue: new Guid("4bb3890e-2acc-4ebe-9e5f-e0527b4b33cb"));

            migrationBuilder.DeleteData(
                table: "Ratings",
                keyColumn: "Id",
                keyValue: new Guid("7a500d61-2f35-423e-a1e1-e6b58f6c0253"));

            migrationBuilder.DeleteData(
                table: "Ratings",
                keyColumn: "Id",
                keyValue: new Guid("91f9aee4-d7d3-4ea1-b4ba-e6c11c37efe3"));

            migrationBuilder.DeleteData(
                table: "Ratings",
                keyColumn: "Id",
                keyValue: new Guid("a3c7d69e-0c07-47db-a0fe-f7eb6160f568"));

            migrationBuilder.AddColumn<Guid>(
                name: "ReleaseTime",
                table: "Books",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "ReleaseTimes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TimeFrame = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReleaseTimes", x => x.Id);
                });
        }
    }
}
