using Microsoft.EntityFrameworkCore.Migrations;

namespace PantryPlanner.Migrations
{
    public partial class AddCreatedByUserIdForeignKeyToKitchen : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Kitchen_CreatedByUserId",
                schema: "app",
                table: "Kitchen",
                column: "CreatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "UserToKitchenFK",
                schema: "app",
                table: "Kitchen",
                column: "CreatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "UserToKitchenFK",
                schema: "app",
                table: "Kitchen");

            migrationBuilder.DropIndex(
                name: "IX_Kitchen_CreatedByUserId",
                schema: "app",
                table: "Kitchen");
        }
    }
}
