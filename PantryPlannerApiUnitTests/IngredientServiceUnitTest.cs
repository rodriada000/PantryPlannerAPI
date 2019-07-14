using PantryPlanner.Exceptions;
using PantryPlanner.Migrations;
using PantryPlanner.Models;
using PantryPlanner.Services;
using PantryPlannerApiUnitTests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PantryPlannerApiUnitTests
{
    public class IngredientServiceUnitTest
    {
        PantryPlannerContext _context;
        IngredientService _ingredientService;
        PantryPlannerUser _testUser;

        public IngredientServiceUnitTest()
        {
            _testUser = InMemoryDataGenerator.TestUser;
            _context = InMemoryDataGenerator.CreateAndInitializeInMemoryDatabaseContext(Guid.NewGuid().ToString(), _testUser);

            // load ingredient data into in-memory database
            USDAFoodCompositionDbETL etl = new USDAFoodCompositionDbETL(FoodCompositionETLUnitTest.FoodCompositionFolderLocation);
            etl.StartEtlProcess(_context);

            _ingredientService = new IngredientService(_context);
        }


        #region Get Test Methods

        [Fact]
        public void GetIngredientByName_Butter_ReturnsCorrectCount()
        {
            int expectedCount = _context.Ingredient.Where(i => i.Name.Contains("butter", StringComparison.OrdinalIgnoreCase)).Count();

            List<Ingredient> ingredients = _ingredientService.GetIngredientByName("butter");

            Assert.Equal(expectedCount, ingredients.Count);
        }

        [Fact]
        public void GetIngredientByNameAndCategory_ButterAndDairy_ReturnsCorrectCount()
        {
            Category dairyCat = _context.Category.Where(c => c.Name == "Dairy and Egg Products").FirstOrDefault();

            int expectedCount = _context.Ingredient.Where(i => i.Name.Contains("butter", StringComparison.OrdinalIgnoreCase) && i.CategoryId == dairyCat.CategoryId).Count();

            List<Ingredient> ingredients = _ingredientService.GetIngredientByNameAndCategory("butter", "Dairy and Egg Products");

            Assert.Equal(expectedCount, ingredients.Count);
        }

        #endregion


        #region Delete Test Methods

        [Fact]
        public void DeleteIngredient_ValidIngredientAndUser_ReturnsIngredientDeleted()
        {
            Category dairyCat = _context.Category.Where(c => c.Name == "Dairy and Egg Products").FirstOrDefault();

            Ingredient ingredient = new Ingredient()
            {
                Name = "Buttery butter",
                Description = "better than butter",
                CategoryId = dairyCat.CategoryId,
                IsPublic = false
            };

            _ingredientService.AddIngredient(ingredient, _testUser);

            Ingredient deletedIngredient = _ingredientService.DeleteIngredient(ingredient, _testUser);

            Assert.Equal(ingredient, deletedIngredient);
            Assert.Equal(0, _testUser.Ingredient.Count); //assert no ingredients added by user
        }

        [Fact]
        public void DeleteIngredient_UserWithNoRights_ThrowsPermissionsExceptions()
        {
            Category dairyCat = _context.Category.Where(c => c.Name == "Dairy and Egg Products").FirstOrDefault();

            Ingredient ingredient = new Ingredient()
            {
                Name = "Buttery butter",
                Description = "better than butter",
                CategoryId = dairyCat.CategoryId,
                IsPublic = true
            };

            _ingredientService.AddIngredient(ingredient);

            Assert.Throws<PermissionsException>(() =>
            {
                _ingredientService.DeleteIngredient(ingredient, _testUser);
            });
        }

        [Fact]
        public void DeleteIngredient_InvalidIngredient_ThrowsIngredientNotFoundException()
        {
            Assert.Throws<IngredientNotFoundException>(() =>
            {
                _ingredientService.DeleteIngredient(new Ingredient() { IngredientId = -5 }, _testUser);
            });
        }

        #endregion


        #region Add Test Methods

        [Fact]
        public void AddIngredient_AlreadyExistsPublicly_ThrowsInvalidOperationException()
        {
            Category dairyCat = _context.Category.Where(c => c.Name == "Dairy and Egg Products").FirstOrDefault();

            Ingredient ingredient = new Ingredient()
            {
                Name = "Butter, without salt",
                CategoryId = dairyCat.CategoryId,
                IsPublic = true
            };

            Assert.Throws<InvalidOperationException>(() =>
            {
                _ingredientService.AddIngredient(ingredient);
            });

            Assert.Throws<InvalidOperationException>(() =>
            {
                _ingredientService.AddIngredient(ingredient, _testUser);
            });
        }

        [Fact]
        public void AddIngredient_ValidIngredient_IngredientIsAdded()
        {
            Category dairyCat = _context.Category.Where(c => c.Name == "Dairy and Egg Products").FirstOrDefault();

            Ingredient ingredient = new Ingredient()
            {
                Name = "Buttery butter",
                Description = "better than butter",
                CategoryId = dairyCat.CategoryId,
                IsPublic = false
            };

            _ingredientService.AddIngredient(ingredient, _testUser);

            Assert.Equal(ingredient.AddedByUserId, _testUser.Id);
            Assert.True(ingredient.IngredientId > 0);
            Assert.True(_context.Ingredient.Any(i => i.Name == ingredient.Name && i.Description == ingredient.Description && i.CategoryId == ingredient.CategoryId));
        }

        [Fact]
        public void AddIngredient_TryDuplicateAdd_ThrowsInvalidOperationException()
        {
            Category dairyCat = _context.Category.Where(c => c.Name == "Dairy and Egg Products").FirstOrDefault();

            Ingredient ingredient = new Ingredient()
            {
                Name = "Buttery butter",
                Description = "better than butter",
                CategoryId = dairyCat.CategoryId,
                IsPublic = false
            };

            _ingredientService.AddIngredient(ingredient, _testUser);


            Assert.Throws<InvalidOperationException>(() =>
            {
                _ingredientService.AddIngredient(ingredient, _testUser);
            });
        }

        [Fact]
        public void AddIngredient_ValidIngredientWithNoUser_IngredientIsAdded()
        {
            Category dairyCat = _context.Category.Where(c => c.Name == "Dairy and Egg Products").FirstOrDefault();

            Ingredient ingredient = new Ingredient()
            {
                Name = "Buttery butter",
                Description = "better than butter",
                CategoryId = dairyCat.CategoryId
            };

            _ingredientService.AddIngredient(ingredient);

            Assert.Null(ingredient.AddedByUserId);
            Assert.True(ingredient.IngredientId > 0);
            Assert.True(_context.Ingredient.Any(i => i.Name == ingredient.Name && i.Description == ingredient.Description && i.CategoryId == ingredient.CategoryId));
        }

        #endregion


        #region Update Test Methods



        #endregion

    }
}
