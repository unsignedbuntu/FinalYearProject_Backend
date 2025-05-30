using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KTUN_Final_Year_Project.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserEntityForProfileInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // migrationBuilder.AddColumn<string>(
            //     name: "FirstName",
            //     table: "Users",
            //     type: "nvarchar(100)",
            //     maxLength: 100,
            //     nullable: true);
            //
            // migrationBuilder.AddColumn<string>(
            //     name: "LastName",
            //     table: "Users",
            //     type: "nvarchar(100)",
            //     maxLength: 100,
            //     nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // migrationBuilder.DropColumn(
            //     name: "FirstName",
            //     table: "Users");
            //
            // migrationBuilder.DropColumn(
            //     name: "LastName",
            //     table: "Users");
        }
    }
}
