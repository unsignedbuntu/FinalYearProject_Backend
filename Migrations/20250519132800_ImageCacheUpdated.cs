using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KTUN_Final_Year_Project.Migrations
{
    /// <inheritdoc />
    public partial class ImageCacheUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ImageCache_ProductID",
                table: "ImageCache");

          //  migrationBuilder.DropColumn(
          //      name: "PageID",
          //      table: "ImageCache");

         //   migrationBuilder.RenameIndex(
         //       name: "IX_ImageCache_HashValue",
         //       table: "ImageCache",
         //       newName: "IX_ImageCache_HashValue_Unique");

            migrationBuilder.CreateIndex(
                name: "IX_ImageCache_ProductID",
                table: "ImageCache",
                column: "ProductID",
                filter: "[ProductID] IS NOT NULL");

            // migrationBuilder.CreateIndex(
            //     name: "IX_ImageCache_SupplierID",
            //     table: "ImageCache",
            //     column: "SupplierID",
            //     filter: "[SupplierID] IS NOT NULL");

            // migrationBuilder.AddForeignKey(
            //     name: "FK_ImageCache_Suppliers_SupplierID",
            //     table: "ImageCache",
            //     column: "SupplierID",
            //     principalTable: "Suppliers",
            //     principalColumn: "SupplierID",
            //     onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImageCache_Suppliers_SupplierID",
                table: "ImageCache");

            migrationBuilder.DropIndex(
                name: "IX_ImageCache_ProductID",
                table: "ImageCache");

            migrationBuilder.DropIndex(
                name: "IX_ImageCache_SupplierID",
                table: "ImageCache");

            migrationBuilder.DropColumn(
                name: "SupplierID",
                table: "ImageCache");

            migrationBuilder.RenameIndex(
                name: "IX_ImageCache_HashValue_Unique",
                table: "ImageCache",
                newName: "IX_ImageCache_HashValue");

            migrationBuilder.AddColumn<string>(
                name: "PageID",
                table: "ImageCache",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ImageCache_ProductID",
                table: "ImageCache",
                column: "ProductID");
        }
    }
}
