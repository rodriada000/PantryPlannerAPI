using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PantryPlanner.Migrations
{
    public partial class InitializeTablesAndPrimaryKeysToAspNetUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "app");

            migrationBuilder.CreateTable(
                name: "CategoryType",
                schema: "app",
                columns: table => new
                {
                    CategoryTypeID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryType", x => x.CategoryTypeID);
                });

            migrationBuilder.CreateTable(
                name: "Kitchen",
                schema: "app",
                columns: table => new
                {
                    KitchenID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UniquePublicGuid = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    Description = table.Column<string>(maxLength: 255, nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false, defaultValueSql: "(getutcdate())"),
                    CreatedByUserId = table.Column<string>(maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kitchen", x => x.KitchenID);
                });

            migrationBuilder.CreateTable(
                name: "Recipe",
                schema: "app",
                columns: table => new
                {
                    RecipeID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedByUserID = table.Column<string>(nullable: true),
                    RecipeUrl = table.Column<string>(unicode: false, maxLength: 255, nullable: true),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    Description = table.Column<string>(maxLength: 500, nullable: false),
                    PrepTime = table.Column<int>(nullable: true),
                    CookTime = table.Column<int>(nullable: true),
                    ServingSize = table.Column<string>(maxLength: 50, nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false, defaultValueSql: "(getutcdate())"),
                    IsPublic = table.Column<bool>(nullable: false, defaultValueSql: "((1))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recipe", x => x.RecipeID);
                    table.ForeignKey(
                        name: "UserToRecipeFK",
                        column: x => x.CreatedByUserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Category",
                schema: "app",
                columns: table => new
                {
                    CategoryID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CategoryTypeID = table.Column<int>(nullable: true),
                    CreatedByKitchenId = table.Column<long>(nullable: true),
                    Name = table.Column<string>(unicode: false, maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.CategoryID);
                    table.ForeignKey(
                        name: "TypeToCategoryFK",
                        column: x => x.CategoryTypeID,
                        principalSchema: "app",
                        principalTable: "CategoryType",
                        principalColumn: "CategoryTypeID",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "KitchenToCategoryFK",
                        column: x => x.CreatedByKitchenId,
                        principalSchema: "app",
                        principalTable: "Kitchen",
                        principalColumn: "KitchenID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "KitchenUser",
                schema: "app",
                columns: table => new
                {
                    KitchenUserID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserID = table.Column<string>(nullable: false),
                    KitchenID = table.Column<long>(nullable: false),
                    IsOwner = table.Column<bool>(nullable: false),
                    HasAcceptedInvite = table.Column<bool>(nullable: false, defaultValueSql: "((0))"),
                    DateAdded = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KitchenUser", x => x.KitchenUserID);
                    table.ForeignKey(
                        name: "KitchenToKitchenUserFK",
                        column: x => x.KitchenID,
                        principalSchema: "app",
                        principalTable: "Kitchen",
                        principalColumn: "KitchenID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "UserToKitchenUserFK",
                        column: x => x.UserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RecipeStep",
                schema: "app",
                columns: table => new
                {
                    RecipeStepID = table.Column<int>(nullable: false),
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

            migrationBuilder.CreateTable(
                name: "Ingredient",
                schema: "app",
                columns: table => new
                {
                    IngredientID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AddedByUserID = table.Column<string>(nullable: true),
                    CategoryId = table.Column<long>(nullable: true),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    Description = table.Column<string>(maxLength: 100, nullable: true),
                    PreviewPicture = table.Column<byte[]>(type: "image", nullable: true),
                    DateAdded = table.Column<DateTime>(nullable: false, defaultValueSql: "(getutcdate())"),
                    IsPublic = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ingredient", x => x.IngredientID);
                    table.ForeignKey(
                        name: "UserToIngredientFK",
                        column: x => x.AddedByUserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "CategoryToIngredientFK",
                        column: x => x.CategoryId,
                        principalSchema: "app",
                        principalTable: "Category",
                        principalColumn: "CategoryID",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "KitchenList",
                schema: "app",
                columns: table => new
                {
                    KitchenListID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    KitchenID = table.Column<long>(nullable: false),
                    CategoryID = table.Column<long>(nullable: true),
                    Name = table.Column<string>(unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KitchenList", x => x.KitchenListID);
                    table.ForeignKey(
                        name: "CategoryToListFK",
                        column: x => x.CategoryID,
                        principalSchema: "app",
                        principalTable: "Category",
                        principalColumn: "CategoryID",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "KitchenToListFK",
                        column: x => x.KitchenID,
                        principalSchema: "app",
                        principalTable: "Kitchen",
                        principalColumn: "KitchenID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KitchenRecipe",
                schema: "app",
                columns: table => new
                {
                    KitchenRecipeID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    KitchenID = table.Column<long>(nullable: false),
                    RecipeID = table.Column<long>(nullable: false),
                    CategoryID = table.Column<long>(nullable: true),
                    IsFavorite = table.Column<bool>(nullable: false),
                    Note = table.Column<string>(unicode: false, maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KitchenRecipe", x => new { x.KitchenRecipeID, x.KitchenID, x.RecipeID });
                    table.ForeignKey(
                        name: "CategoryToKitchenRecipeFK",
                        column: x => x.CategoryID,
                        principalSchema: "app",
                        principalTable: "Category",
                        principalColumn: "CategoryID",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "KitchenToKitchenRecipeFK",
                        column: x => x.KitchenID,
                        principalSchema: "app",
                        principalTable: "Kitchen",
                        principalColumn: "KitchenID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "RecipeToKitchenRecipeFK",
                        column: x => x.RecipeID,
                        principalSchema: "app",
                        principalTable: "Recipe",
                        principalColumn: "RecipeID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MealPlan",
                schema: "app",
                columns: table => new
                {
                    MealPlanID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    KitchenID = table.Column<long>(nullable: false),
                    CreatedByKitchenUserID = table.Column<long>(nullable: true),
                    CategoryID = table.Column<long>(nullable: true),
                    Name = table.Column<string>(unicode: false, maxLength: 50, nullable: false),
                    Description = table.Column<string>(maxLength: 255, nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false, defaultValueSql: "(getutcdate())"),
                    SortOrder = table.Column<int>(nullable: false),
                    IsFavorite = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MealPlan", x => x.MealPlanID);
                    table.ForeignKey(
                        name: "CategoryToMealPlanFK",
                        column: x => x.CategoryID,
                        principalSchema: "app",
                        principalTable: "Category",
                        principalColumn: "CategoryID",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "KitchenUserToMealPlanFK",
                        column: x => x.CreatedByKitchenUserID,
                        principalSchema: "app",
                        principalTable: "KitchenUser",
                        principalColumn: "KitchenUserID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "KitchenToMealPlanFK",
                        column: x => x.KitchenID,
                        principalSchema: "app",
                        principalTable: "Kitchen",
                        principalColumn: "KitchenID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IngredientTag",
                schema: "app",
                columns: table => new
                {
                    IngredientTagID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IngredientID = table.Column<long>(nullable: false),
                    KitchenID = table.Column<long>(nullable: false),
                    CreatedByKitchenUserID = table.Column<long>(nullable: true),
                    TagName = table.Column<string>(unicode: false, maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IngredientTag", x => x.IngredientTagID);
                    table.ForeignKey(
                        name: "UserToTagFK",
                        column: x => x.CreatedByKitchenUserID,
                        principalSchema: "app",
                        principalTable: "KitchenUser",
                        principalColumn: "KitchenUserID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "IngredientToTagFK",
                        column: x => x.IngredientID,
                        principalSchema: "app",
                        principalTable: "Ingredient",
                        principalColumn: "IngredientID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "KitchenToTagFK",
                        column: x => x.KitchenID,
                        principalSchema: "app",
                        principalTable: "Kitchen",
                        principalColumn: "KitchenID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KitchenIngredient",
                schema: "app",
                columns: table => new
                {
                    KitchenIngredientID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IngredientID = table.Column<long>(nullable: false),
                    KitchenID = table.Column<long>(nullable: false),
                    AddedByKitchenUserID = table.Column<long>(nullable: true),
                    CategoryID = table.Column<long>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: false, defaultValueSql: "(getutcdate())"),
                    Quantity = table.Column<int>(nullable: true),
                    Note = table.Column<string>(maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KitchenIngredient", x => new { x.KitchenIngredientID, x.IngredientID, x.KitchenID });
                    table.ForeignKey(
                        name: "KitchenUserToIngredientFK",
                        column: x => x.AddedByKitchenUserID,
                        principalSchema: "app",
                        principalTable: "KitchenUser",
                        principalColumn: "KitchenUserID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "CategoryToKitchenIngredientFK",
                        column: x => x.CategoryID,
                        principalSchema: "app",
                        principalTable: "Category",
                        principalColumn: "CategoryID",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "IngredientToKitchenIngredientFK",
                        column: x => x.IngredientID,
                        principalSchema: "app",
                        principalTable: "Ingredient",
                        principalColumn: "IngredientID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "KitchenToKitchenIngredientFK",
                        column: x => x.KitchenID,
                        principalSchema: "app",
                        principalTable: "Kitchen",
                        principalColumn: "KitchenID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RecipeIngredient",
                schema: "app",
                columns: table => new
                {
                    RecipeIngredientID = table.Column<int>(nullable: false),
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
                name: "KitchenListIngredient",
                schema: "app",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false),
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

            migrationBuilder.CreateTable(
                name: "MealPlanRecipe",
                schema: "app",
                columns: table => new
                {
                    MealPlanRecipeID = table.Column<int>(nullable: false),
                    RecipeID = table.Column<long>(nullable: false),
                    MealPlanID = table.Column<long>(nullable: false),
                    SortOrder = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MealPlanRecipe", x => new { x.MealPlanRecipeID, x.RecipeID, x.MealPlanID });
                    table.ForeignKey(
                        name: "MealPlanToRecipeFK",
                        column: x => x.MealPlanID,
                        principalSchema: "app",
                        principalTable: "MealPlan",
                        principalColumn: "MealPlanID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "RecipeToMealPlanFK",
                        column: x => x.RecipeID,
                        principalSchema: "app",
                        principalTable: "Recipe",
                        principalColumn: "RecipeID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "fkIdx_161",
                schema: "app",
                table: "Category",
                column: "CategoryTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Category_CreatedByKitchenId",
                schema: "app",
                table: "Category",
                column: "CreatedByKitchenId");

            migrationBuilder.CreateIndex(
                name: "fkIdx_40",
                schema: "app",
                table: "Ingredient",
                column: "AddedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Ingredient_CategoryId",
                schema: "app",
                table: "Ingredient",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "fkIdx_204",
                schema: "app",
                table: "IngredientTag",
                column: "CreatedByKitchenUserID");

            migrationBuilder.CreateIndex(
                name: "fkIdx_198",
                schema: "app",
                table: "IngredientTag",
                column: "IngredientID");

            migrationBuilder.CreateIndex(
                name: "fkIdx_201",
                schema: "app",
                table: "IngredientTag",
                column: "KitchenID");

            migrationBuilder.CreateIndex(
                name: "fkIdx_115",
                schema: "app",
                table: "KitchenIngredient",
                column: "AddedByKitchenUserID");

            migrationBuilder.CreateIndex(
                name: "fkIdx_187",
                schema: "app",
                table: "KitchenIngredient",
                column: "CategoryID");

            migrationBuilder.CreateIndex(
                name: "fkIdx_50",
                schema: "app",
                table: "KitchenIngredient",
                column: "IngredientID");

            migrationBuilder.CreateIndex(
                name: "fkIdx_47",
                schema: "app",
                table: "KitchenIngredient",
                column: "KitchenID");

            migrationBuilder.CreateIndex(
                name: "fkIdx_175",
                schema: "app",
                table: "KitchenList",
                column: "CategoryID");

            migrationBuilder.CreateIndex(
                name: "fkIdx_145",
                schema: "app",
                table: "KitchenList",
                column: "KitchenID");

            migrationBuilder.CreateIndex(
                name: "IX_KitchenListIngredient_AddedFromRecipeId",
                schema: "app",
                table: "KitchenListIngredient",
                column: "AddedFromRecipeId");

            migrationBuilder.CreateIndex(
                name: "fkIdx_172",
                schema: "app",
                table: "KitchenListIngredient",
                column: "IngredientID");

            migrationBuilder.CreateIndex(
                name: "fkIdx_178",
                schema: "app",
                table: "KitchenListIngredient",
                column: "KitchenListID");

            migrationBuilder.CreateIndex(
                name: "fkIdx_190",
                schema: "app",
                table: "KitchenRecipe",
                column: "CategoryID");

            migrationBuilder.CreateIndex(
                name: "fkIdx_97",
                schema: "app",
                table: "KitchenRecipe",
                column: "KitchenID");

            migrationBuilder.CreateIndex(
                name: "fkIdx_100",
                schema: "app",
                table: "KitchenRecipe",
                column: "RecipeID");

            migrationBuilder.CreateIndex(
                name: "fkIdx_22",
                schema: "app",
                table: "KitchenUser",
                column: "KitchenID");

            migrationBuilder.CreateIndex(
                name: "fkIdx_19",
                schema: "app",
                table: "KitchenUser",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "fkIdx_181",
                schema: "app",
                table: "MealPlan",
                column: "CategoryID");

            migrationBuilder.CreateIndex(
                name: "fkIdx_123",
                schema: "app",
                table: "MealPlan",
                column: "CreatedByKitchenUserID");

            migrationBuilder.CreateIndex(
                name: "fkIdx_108",
                schema: "app",
                table: "MealPlan",
                column: "KitchenID");

            migrationBuilder.CreateIndex(
                name: "fkIdx_136",
                schema: "app",
                table: "MealPlanRecipe",
                column: "MealPlanID");

            migrationBuilder.CreateIndex(
                name: "fkIdx_133",
                schema: "app",
                table: "MealPlanRecipe",
                column: "RecipeID");

            migrationBuilder.CreateIndex(
                name: "fkIdx_69",
                schema: "app",
                table: "Recipe",
                column: "CreatedByUserID");

            migrationBuilder.CreateIndex(
                name: "fkIdx_76",
                schema: "app",
                table: "RecipeIngredient",
                column: "IngredientID");

            migrationBuilder.CreateIndex(
                name: "fkIdx_73",
                schema: "app",
                table: "RecipeIngredient",
                column: "RecipeID");

            migrationBuilder.CreateIndex(
                name: "fkIdx_87",
                schema: "app",
                table: "RecipeStep",
                column: "RecipeID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IngredientTag",
                schema: "app");

            migrationBuilder.DropTable(
                name: "KitchenIngredient",
                schema: "app");

            migrationBuilder.DropTable(
                name: "KitchenListIngredient",
                schema: "app");

            migrationBuilder.DropTable(
                name: "KitchenRecipe",
                schema: "app");

            migrationBuilder.DropTable(
                name: "MealPlanRecipe",
                schema: "app");

            migrationBuilder.DropTable(
                name: "RecipeIngredient",
                schema: "app");

            migrationBuilder.DropTable(
                name: "RecipeStep",
                schema: "app");

            migrationBuilder.DropTable(
                name: "KitchenList",
                schema: "app");

            migrationBuilder.DropTable(
                name: "MealPlan",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Ingredient",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Recipe",
                schema: "app");

            migrationBuilder.DropTable(
                name: "KitchenUser",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Category",
                schema: "app");

            migrationBuilder.DropTable(
                name: "CategoryType",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Kitchen",
                schema: "app");
        }
    }
}
