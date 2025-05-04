using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KTUN_Final_Year_Project.Migrations
{
    /// <inheritdoc />
    public partial class AddImageUrlToProductsAndFKToImageCache : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           /* migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Products",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: true);
*/
            migrationBuilder.AddColumn<int>(
                name: "ProductID",
                table: "ImageCache",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ImageCache_ProductID",
                table: "ImageCache",
                column: "ProductID");

            migrationBuilder.AddForeignKey(
                name: "FK_ImageCache_Products_ProductID",
                table: "ImageCache",
                column: "ProductID",
                principalTable: "Products",
                principalColumn: "ProductID",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImageCache_Products_ProductID",
                table: "ImageCache");

            migrationBuilder.DropIndex(
                name: "IX_ImageCache_ProductID",
                table: "ImageCache");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ProductID",
                table: "ImageCache");
        }
    }
}
