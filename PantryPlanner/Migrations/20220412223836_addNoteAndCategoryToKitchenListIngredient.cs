using Microsoft.EntityFrameworkCore.Migrations;

namespace PantryPlanner.Migrations
{
    public partial class addNoteAndCategoryToKitchenListIngredient : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CategoryId",
                schema: "app",
                table: "KitchenListIngredient",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                schema: "app",
                table: "KitchenListIngredient",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_KitchenListIngredient_CategoryId",
                schema: "app",
                table: "KitchenListIngredient",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_KitchenListIngredient_Category_CategoryId",
                schema: "app",
                table: "KitchenListIngredient",
                column: "CategoryId",
                principalSchema: "app",
                principalTable: "Category",
                principalColumn: "CategoryID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_KitchenListIngredient_Category_CategoryId",
                schema: "app",
                table: "KitchenListIngredient");

            migrationBuilder.DropIndex(
                name: "IX_KitchenListIngredient_CategoryId",
                schema: "app",
                table: "KitchenListIngredient");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                schema: "app",
                table: "KitchenListIngredient");

            migrationBuilder.DropColumn(
                name: "Note",
                schema: "app",
                table: "KitchenListIngredient");
        }
    }
}
