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
    public class RecipeService : IPantryService
    {
        public PantryPlannerContext Context { get; set; }

        private PermissionService Permissions { get; set; }

        public RecipeService(PantryPlannerContext context)
        {
            Context = context;
            Permissions = new PermissionService(Context);
        }


        #region Get Methods

        /// <summary>
        /// Return Recipe for <paramref name="recipeId"/>
        /// </summary>
        public Recipe GetRecipeById(long recipeId, PantryPlannerUser user)
        {
            if (Context.RecipeExists(recipeId) == false)
            {
                throw new RecipeNotFoundException(recipeId);
            }

            Recipe recipe = GetRecipeById(recipeId);

            if (!recipe.IsPublic.Value && Permissions.UserAddedRecipe(recipe, user) == false)
            {
                throw new PermissionsException("You do not have rights to this recipe");
            }

            return recipe;
        }

        /// <summary>
        /// Return Recipe for <paramref name="recipeId"/>
        /// </summary>
        public Recipe GetRecipeById(long recipeId)
        {
            return Context.Recipe.Where(r => r.RecipeId == recipeId)
                                            .Include(i => i.RecipeIngredient)
                                            .Include(i => i.RecipeStep)
                                            .Include(i => i.CreatedByUser)
                                            .FirstOrDefault();
        }

        /// <summary>
        /// Return list of Recipes with names that match the given <paramref name="name"/> passed in.
        /// </summary>
        /// <param name="name"> name to search for </param>
        public List<Recipe> GetRecipeByName(string name)
        {
            if (String.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            // first check for exact match
            if (Context.Recipe.Any(r => r.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && r.IsPublic.Value))
            {
                return Context.Recipe.Where(r => r.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && r.IsPublic.Value).ToList();
            }


            // second check for any matches that have all the words entered
            List<string> wordsToSearchFor = name.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToList();

            List<Recipe> ingredients = Context.Recipe.Where(r => wordsToSearchFor.All(w => r.Name.Contains(w, StringComparison.OrdinalIgnoreCase)) && r.IsPublic.Value)
                            .Include(i => i.RecipeIngredient)
                            .Include(i => i.RecipeStep)
                            .Include(i => i.CreatedByUser)
                            .ToList();


            if (ingredients.Count == 0)
            {
                // if no matches then lastly check if any word entered matches
                ingredients = Context.Recipe.Where(r => wordsToSearchFor.Any(w => r.Name.Contains(w, StringComparison.OrdinalIgnoreCase)) && r.IsPublic.Value)
                            .Include(i => i.RecipeIngredient)
                            .Include(i => i.RecipeStep)
                            .Include(i => i.CreatedByUser)
                            .ToList();
            }

            return ingredients;
        }

        #endregion


        #region Add Methods


        /// <summary>
        /// Adds a Recipe to the <see cref="Context"/> that was added by the <paramref name="user"/>.
        /// </summary>
        /// <param name="newRecipe"> recipe to add </param>
        /// <param name="user"> user who is adding recipe </param>
        public RecipeDto AddRecipe(RecipeDto newRecipe, PantryPlannerUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (newRecipe == null)
            {
                throw new ArgumentNullException(nameof(newRecipe));
            }

            if (Context.UserExists(user.Id) == false)
            {
                throw new UserNotFoundException(user.UserName);
            }

            Recipe recipeToAdd = newRecipe.Create();

            return AddRecipe(recipeToAdd, user);
        }

        /// <summary>
        /// Adds a Recipe to the <see cref="Context"/> that was added by the <paramref name="user"/>.
        /// </summary>
        /// <param name="newRecipe"> recipe to add </param>
        /// <param name="user"> user who is adding recipe </param>
        public RecipeDto AddRecipe(Recipe newRecipe, PantryPlannerUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (newRecipe == null)
            {
                throw new ArgumentNullException(nameof(newRecipe));
            }

            if (Context.UserExists(user.Id) == false)
            {
                throw new UserNotFoundException(user.UserName);
            }

            // validate name passed in
            if (String.IsNullOrWhiteSpace(newRecipe.Name))
            {
                throw new InvalidOperationException("Recipe name is required");
            }

            newRecipe.CreatedByUserId = user.Id;
            newRecipe.DateCreated = DateTime.Now;

            Context.Recipe.Add(newRecipe);
            Context.SaveChanges();

            return new RecipeDto(newRecipe);
        }

        #endregion


        #region Update Methods

        /// <summary>
        /// Updates recipe if user has rights to it (i.e. added the recipe)
        /// </summary>
        public async Task<bool> UpdateRecipeAsync(RecipeDto recipeDto, PantryPlannerUser userUpdating)
        {
            if (recipeDto == null)
            {
                throw new ArgumentNullException(nameof(recipeDto));
            }

            if (userUpdating == null)
            {
                throw new ArgumentNullException(nameof(userUpdating));
            }

            if (Context.RecipeExists(recipeDto.RecipeId) == false)
            {
                throw new RecipeNotFoundException(recipeDto.RecipeId);
            }

            if (Permissions.UserAddedRecipe(recipeDto.RecipeId, userUpdating) == false)
            {
                throw new PermissionsException("You do not have rights to update this recipe");
            }

            Recipe recipeToUpdate = Context.Recipe
                                           .Where(r => r.RecipeId == recipeDto.RecipeId)
                                           .FirstOrDefault();

            if (recipeDto.CookTime != null)
            {
                recipeToUpdate.CookTime = recipeDto.CookTime;
            }

            if (recipeDto.Description != null)
            {
                recipeToUpdate.Description = recipeDto.Description;
            }

            if (recipeDto.Name != null)
            {
                recipeToUpdate.Name = recipeDto.Name;
            }

            if (recipeDto.PrepTime != null)
            {
                recipeToUpdate.PrepTime = recipeDto.PrepTime;
            }

            if (recipeDto.ServingSize != null)
            {
                recipeToUpdate.ServingSize = recipeDto.ServingSize;
            }

            if (recipeDto.RecipeUrl != null)
            {
                recipeToUpdate.RecipeUrl = recipeDto.RecipeUrl;
            }

            if (recipeDto.IsPublic != null)
            {
                recipeToUpdate.IsPublic = recipeDto.IsPublic;
            }

            Context.Entry(recipeToUpdate).State = EntityState.Modified;
            await Context.SaveChangesAsync().ConfigureAwait(false);

            return true;
        }

        /// <summary>
        /// Updates recipe if user has rights to it (i.e. added the recipe)
        /// </summary>
        public async Task<bool> UpdateRecipeAsync(Recipe recipe, PantryPlannerUser userUpdating)
        {
            if (recipe == null)
            {
                throw new ArgumentNullException(nameof(recipe));
            }

            if (userUpdating == null)
            {
                throw new ArgumentNullException(nameof(userUpdating));
            }

            if (Context.RecipeExists(recipe) == false)
            {
                throw new RecipeNotFoundException(recipe.RecipeId);
            }

            if (Permissions.UserAddedRecipe(recipe, userUpdating) == false)
            {
                throw new PermissionsException("You do not have rights to update this recipe");
            }

            Context.Entry(recipe).State = EntityState.Modified;
            await Context.SaveChangesAsync().ConfigureAwait(false);

            return true;
        }

        #endregion


        #region Delete Methods

        /// <summary>
        /// Deletes recipe if user has rights to it (i.e. added the recipe)
        /// </summary>
        public Recipe DeleteRecipe(Recipe recipe, PantryPlannerUser userDeleting)
        {
            if (recipe == null)
            {
                throw new ArgumentNullException(nameof(recipe));
            }

            return DeleteRecipe(recipe.RecipeId, userDeleting);
        }

        /// <summary>
        /// Deletes recipe if user has rights to it (i.e. added the recipe)
        /// </summary>
        public Recipe DeleteRecipe(long recipeId, PantryPlannerUser userDeleting)
        {
            if (userDeleting == null)
            {
                throw new ArgumentNullException(nameof(userDeleting));
            }

            if (Context.RecipeExists(recipeId) == false)
            {
                throw new RecipeNotFoundException(recipeId);
            }

            if (Permissions.UserAddedRecipe(recipeId, userDeleting) == false)
            {
                throw new PermissionsException($"You do not have rights to delete this recipe");
            }

            Recipe recipeToDelete = Context.Recipe.Find(recipeId);

            Context.Recipe.Remove(recipeToDelete);
            Context.SaveChanges();

            return recipeToDelete;
        }

        #endregion

    }
}
