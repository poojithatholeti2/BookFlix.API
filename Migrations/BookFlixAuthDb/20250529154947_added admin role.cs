using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookFlix.API.Migrations.BookFlixAuthDb
{
    /// <inheritdoc />
    public partial class addedadminrole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "e35a0494-19e5-4a76-a48b-0a897478cd45", "e35a0494-19e5-4a76-a48b-0a897478cd45", "Admin", "ADMIN" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e35a0494-19e5-4a76-a48b-0a897478cd45");
        }
    }
}
