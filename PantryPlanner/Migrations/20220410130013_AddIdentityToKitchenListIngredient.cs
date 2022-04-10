using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PantryPlanner.Migrations
{
    public partial class AddIdentityToKitchenListIngredient : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KitchenListIngredient",
                schema: "app");

            migrationBuilder.CreateTable(
                name: "KitchenListIngredient",
                schema: "app",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false).Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    KitchenListID = table.Column<long>(nullable: false),
                    IngredientID = table.Column<long>(nullable: false),
                    AddedFromRecipeId = table.Column<long>(nullable: true),
                    Quantity = table.Column<int>(nullable: true),
                    SortOrder = table.Column<int>(nullable: false),
                    IsChecked = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KitchenListRecipe", x => new { x.ID, x.KitchenListID, x.IngredientID });
                    table.ForeignKey(
                        name: "RecipeToListIngredientFK",
                        column: x => x.AddedFromRecipeId,
                        principalSchema: "app",
                        principalTable: "Recipe",
                        principalColumn: "RecipeID",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "IngredientToListIngredientFK",
                        column: x => x.IngredientID,
                        principalSchema: "app",
                        principalTable: "Ingredient",
                        principalColumn: "IngredientID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "KitchenListToIngredientFK",
                        column: x => x.KitchenListID,
                        principalSchema: "app",
                        principalTable: "KitchenList",
                        principalColumn: "KitchenListID",
                        onDelete: ReferentialAction.Cascade);
                });

            //migrationBuilder.AlterColumn<int>(
            //    name: "ID",
            //    schema: "app",
            //    table: "KitchenListIngredient",
            //    nullable: false,
            //    oldClrType: typeof(int))
            //    .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ID",
                schema: "app",
                table: "KitchenListIngredient",
                nullable: false,
                oldClrType: typeof(int))
                .OldAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);
        }
    }
}
