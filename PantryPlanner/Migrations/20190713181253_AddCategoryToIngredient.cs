using Microsoft.EntityFrameworkCore.Migrations;

namespace PantryPlanner.Migrations
{
    public partial class AddCategoryToIngredient : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CategoryId",
                schema: "app",
                table: "Ingredient",
                nullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "CreatedByKitchenId",
                schema: "app",
                table: "Category",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.CreateIndex(
                name: "IX_Ingredient_CategoryId",
                schema: "app",
                table: "Ingredient",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "CategoryToIngredientFK",
                schema: "app",
                table: "Ingredient",
                column: "CategoryId",
                principalSchema: "app",
                principalTable: "Category",
                principalColumn: "CategoryID",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "CategoryToIngredientFK",
                schema: "app",
                table: "Ingredient");

            migrationBuilder.DropIndex(
                name: "IX_Ingredient_CategoryId",
                schema: "app",
                table: "Ingredient");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                schema: "app",
                table: "Ingredient");

            migrationBuilder.AlterColumn<long>(
                name: "CreatedByKitchenId",
                schema: "app",
                table: "Category",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);
        }
    }
}
