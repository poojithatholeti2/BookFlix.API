using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookFlix.API.Migrations
{
    /// <inheritdoc />
    public partial class Seedingdata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Rating",
                table: "Books",
                newName: "RatingId");

            migrationBuilder.RenameColumn(
                name: "Category",
                table: "Books",
                newName: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Books_CategoryId",
                table: "Books",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Books_RatingId",
                table: "Books",
                column: "RatingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Categories_CategoryId",
                table: "Books",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Ratings_RatingId",
                table: "Books",
                column: "RatingId",
                principalTable: "Ratings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_Categories_CategoryId",
                table: "Books");

            migrationBuilder.DropForeignKey(
                name: "FK_Books_Ratings_RatingId",
                table: "Books");

            migrationBuilder.DropIndex(
                name: "IX_Books_CategoryId",
                table: "Books");

            migrationBuilder.DropIndex(
                name: "IX_Books_RatingId",
                table: "Books");

            migrationBuilder.RenameColumn(
                name: "RatingId",
                table: "Books",
                newName: "Rating");

            migrationBuilder.RenameColumn(
                name: "CategoryId",
                table: "Books",
                newName: "Category");
        }
    }
}
