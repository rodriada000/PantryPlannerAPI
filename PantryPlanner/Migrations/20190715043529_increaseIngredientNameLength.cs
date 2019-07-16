using Microsoft.EntityFrameworkCore.Migrations;

namespace PantryPlanner.Migrations
{
    public partial class increaseIngredientNameLength : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "app",
                table: "Ingredient",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 100);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "app",
                table: "Ingredient",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 200);
        }
    }
}
