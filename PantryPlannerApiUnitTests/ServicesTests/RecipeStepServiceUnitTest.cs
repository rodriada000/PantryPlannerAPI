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
    public class RecipeStepServiceUnitTest
    {
        PantryPlannerContext _context;
        RecipeStepService _service;
        PantryPlannerUser _testUser;

        public RecipeStepServiceUnitTest()
        {
            _testUser = InMemoryDataGenerator.TestUser;
            _context = InMemoryDataGenerator.CreateAndInitializeInMemoryDatabaseContext(Guid.NewGuid().ToString(), _testUser, insertIngredientData: true);

            InMemoryDataGenerator.AddNewRandomRecipeWithNoIngredientsOrSteps(_context, _testUser, isPublic: true);

            _service = new RecipeStepService(_context);
        }


        #region Get Test Methods


        [Fact]
        public void GetStepsForRecipe_ValidRecipe_CorrectListOfRecipeIngredientsReturned()
        {
            // setup recipe with a few steps
            Recipe recipe = InMemoryDataGenerator.AddNewRandomRecipeWithNoIngredientsOrSteps(_context, _testUser);
            int expectedCount = 3;

            for (int i = 0; i < expectedCount; i++)
            {
                InMemoryDataGenerator.AddStepToRecipe(_context, recipe, $"step number {i+1}");
            }

            List<RecipeStep> steps = _service.GetStepsForRecipe(recipe.RecipeId, _testUser);

            Assert.Equal(expectedCount, steps.Count);
        }



        #endregion


        #region Delete Test Methods


        #endregion


        #region Add Test Methods

  
        #endregion


        #region Update Test Methods


        #endregion

    }
}
