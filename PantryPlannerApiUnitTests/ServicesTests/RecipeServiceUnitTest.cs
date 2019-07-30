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
    public class RecipeServiceUnitTest
    {
        PantryPlannerContext _context;
        RecipeService _recipeService;
        PantryPlannerUser _testUser;

        public RecipeServiceUnitTest()
        {
            _testUser = InMemoryDataGenerator.TestUser;
            _context = InMemoryDataGenerator.CreateAndInitializeInMemoryDatabaseContext(Guid.NewGuid().ToString(), _testUser, insertIngredientData: true);
            _recipeService = new RecipeService(_context);
        }


        #region Get Test Methods

        [Fact]
        public void GetRecipeById_ValidRecipeId_ReturnsCorrectRecipe()
        {
            Recipe expectedRecipe = _testUser.Recipe.FirstOrDefault();

            Recipe actualRecipe = _recipeService.GetRecipeById(expectedRecipe.RecipeId);

            Xunit.Asserts.Compare.DeepAssert.Equals(expectedRecipe, actualRecipe);
        }


        [Fact]
        public void GetRecipeById_InvalidRecipeId_ThrowsRecipeNotFoundException()
        {
            Assert.Throws<RecipeNotFoundException>(() =>
            {
                _recipeService.GetRecipeById(-5, _testUser);
            });
        }

        [Fact]
        public void GetRecipeById_UserNoRightsToPrivateIngredient_ReturnsCorrectRecipe()
        {
            Recipe privateRecipe = InMemoryDataGenerator.AddNewRandomRecipeWithNoIngredientsOrSteps(_context, _testUser, isPublic: false);
            PantryPlannerUser userWithNoRights = InMemoryDataGenerator.AddNewRandomUser(_context);

            Assert.Throws<PermissionsException>(() =>
            {
                _recipeService.GetRecipeById(privateRecipe.RecipeId, userWithNoRights);
            });
        }

        [Fact]
        public void GetRecipeByName_ExactNameMatch_ReturnsOneRecipeInList()
        {
            Recipe privateRecipe = InMemoryDataGenerator.AddNewRandomRecipeWithNoIngredientsOrSteps(_context, _testUser, isPublic: true);

            List<Recipe> recipes = _recipeService.GetRecipeByName(privateRecipe.Name);

            Assert.Single(recipes, privateRecipe);
        }

        [Fact]
        public void GetRecipeByName_EmptyName_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                _recipeService.GetRecipeByName("");
            });
        }


        #endregion


        #region Delete Test Methods

        [Fact]
        public void DeleteRecipe_ValidRecipeAndUser_ReturnsRecipeDeleted()
        {
            Recipe recipeToDelete = _testUser.Recipe.FirstOrDefault();

            int countBeforeDelete = _testUser.Recipe.Count;

            Recipe deletedRecipe = _recipeService.DeleteRecipe(recipeToDelete, _testUser);

            Assert.Equal(recipeToDelete, deletedRecipe);
            Assert.Equal(countBeforeDelete - 1, _testUser.Recipe.Count);
        }

        [Fact]
        public void DeleteRecipe_ValidRecipeIdAndUser_ReturnsRecipeDeleted()
        {
            Recipe recipeToDelete = _testUser.Recipe.FirstOrDefault();

            int countBeforeDelete = _testUser.Recipe.Count;

            Recipe deletedRecipe = _recipeService.DeleteRecipe(recipeToDelete.RecipeId, _testUser);

            Assert.Equal(recipeToDelete, deletedRecipe);
            Assert.Equal(countBeforeDelete - 1, _testUser.Recipe.Count);
        }

        [Fact]
        public void DeleteRecipe_UserWithNoRights_ThrowsPermissionsExceptions()
        {
            PantryPlannerUser randomUser = InMemoryDataGenerator.AddNewRandomUser(_context);
            Recipe notMyRecipe = InMemoryDataGenerator.AddNewRandomRecipeWithNoIngredientsOrSteps(_context, randomUser);

            Assert.Throws<PermissionsException>(() =>
            {
                _recipeService.DeleteRecipe(notMyRecipe, _testUser);
            });
        }

        [Fact]
        public void DeleteRecipe_RecipeNotExists_ThrowsRecipeNotFoundException()
        {
            Assert.Throws<RecipeNotFoundException>(() =>
            {
                _recipeService.DeleteRecipe(new Recipe() { RecipeId = -5 }, _testUser);
            });
        }

        #endregion


        #region Add Test Methods

        [Fact]
        public void AddRecipe_ValidRecipeDtoAndUser_ReturnsAddedRecipe()
        {
            string expectedName = "Ultimate Recipe Burrito";
            string expectedDescription = "this is actually a pizza recipe...";
            int expectedCookTime = 30;
            RecipeDto recipeToAdd = new RecipeDto()
            {
                Name = expectedName,
                Description = expectedDescription,
                CookTime = expectedCookTime
            };

            RecipeDto actualAddedRecipe = _recipeService.AddRecipe(recipeToAdd, _testUser);

            Assert.True(actualAddedRecipe.RecipeId != 0);
            Assert.Equal(expectedName, actualAddedRecipe.Name);
            Assert.Equal(expectedDescription, actualAddedRecipe.Description);
            Assert.Equal(expectedCookTime, actualAddedRecipe.CookTime);
        }

        [Fact]
        public void AddRecipe_MissingRecipeName_ThrowsInvalidOperationException()
        {
            RecipeDto recipeToAdd = new RecipeDto()
            {
                Description = "pew pew",
            };

            Assert.Throws<InvalidOperationException>(() =>
            {
                RecipeDto actualAddedRecipe = _recipeService.AddRecipe(recipeToAdd, _testUser);
            });
        }

        [Fact]
        public void AddRecipe_UserNotExists_ThrowsUserNotFoundException()
        {
            RecipeDto recipeToAdd = new RecipeDto()
            {
                Name = "some name",
                Description = "soem description",
                CookTime = 20
            };

            Assert.Throws<UserNotFoundException>(() =>
            {
                RecipeDto actualAddedRecipe = _recipeService.AddRecipe(recipeToAdd, new PantryPlannerUser() { Id = "Idontexist" });
            });
        }

        [Fact]
        public void AddRecipe_NullArguments_ThrowsArgumentNullExceptions()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                RecipeDto nullRecipeDto = null;
                _recipeService.AddRecipe(nullRecipeDto, _testUser);
            });

            Assert.Throws<ArgumentNullException>(() =>
            {
                Recipe nullRecipe = null;
                _recipeService.AddRecipe(nullRecipe, _testUser);
            });


            Assert.Throws<ArgumentNullException>(() =>
            {
                RecipeDto recipeToAdd = new RecipeDto()
                {
                    Name = "some name",
                    Description = "soem description",
                    CookTime = 20
                };

                _recipeService.AddRecipe(recipeToAdd, null);
            });
        }

        #endregion


        #region Update Test Methods

        [Fact]
        public async Task UpdateRecipe_ValidRecipeAndUser_ReturnsTrueAndRecipeIsUpdatedAsync()
        {
            Recipe recipeToUpdate = _testUser.Recipe.FirstOrDefault();
            string expectedDescription = "listen everyone if you let your kids play sarcastaball...";
            int expectedCookTime = 30;

            recipeToUpdate.Description = expectedDescription;
            recipeToUpdate.CookTime = expectedCookTime;

            bool result = await _recipeService.UpdateRecipeAsync(recipeToUpdate, _testUser);

            Assert.True(result);

            Recipe updatedRecipe = _context.Recipe.Where(r => r.RecipeId == recipeToUpdate.RecipeId).FirstOrDefault();

            Assert.Equal(expectedDescription, updatedRecipe.Description);
            Assert.Equal(expectedCookTime, updatedRecipe.CookTime);
        }

        [Fact]
        public async Task UpdateRecipe_ValidRecipeDtoAndUser_ReturnsTrueAndRecipeIsUpdatedAsync()
        {
            RecipeDto recipeToUpdate = new RecipeDto(_testUser.Recipe.FirstOrDefault());
            string expectedDescription = "listen everyone if you let your kids play sarcastaball...";
            int expectedCookTime = 30;

            recipeToUpdate.Description = expectedDescription;
            recipeToUpdate.CookTime = expectedCookTime;

            bool result = await _recipeService.UpdateRecipeAsync(recipeToUpdate, _testUser);

            Assert.True(result);

            Recipe updatedRecipe = _context.Recipe.Where(r => r.RecipeId == recipeToUpdate.RecipeId).FirstOrDefault();

            Assert.Equal(expectedDescription, updatedRecipe.Description);
            Assert.Equal(expectedCookTime, updatedRecipe.CookTime);
        }

        [Fact]
        public async Task UpdateRecipe_ValidRecipeDtoWithNullProperties_ReturnsTrueAndOnlyNonNullPropertiesAreUpdated()
        {
            Recipe recipeToUpdate = _testUser.Recipe.FirstOrDefault();
            string expectedDescription = "listen everyone if you let your kids play sarcastaball...";
            int expectedCookTime = 30;

            int? prepTimeBeforeUpdate = recipeToUpdate.PrepTime;
            string servingSizeBeforeUpdate = recipeToUpdate.ServingSize;
            string nameBeforeUpdate = recipeToUpdate.Name;
            bool? isPublicBeforeUpdate = recipeToUpdate.IsPublic;


            RecipeDto recipeDto = new RecipeDto()
            {
                RecipeId = recipeToUpdate.RecipeId,
                CreatedByUserId = _testUser.Id,
                Description = expectedDescription,
                CookTime = expectedCookTime
            };

            bool result = await _recipeService.UpdateRecipeAsync(recipeDto, _testUser);

            Assert.True(result);

            Recipe updatedRecipe = _context.Recipe.Where(r => r.RecipeId == recipeDto.RecipeId).FirstOrDefault();

            Assert.Equal(expectedDescription, updatedRecipe.Description);
            Assert.Equal(expectedCookTime, updatedRecipe.CookTime);
            Assert.Equal(prepTimeBeforeUpdate, updatedRecipe.PrepTime);
            Assert.Equal(servingSizeBeforeUpdate, updatedRecipe.ServingSize);
            Assert.Equal(nameBeforeUpdate, updatedRecipe.Name);
            Assert.Equal(isPublicBeforeUpdate, updatedRecipe.IsPublic);
        }

        [Fact]
        public async Task UpdateRecipe_RecipeNotExists_ThrowsRecipeNotFoundException()
        {
            await Assert.ThrowsAsync<RecipeNotFoundException>(async () =>
             {
                 await _recipeService.UpdateRecipeAsync(new RecipeDto() { RecipeId = -5 }, _testUser);
             });
        }

        [Fact]
        public async Task UpdateRecipe_UserNoRightsToRecipe_ThrowsPermissionsException()
        {
            RecipeDto recipeToUpdate = new RecipeDto(_testUser.Recipe.FirstOrDefault());
            PantryPlannerUser userWithNoRights = InMemoryDataGenerator.AddNewRandomUser(_context);


            recipeToUpdate.Description = "this wont update because I dont have rights";

            await Assert.ThrowsAsync<PermissionsException>(async () =>
            {
                await _recipeService.UpdateRecipeAsync(recipeToUpdate, userWithNoRights);
            });
        }

        #endregion

    }
}
