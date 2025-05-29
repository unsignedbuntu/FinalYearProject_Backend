using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KTUN_Final_Year_Project.Migrations
{
    /// <inheritdoc />
    public partial class AddUserInformationAndAddresses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Tables and indexes are already created via SQL script.
            // This migration is to make EF Core aware of the schema state.

            // migrationBuilder.CreateTable(
            //     name: "UserAddresses",
            //     columns: table => new
            //     {
            //         UserAddressID = table.Column<int>(type: "int", nullable: false)
            //             .Annotation("SqlServer:Identity", "1, 1"),
            //         UserID = table.Column<int>(type: "int", nullable: false),
            //         AddressTitle = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
            //         FullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
            //         PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
            //         City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
            //         District = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
            //         FullAddress = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
            //         IsDefault = table.Column<bool>(type: "bit", nullable: false),
            //         Status = table.Column<bool>(type: "bit", nullable: false)
            //     },
            //     constraints: table =>
            //     {
            //         table.PrimaryKey("PK_UserAddresses", x => x.UserAddressID);
            //         table.ForeignKey(
            //             name: "FK_UserAddresses_Users_UserID",
            //             column: x => x.UserID,
            //             principalTable: "Users",
            //             principalColumn: "Id",
            //             onDelete: ReferentialAction.Cascade);
            //     });

            // migrationBuilder.CreateTable(
            //     name: "UserInformation",
            //     columns: table => new
            //     {
            //         UserInformationID = table.Column<int>(type: "int", nullable: false)
            //             .Annotation("SqlServer:Identity", "1, 1"),
            //         UserID = table.Column<int>(type: "int", nullable: false),
            //         FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
            //         LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
            //         DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true)
            //     },
            //     constraints: table =>
            //     {
            //         table.PrimaryKey("PK_UserInformation", x => x.UserInformationID);
            //         table.ForeignKey(
            //             name: "FK_UserInformation_Users_UserID",
            //             column: x => x.UserID,
            //             principalTable: "Users",
            //             principalColumn: "Id",
            //             onDelete: ReferentialAction.Cascade);
            //     });

            // migrationBuilder.CreateIndex(
            //     name: "IX_UserAddresses_UserID",
            //     table: "UserAddresses",
            //     column: "UserID");

            // migrationBuilder.CreateIndex(
            //     name: "UQ_UserInformation_UserID",
            //     table: "UserInformation",
            //     column: "UserID",
            //     unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserAddresses");

            migrationBuilder.DropTable(
                name: "UserInformation");
        }
    }
}
