using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KTUN_Final_Year_Project.Migrations
{
    /// <inheritdoc />
    public partial class AddFavoriteListFunctionality : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           // migrationBuilder.CreateTable(
           //     name: "FavoriteLists",
           //     columns: table => new
           //     {
           //         FavoriteListID = table.Column<int>(type: "int", nullable: false)
           //             .Annotation("SqlServer:Identity", "1, 1"),
           //         UserID = table.Column<int>(type: "int", nullable: false),
           //         ListName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
           //         IsPrivate = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
           //         CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
           //         Status = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
           //     },
           //     constraints: table =>
           //     {
           //         table.PrimaryKey("PK_FavoriteLists", x => x.FavoriteListID);
           //         table.ForeignKey(
           //             name: "FK_FavoriteLists_Users_UserID",
           //             column: x => x.UserID,
           //             principalTable: "Users",
           //             principalColumn: "Id",
           //             onDelete: ReferentialAction.Cascade);
           //     });

           // migrationBuilder.CreateTable(
           //     name: "FavoriteListItems",
           //     columns: table => new
           //     {
           //         FavoriteListItemID = table.Column<int>(type: "int", nullable: false)
           //             .Annotation("SqlServer:Identity", "1, 1"),
           //         FavoriteListID = table.Column<int>(type: "int", nullable: false),
           //         ProductID = table.Column<int>(type: "int", nullable: false),
           //         AddedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
           //     },
           //     constraints: table =>
           //     {
           //         table.PrimaryKey("PK_FavoriteListItems", x => x.FavoriteListItemID);
           //         table.ForeignKey(
           //             name: "FK_FavoriteListItems_FavoriteLists_FavoriteListID",
           //             column: x => x.FavoriteListID,
           //             principalTable: "FavoriteLists",
           //             principalColumn: "FavoriteListID",
           //             onDelete: ReferentialAction.Cascade);
           //         table.ForeignKey(
           //             name: "FK_FavoriteListItems_Products_ProductID",
           //             column: x => x.ProductID,
           //             principalTable: "Products",
           //             principalColumn: "ProductID",
           //             onDelete: ReferentialAction.Cascade);
           //     });

           // migrationBuilder.CreateIndex(
           //     name: "IX_FavoriteListItems_ProductID",
           //     table: "FavoriteListItems",
           //     column: "ProductID");

           // migrationBuilder.CreateIndex(
           //     name: "UQ_FavoriteListItems_List_Product",
           //     table: "FavoriteListItems",
           //     columns: new[] { "FavoriteListID", "ProductID" },
           //     unique: true);

           // migrationBuilder.CreateIndex(
           //     name: "IX_FavoriteLists_UserID",
           //     table: "FavoriteLists",
           //     column: "UserID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropTable(
            //    name: "FavoriteListItems");

            //migrationBuilder.DropTable(
            //    name: "FavoriteLists");
        }
    }
}
