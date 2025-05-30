using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KTUN_Final_Year_Project.Migrations
{
    /// <inheritdoc />
    public partial class AddUserFollowedSuppliers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Table and indexes are already created via SQL script.
            // This migration is to make EF Core aware of the schema state.

            // migrationBuilder.CreateTable(
            //     name: "UserFollowedSuppliers",
            //     columns: table => new
            //     {
            //         UserFollowedSupplierID = table.Column<int>(type: "int", nullable: false)
            //             .Annotation("SqlServer:Identity", "1, 1"),
            //         UserID = table.Column<int>(type: "int", nullable: false),
            //         SupplierID = table.Column<int>(type: "int", nullable: false),
            //         FollowedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
            //     },
            //     constraints: table =>
            //     {
            //         table.PrimaryKey("PK_UserFollowedSuppliers", x => x.UserFollowedSupplierID);
            //         table.ForeignKey(
            //             name: "FK_UserFollowedSuppliers_Suppliers_SupplierID",
            //             column: x => x.SupplierID,
            //             principalTable: "Suppliers",
            //             principalColumn: "SupplierID",
            //             onDelete: ReferentialAction.Cascade);
            //         table.ForeignKey(
            //             name: "FK_UserFollowedSuppliers_Users_UserID",
            //             column: x => x.UserID,
            //             principalTable: "Users",
            //             principalColumn: "Id",
            //             onDelete: ReferentialAction.Cascade);
            //     });

            // migrationBuilder.CreateIndex(
            //     name: "IX_UserFollowedSuppliers_SupplierID",
            //     table: "UserFollowedSuppliers",
            //     column: "SupplierID");

            // migrationBuilder.CreateIndex(
            //     name: "UQ_User_Supplier_Follow",
            //     table: "UserFollowedSuppliers",
            //     columns: new[] { "UserID", "SupplierID" },
            //     unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserFollowedSuppliers");
        }
    }
} 