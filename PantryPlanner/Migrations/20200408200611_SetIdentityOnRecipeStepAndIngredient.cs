using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PantryPlanner.Migrations
{
    public partial class SetIdentityOnRecipeStepAndIngredient : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(schema: "app", name: "RecipeIngredient");
            migrationBuilder.DropTable(schema: "app", name: "RecipeStep");

            migrationBuilder.CreateTable(
                name: "RecipeIngredient",
                schema: "app",
                columns: table => new
                {
                    RecipeIngredientID = table.Column<int>(nullable: false).Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IngredientID = table.Column<long>(nullable: false),
                    RecipeID = table.Column<long>(nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(12, 4)", nullable: false),
                    UnitOfMeasure = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    Method = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    SortOrder = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeIngredient", x => new { x.RecipeIngredientID, x.IngredientID, x.RecipeID });
                    table.ForeignKey(
                        name: "IngredientToRecipeIngredientFK",
                        column: x => x.IngredientID,
                        principalSchema: "app",
                        principalTable: "Ingredient",
                        principalColumn: "IngredientID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "RecipeToRecipeIngredientFK",
                        column: x => x.RecipeID,
                        principalSchema: "app",
                        principalTable: "Recipe",
                        principalColumn: "RecipeID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RecipeStep",
                schema: "app",
                columns: table => new
                {
                    RecipeStepID = table.Column<int>(nullable: false).Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RecipeID = table.Column<long>(nullable: false),
                    Text = table.Column<string>(maxLength: 500, nullable: false),
                    SortOrder = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeStep", x => new { x.RecipeStepID, x.RecipeID });
                    table.ForeignKey(
                        name: "RecipeToStepFK",
                        column: x => x.RecipeID,
                        principalSchema: "app",
                        principalTable: "Recipe",
                        principalColumn: "RecipeID",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "RecipeStepID",
                schema: "app",
                table: "RecipeStep",
                nullable: false,
                oldClrType: typeof(int))
                .OldAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<int>(
                name: "RecipeIngredientID",
                schema: "app",
                table: "RecipeIngredient",
                nullable: false,
                oldClrType: typeof(int))
                .OldAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);
        }
    }
}
