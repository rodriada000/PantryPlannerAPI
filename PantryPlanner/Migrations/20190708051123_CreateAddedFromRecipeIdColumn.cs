using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PantryPlanner.Migrations
{
    public partial class CreateAddedFromRecipeIdColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "DateCreated",
                schema: "app",
                table: "MealPlan",
                nullable: false,
                defaultValueSql: "(getutcdate())",
                oldClrType: typeof(DateTime));

            migrationBuilder.AddColumn<bool>(
                name: "HasAcceptedInvite",
                schema: "app",
                table: "KitchenUser",
                nullable: false,
                defaultValueSql: "((0))");

            migrationBuilder.AddColumn<long>(
                name: "AddedFromRecipeId",
                schema: "app",
                table: "KitchenListIngredient",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastUpdated",
                schema: "app",
                table: "KitchenIngredient",
                nullable: false,
                defaultValueSql: "(getutcdate())",
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateCreated",
                schema: "app",
                table: "Kitchen",
                nullable: false,
                defaultValueSql: "(getutcdate())",
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateAdded",
                schema: "app",
                table: "Ingredient",
                nullable: false,
                defaultValueSql: "(getutcdate())",
                oldClrType: typeof(DateTime));

            migrationBuilder.CreateIndex(
                name: "IX_KitchenListIngredient_AddedFromRecipeId",
                schema: "app",
                table: "KitchenListIngredient",
                column: "AddedFromRecipeId");

            migrationBuilder.AddForeignKey(
                name: "RecipeToListIngredientFK",
                schema: "app",
                table: "KitchenListIngredient",
                column: "AddedFromRecipeId",
                principalSchema: "app",
                principalTable: "Recipe",
                principalColumn: "RecipeID",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "RecipeToListIngredientFK",
                schema: "app",
                table: "KitchenListIngredient");

            migrationBuilder.DropIndex(
                name: "IX_KitchenListIngredient_AddedFromRecipeId",
                schema: "app",
                table: "KitchenListIngredient");

            migrationBuilder.DropColumn(
                name: "HasAcceptedInvite",
                schema: "app",
                table: "KitchenUser");

            migrationBuilder.DropColumn(
                name: "AddedFromRecipeId",
                schema: "app",
                table: "KitchenListIngredient");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateCreated",
                schema: "app",
                table: "MealPlan",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldDefaultValueSql: "(getutcdate())");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastUpdated",
                schema: "app",
                table: "KitchenIngredient",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldDefaultValueSql: "(getutcdate())");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateCreated",
                schema: "app",
                table: "Kitchen",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldDefaultValueSql: "(getutcdate())");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateAdded",
                schema: "app",
                table: "Ingredient",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldDefaultValueSql: "(getutcdate())");
        }
    }
}
