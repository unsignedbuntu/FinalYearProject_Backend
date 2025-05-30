using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KTUN_Final_Year_Project.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderItemToReviewsAndRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // migrationBuilder.AddColumn<int>(
            //     name: "OrderItemID",
            //     table: "Reviews",
            //     type: "int",
            //     nullable: true);

            // migrationBuilder.CreateIndex(
            //     name: "IX_Reviews_OrderItemID",
            //     table: "Reviews",
            //     column: "OrderItemID");

            // migrationBuilder.AddForeignKey(
            //     name: "FK_Reviews_OrderItems_OrderItemID",
            //     table: "Reviews",
            //     column: "OrderItemID",
            //     principalTable: "OrderItems",
            //     principalColumn: "OrderItemID",
            //     onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_OrderItems_OrderItemID",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_OrderItemID",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "OrderItemID",
                table: "Reviews");
        }
    }
}
