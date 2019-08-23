using PantryPlanner.DTOs;
using PantryPlanner.Exceptions;
using PantryPlanner.Migrations;
using PantryPlanner.Models;
using PantryPlanner.Services;
using PantryPlannerApiUnitTests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PantryPlannerApiUnitTests
{
    public class RecipeIngredientServiceUnitTest
    {
        PantryPlannerContext _context;
        RecipeIngredientService _service;
        PantryPlannerUser _testUser;

        public RecipeIngredientServiceUnitTest()
        {
            _testUser = InMemoryDataGenerator.TestUser;
            _context = InMemoryDataGenerator.CreateAndInitializeInMemoryDatabaseContext(Guid.NewGuid().ToString(), _testUser, insertIngredientData: true);

            InMemoryDataGenerator.AddNewRandomRecipeWithNoIngredientsOrSteps(_context, _testUser, isPublic: true);

            _service = new RecipeIngredientService(_context);
        }


        #region Get Test Methods


        [Fact]
        public void GetIngredientsForRecipe_ValidRecipe_CorrectListOfRecipeIngredientsReturned()
        {
            // setup recipe with a few ingredients
            Recipe recipe = InMemoryDataGenerator.AddNewRandomRecipeWithNoIngredientsOrSteps(_context, _testUser);
            int expectedCount = 3;

            for (int i = 0; i < expectedCount; i++)
            {
                Ingredient ingredient = InMemoryDataGenerator.AddNewRandomIngredient(_context, _testUser);
                InMemoryDataGenerator.AddIngredientToRecipe(_context, recipe, ingredient);
            }


            List<RecipeIngredient> ingredients = _service.GetIngredientsForRecipe(recipe.RecipeId, _testUser);

            Assert.Equal(expectedCount, ingredients.Count);
        }



        #endregion


        #region Delete Test Methods


        [Fact]
        public void DeleteRecipeIngredient_ValidRecipeIngredient_ReturnsRecipeIngredientDeleted()
        {
            Ingredient ingredient = InMemoryDataGenerator.AddNewRandomIngredient(_context, _testUser);
            Recipe recipe = InMemoryDataGenerator.AddNewRandomRecipeWithNoIngredientsOrSteps(_context, _testUser);

            RecipeIngredient toDelete = InMemoryDataGenerator.AddIngredientToRecipe(_context, recipe, ingredient);

            int countBeforeDelete = recipe.RecipeIngredient.Count;


            RecipeIngredient deletedResult = _service.DeleteRecipeIngredient(toDelete, _testUser);


            Assert.Equal(countBeforeDelete - 1, recipe.RecipeIngredient.Count);
            Assert.Equal(toDelete.RecipeId, deletedResult.RecipeId);
            Assert.Equal(toDelete.IngredientId, deletedResult.IngredientId);
        }

        [Fact]
        public void DeleteRecipeIngredient_NoRightsToRecipe_ThrowsPermissionsException()
        {
            Ingredient ingredient = InMemoryDataGenerator.AddNewRandomIngredient(_context, _testUser);
            Recipe recipe = InMemoryDataGenerator.AddNewRandomRecipeWithNoIngredientsOrSteps(_context, _testUser);

            RecipeIngredient toDelete = InMemoryDataGenerator.AddIngredientToRecipe(_context, recipe, ingredient);

            int countBeforeDelete = recipe.RecipeIngredient.Count;

            var userWithNoRights = InMemoryDataGenerator.AddNewRandomUser(_context);

            Assert.Throws<PermissionsException>(() =>
            {
                _service.DeleteRecipeIngredient(toDelete, userWithNoRights);
            });
        }


        #endregion


        #region Add Test Methods

        [Fact]
        public void AddIngredientToRecipe_NoRightsToRecipe_ThrowsPermissionsException()
        {
            Ingredient ingredientToAdd = _context.Ingredient.Where(i => i.IsPublic).FirstOrDefault();
            Recipe recipe = InMemoryDataGenerator.AddNewRandomRecipeWithNoIngredientsOrSteps(_context
                                                                                            , InMemoryDataGenerator.AddNewRandomUser(_context)
                                                                                            , false);

            if (ingredientToAdd == null || recipe == null)
            {
                throw new Exception("recipe/ingredient not setup for test");
            }

            Assert.Throws<PermissionsException>(() =>
            {
                _service.AddIngredientToRecipe(ingredientToAdd, recipe, _testUser);
            });
        }

        [Fact]
        public void AddIngredientToRecipe_RecipeDoesNotExist_ThrowsRecipeNotFoundException()
        {
            Ingredient ingredientToAdd = _context.Ingredient.Where(i => i.IsPublic).FirstOrDefault();

            if (ingredientToAdd == null)
            {
                throw new Exception("ingredient not setup for test");
            }

            Assert.Throws<RecipeNotFoundException>(() =>
            {
                _service.AddIngredientToRecipe(ingredientToAdd.IngredientId, -5, _testUser);
            });
        }

        [Fact]
        public void AddIngredientToRecipe_IngredientDoesNotExist_ThrowsIngredientNotFoundException()
        {
            Recipe recipe = _testUser.Recipe.FirstOrDefault();

            if (recipe == null)
            {
                throw new Exception("recipe not setup for test");
            }

            Assert.Throws<IngredientNotFoundException>(() =>
            {
                _service.AddIngredientToRecipe(-5, recipe.RecipeId, _testUser);
            });
        }

        [Fact]
        public void AddIngredientToRecipe_UserDoesNotExist_ThrowsUserNotFoundException()
        {
            Ingredient ingredientToAdd = InMemoryDataGenerator.AddNewRandomIngredient(_context);
            Recipe recipe = _testUser.Recipe.FirstOrDefault();

            if (recipe == null || ingredientToAdd == null)
            {
                throw new Exception("ingredient/recipe not setup for test");
            }

            Assert.Throws<UserNotFoundException>(() =>
            {
                _service.AddIngredientToRecipe(ingredientToAdd, recipe, new PantryPlannerUser() { Id = "fake" });
            });
        }

        [Fact]
        public void AddIngredientToRecipe_NullArguments_ThrowsArgumentNullException()
        {
            Ingredient ingredientToAdd = InMemoryDataGenerator.AddNewRandomIngredient(_context);
            Recipe recipe = _testUser.Recipe.FirstOrDefault();

            if (recipe == null || ingredientToAdd == null)
            {
                throw new Exception("ingredient/recipe not setup for test");
            }

            Assert.Throws<ArgumentNullException>(() =>
            {
                _service.AddIngredientToRecipe(null, recipe, _testUser);
            });

            Assert.Throws<ArgumentNullException>(() =>
            {
                _service.AddIngredientToRecipe(ingredientToAdd, null, _testUser);
            });

            Assert.Throws<ArgumentNullException>(() =>
            {
                _service.AddIngredientToRecipe(ingredientToAdd, recipe, null);
            });
        }

        [Fact]
        public void AddIngredientToRecipe_ValidRecipeAndIngredient_ReturnsRecipeIngredientDto()
        {
            Ingredient ingredientToAdd = _context.Ingredient.FirstOrDefault();
            Recipe recipe = _testUser.Recipe.FirstOrDefault();

            if (ingredientToAdd == null || recipe == null)
            {
                throw new Exception("recipe/ingredient not setup for test");
            }

            RecipeIngredient addedResult = _service.AddIngredientToRecipe(ingredientToAdd, recipe, _testUser);

            Assert.Equal(recipe.RecipeId, addedResult.RecipeId);
            Assert.Equal(ingredientToAdd.IngredientId, addedResult.IngredientId);
        }

        [Fact]
        public void AddIngredientToRecipe_ValidRecipeAndIngredient_QuantitySetTo1()
        {
            Ingredient ingredientToAdd = InMemoryDataGenerator.AddNewRandomIngredient(_context, _testUser);
            Recipe recipe = _testUser.Recipe.FirstOrDefault();

            if (ingredientToAdd == null || recipe == null)
            {
                throw new Exception("recipe/ingredient not setup for test");
            }

            RecipeIngredient addedResult = _service.AddIngredientToRecipe(ingredientToAdd, recipe, _testUser);

            Assert.Equal(1, addedResult.Quantity);
        }

        [Fact]
        public void AddIngredientToRecipe_MultipleIngredientsAdded_SortOrderIsSetCorrectly()
        {
            Recipe recipe = _testUser.Recipe.FirstOrDefault();

            if (recipe == null)
            {
                throw new Exception("recipe not setup for test");
            }

            Ingredient ingredientToAdd = InMemoryDataGenerator.AddNewRandomIngredient(_context, _testUser);
            RecipeIngredient firstResult = _service.AddIngredientToRecipe(ingredientToAdd, recipe, _testUser);

            ingredientToAdd = InMemoryDataGenerator.AddNewRandomIngredient(_context, _testUser);
            RecipeIngredient secondResult = _service.AddIngredientToRecipe(ingredientToAdd, recipe, _testUser);

            ingredientToAdd = InMemoryDataGenerator.AddNewRandomIngredient(_context, _testUser);
            RecipeIngredient thirdResult = _service.AddIngredientToRecipe(ingredientToAdd, recipe, _testUser);

            Assert.Equal(1, firstResult.SortOrder);
            Assert.Equal(2, secondResult.SortOrder);
            Assert.Equal(3, thirdResult.SortOrder);
        }

        [Fact]
        public void AddRecipeIngredient_ValidRecipeIngredientDto_FieldsSetCorrectly()
        {
            Ingredient ingredientToAdd = InMemoryDataGenerator.AddNewRandomIngredient(_context, _testUser);
            Recipe recipe = _testUser.Recipe.FirstOrDefault();

            if (ingredientToAdd == null || recipe == null)
            {
                throw new Exception("recipe/ingredient not setup for test");
            }

            string expectedMethod = "chopped";
            string expectedUoM = "cups";
            decimal expectedQty = 1.25m;

            RecipeIngredientDto dtoToAdd = new RecipeIngredientDto()
            {
                RecipeId = recipe.RecipeId,
                IngredientId = ingredientToAdd.IngredientId,
                Quantity = expectedQty,
                UnitOfMeasure = expectedUoM,
                Method = expectedMethod
            };

            RecipeIngredient addedResult = _service.AddRecipeIngredient(dtoToAdd, _testUser);

            Assert.Equal(expectedQty, addedResult.Quantity);
            Assert.Equal(expectedMethod, addedResult.Method);
            Assert.Equal(expectedUoM, addedResult.UnitOfMeasure);

        }


        #endregion


        #region Update Test Methods

        [Fact]
        public async Task UpdateRecipeIngredient_ValidRecipeIngredient_ContextIsUpdatedAsync()
        {
            // setup a new recipe ingredient
            Ingredient ingredient = InMemoryDataGenerator.AddNewRandomIngredient(_context, _testUser);
            Recipe recipe = InMemoryDataGenerator.AddNewRandomRecipeWithNoIngredientsOrSteps(_context, _testUser);
            RecipeIngredient toUpdate = InMemoryDataGenerator.AddIngredientToRecipe(_context, recipe, ingredient);


            // create DTO and update it with expected values
            RecipeIngredientDto dtoToUpdate = new RecipeIngredientDto(toUpdate);

            decimal expectedQty = 1.5m;
            string expectedUoM = "teaspoons";

            dtoToUpdate.Quantity = expectedQty;
            dtoToUpdate.UnitOfMeasure = expectedUoM;

            // perform update and assert changes
            await _service.UpdateRecipeIngredientAsync(dtoToUpdate, _testUser);

            RecipeIngredient updatedItem = _context.RecipeIngredient.Where(r => r.RecipeIngredientId == dtoToUpdate.RecipeIngredientId).FirstOrDefault();

            Assert.Equal(expectedQty, updatedItem.Quantity);
            Assert.Equal(expectedUoM, updatedItem.UnitOfMeasure);
        }

        #endregion

    }
}
