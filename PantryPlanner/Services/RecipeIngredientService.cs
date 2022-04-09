using Microsoft.EntityFrameworkCore;
using PantryPlanner.DTOs;
using PantryPlanner.Exceptions;
using PantryPlanner.Extensions;
using PantryPlanner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PantryPlanner.Services
{
    public class RecipeIngredientService : IPantryService
    {
        public PantryPlannerContext Context { get; set; }

        private PermissionService Permissions { get; set; }

        public RecipeIngredientService(PantryPlannerContext context)
        {
            Context = context;
            Permissions = new PermissionService(Context);
        }


        #region Get Methods

        /// <summary>
        /// Return ingredient in recipe for <paramref name="recipeIngredientId"/>
        /// </summary>
        public RecipeIngredient GetRecipeIngredientById(long recipeIngredientId, PantryPlannerUser user)
        {
            if (Context.RecipeIngredientExists(recipeIngredientId) == false)
            {
                throw new RecipeIngredientNotFoundException(recipeIngredientId);
            }

            RecipeIngredient recipeIngredient = GetRecipeIngredientById(recipeIngredientId);

            if (Permissions.UserHasViewRightsToRecipeIngredient(recipeIngredient, user) == false)
            {
                throw new PermissionsException("You do not have rights to this recipe");
            }

            return recipeIngredient;
        }

        /// <summary>
        /// Return ingredient in recipe for <paramref name="recipeIngredientId"/>
        /// </summary>
        public RecipeIngredient GetRecipeIngredientById(long recipeIngredientId)
        {
            return Context.RecipeIngredient.Where(r => r.RecipeIngredientId == recipeIngredientId)
                                            .Include(i => i.Recipe)
                                            .Include(i => i.Ingredient)
                                            .FirstOrDefault();
        }

        public List<RecipeIngredient> GetIngredientsForRecipe(long recipeId, PantryPlannerUser user)
        {
            if (Context.RecipeExists(recipeId) == false)
            {
                throw new RecipeNotFoundException(recipeId);
            }

            if (Permissions.UserHasViewRightsToRecipe(recipeId, user) == false)
            {
                throw new PermissionsException("You do not have rights to this recipe");
            }

            return Context.RecipeIngredient.Where(r => r.RecipeId == recipeId)
                                           .Include(r => r.Ingredient)
                                           .ToList();
        }

        #endregion


        #region Add Methods


        /// <summary>
        /// Adds an ingredient to a Recipe.
        /// </summary>
        /// <param name="newRecipeIngredient"> RecipeIngredient to add </param>
        /// <param name="user"> user who is adding ingredient to recipe </param>
        public RecipeIngredient AddRecipeIngredient(RecipeIngredientDto newRecipeIngredient, PantryPlannerUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (newRecipeIngredient == null)
            {
                throw new ArgumentNullException(nameof(newRecipeIngredient));
            }

            if (Context.UserExists(user.Id) == false)
            {
                throw new UserNotFoundException(user.UserName);
            }

            RecipeIngredient recipeToAdd = newRecipeIngredient.Create();

            return AddRecipeIngredient(recipeToAdd, user);
        }

        /// <summary>
        /// Adds an ingredient to a Recipe.
        /// </summary>
        /// <param name="newRecipeIngredient"> RecipeIngredient to add </param>
        /// <param name="user"> user who is adding ingredient to recipe </param>
        public RecipeIngredient AddRecipeIngredient(RecipeIngredient newRecipeIngredient, PantryPlannerUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (newRecipeIngredient == null)
            {
                throw new ArgumentNullException(nameof(newRecipeIngredient));
            }

            if (Context.UserExists(user.Id) == false)
            {
                throw new UserNotFoundException(user.UserName);
            }

            // validate RecipeID and IngredientID exists
            if (Context.RecipeExists(newRecipeIngredient.RecipeId) == false)
            {
                throw new RecipeNotFoundException(newRecipeIngredient.RecipeId);
            }

            if (Context.IngredientExists(newRecipeIngredient.IngredientId) == false)
            {
                throw new IngredientNotFoundException(newRecipeIngredient.IngredientId);
            }

            if (Permissions.UserAddedRecipe(newRecipeIngredient.RecipeId, user) == false)
            {
                throw new PermissionsException("User does not have rights to add to recipe");
            }

            // validate the ingredient is not already added to recipe
            if (Context.RecipeIngredientExists(newRecipeIngredient))
            {
                throw new InvalidOperationException($"Ingredient already exists in recipe.");
            }

            // validate qty >= 0
            if (newRecipeIngredient.Quantity < 0)
            {
                throw new InvalidOperationException($"Quantity must be greater than or equal to zero.");
            }

            if (newRecipeIngredient.SortOrder <= 0)
            {
                newRecipeIngredient.SortOrder = GetNextSortOrderForRecipe(newRecipeIngredient.RecipeId);
            }

            Context.RecipeIngredient.Add(newRecipeIngredient);
            Context.SaveChanges();

            // ensure Ingredient information is returned with new dto
            newRecipeIngredient.Ingredient = Context.Ingredient.Where(r => r.IngredientId == newRecipeIngredient.IngredientId).FirstOrDefault();
            return newRecipeIngredient;
        }


        /// <summary>
        /// Adds <paramref name="ingredientToAdd"/> to <paramref name="recipe"/>
        /// </summary>
        /// <param name="ingredientToAdd"></param>
        /// <param name="recipe"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public RecipeIngredient AddIngredientToRecipe(Ingredient ingredientToAdd, Recipe recipe, PantryPlannerUser user)
        {
            if (ingredientToAdd == null)
            {
                throw new ArgumentNullException(nameof(ingredientToAdd));
            }

            if (recipe == null)
            {
                throw new ArgumentNullException(nameof(recipe));
            }

            return AddIngredientToRecipe(ingredientToAdd.IngredientId, recipe.RecipeId, user);
        }

        public RecipeIngredient AddIngredientToRecipe(long ingredientId, long recipeId, PantryPlannerUser user)
        {
            RecipeIngredient ingredientToAdd = new RecipeIngredient()
            {
                RecipeId = recipeId,
                IngredientId = ingredientId,
                Quantity = 1
            };

            return AddRecipeIngredient(ingredientToAdd, user);
        }

        private int GetNextSortOrderForRecipe(long recipeId)
        {
            if (Context.RecipeExists(recipeId) == false)
            {
                throw new RecipeNotFoundException(recipeId);
            }

            if (Context.RecipeIngredient.Any(r => r.RecipeId == recipeId) == false)
            {
                return 1;
            }

            int maxSortOrder = Context.RecipeIngredient.Where(r => r.RecipeId == recipeId)
                                                       .ToList() // brings the list of recipe ingredients to the client side (api) to finish calculation instead of doing Max() on the Database
                                                       .Max(r => r.SortOrder);

            return maxSortOrder + 1;
        }

        #endregion


        #region Update Methods

        /// <summary>
        /// Updates ingredient in recipe if user has rights to it (i.e. added the recipe)
        /// </summary>
        public async Task UpdateRecipeIngredientAsync(RecipeIngredientDto updateDto, PantryPlannerUser userUpdating)
        {
            if (updateDto == null)
            {
                throw new ArgumentNullException(nameof(updateDto));
            }

            if (userUpdating == null)
            {
                throw new ArgumentNullException(nameof(userUpdating));
            }

            if (Context.RecipeIngredientExists(updateDto.RecipeIngredientId) == false)
            {
                throw new RecipeIngredientNotFoundException(updateDto.RecipeIngredientId);
            }

            if (Permissions.UserAddedRecipe(updateDto.RecipeId, userUpdating) == false)
            {
                throw new PermissionsException("You do not have rights to update this recipe");
            }

            RecipeIngredient ingredientToUpdate = Context.RecipeIngredient
                                                         .Where(r => r.RecipeIngredientId == updateDto.RecipeIngredientId)
                                                         .FirstOrDefault();

            // only update the properties that are not null in the DTO
            if (updateDto.Quantity.HasValue)
            {
                ingredientToUpdate.Quantity = updateDto.Quantity.Value;
            }

            if (updateDto.UnitOfMeasure != null)
            {
                ingredientToUpdate.UnitOfMeasure = updateDto.UnitOfMeasure;
            }

            if (updateDto.Method != null)
            {
                ingredientToUpdate.Method = updateDto.Method;
            }

            if (updateDto.SortOrder.HasValue)
            {
                ingredientToUpdate.SortOrder = updateDto.SortOrder.Value;
            }

            // validate qty >= 0
            if (ingredientToUpdate.Quantity < 0)
            {
                throw new InvalidOperationException($"Quantity must be greater than or equal to zero");
            }

            Context.Entry(ingredientToUpdate).State = EntityState.Modified;
            await Context.SaveChangesAsync().ConfigureAwait(false);

            return;
        }

        /// <summary>
        /// Updates ingredient in recipe if user has rights to it (i.e. added the recipe)
        /// </summary>
        public async Task UpdateRecipeIngredientAsync(RecipeIngredient recipeIngredient, PantryPlannerUser userUpdating)
        {
            if (recipeIngredient == null)
            {
                throw new ArgumentNullException(nameof(recipeIngredient));
            }

            if (userUpdating == null)
            {
                throw new ArgumentNullException(nameof(userUpdating));
            }

            if (Context.RecipeIngredientExists(recipeIngredient) == false)
            {
                throw new RecipeIngredientNotFoundException(recipeIngredient.RecipeIngredientId);
            }

            if (Permissions.UserAddedRecipe(recipeIngredient.RecipeId, userUpdating) == false)
            {
                throw new PermissionsException("You do not have rights to update this recipe");
            }

            // validate qty >= 0
            if (recipeIngredient.Quantity < 0)
            {
                throw new InvalidOperationException($"Quantity must be greater than or equal to zero");
            }

            Context.Entry(recipeIngredient).State = EntityState.Modified;
            await Context.SaveChangesAsync().ConfigureAwait(false);

            return;
        }

        #endregion


        #region Delete Methods

        /// <summary>
        /// Deletes ingredient from recipe if user has rights to it (i.e. added the recipe)
        /// </summary>
        public RecipeIngredient DeleteRecipeIngredient(RecipeIngredient ingredient, PantryPlannerUser userDeleting)
        {
            if (ingredient == null)
            {
                throw new ArgumentNullException(nameof(ingredient));
            }

            return DeleteRecipeIngredient(ingredient.RecipeIngredientId, userDeleting);
        }

        /// <summary>
        /// Deletes ingredient from recipe if user has rights to it (i.e. added the recipe)
        /// </summary>
        public RecipeIngredient DeleteRecipeIngredient(long recipeIngredientId, PantryPlannerUser userDeleting)
        {
            if (userDeleting == null)
            {
                throw new ArgumentNullException(nameof(userDeleting));
            }

            if (Context.RecipeIngredientExists(recipeIngredientId) == false)
            {
                throw new RecipeIngredientNotFoundException(recipeIngredientId);
            }

            if (Permissions.UserHasEditRightsToRecipeIngredient(recipeIngredientId, userDeleting) == false)
            {
                throw new PermissionsException($"You do not have rights to modify this recipe");
            }

            RecipeIngredient ingredientToDelete = Context.RecipeIngredient.Where(r => r.RecipeIngredientId == recipeIngredientId).FirstOrDefault();

            Context.RecipeIngredient.Remove(ingredientToDelete);
            Context.SaveChanges();

            return ingredientToDelete;
        }

        #endregion

    }
}
