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
    public class RecipeStepService : IPantryService
    {
        public PantryPlannerContext Context { get; set; }

        private PermissionService Permissions { get; set; }

        public RecipeStepService(PantryPlannerContext context)
        {
            Context = context;
            Permissions = new PermissionService(Context);
        }


        #region Get Methods

        /// <summary>
        /// Return step in recipe for <paramref name="recipeStepId"/>
        /// </summary>
        public RecipeStep GetRecipeStepById(long recipeStepId, PantryPlannerUser user)
        {
            if (Context.RecipeStepExists(recipeStepId) == false)
            {
                throw new RecipeStepNotFoundException(recipeStepId);
            }

            RecipeStep recipeStep = GetRecipeStepById(recipeStepId);

            if (Permissions.UserHasViewRightsToRecipeStep(recipeStep, user) == false)
            {
                throw new PermissionsException("You do not have rights to this recipe");
            }

            return recipeStep;
        }

        /// <summary>
        /// Return step in recipe for <paramref name="recipeStepId"/>
        /// </summary>
        public RecipeStep GetRecipeStepById(long recipeStepId)
        {
            return Context.RecipeStep.Where(r => r.RecipeStepId == recipeStepId)
                                            .Include(i => i.Recipe)
                                            .FirstOrDefault();
        }

        public List<RecipeStep> GetStepsForRecipe(long recipeId, PantryPlannerUser user)
        {
            if (Context.RecipeExists(recipeId) == false)
            {
                throw new RecipeNotFoundException(recipeId);
            }

            if (Permissions.UserHasViewRightsToRecipe(recipeId, user) == false)
            {
                throw new PermissionsException("You do not have rights to this recipe");
            }

            return Context.RecipeStep.Where(r => r.RecipeId == recipeId).ToList();
        }

        #endregion


        #region Add Methods


        /// <summary>
        /// Adds a step to a Recipe.
        /// </summary>
        /// <param name="newRecipeStep"> RecipeStep to add </param>
        /// <param name="user"> user who is adding step to recipe </param>
        public RecipeStep AddRecipeStep(RecipeStepDto newRecipeStep, PantryPlannerUser user)
        {
            if (newRecipeStep == null)
            {
                throw new ArgumentNullException(nameof(newRecipeStep));
            }

            RecipeStep recipeToAdd = newRecipeStep.Create();

            return AddRecipeStep(recipeToAdd, user);
        }

        /// <summary>
        /// Adds a step to a Recipe.
        /// </summary>
        /// <param name="newRecipeStep"> RecipeStep to add </param>
        /// <param name="user"> user who is adding recipe to recipe </param>
        public RecipeStep AddRecipeStep(RecipeStep newRecipeStep, PantryPlannerUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (newRecipeStep == null)
            {
                throw new ArgumentNullException(nameof(newRecipeStep));
            }

            if (Context.UserExists(user.Id) == false)
            {
                throw new UserNotFoundException(user.UserName);
            }
         
            if (Context.RecipeExists(newRecipeStep.RecipeId) == false)
            {
                throw new RecipeNotFoundException(newRecipeStep.RecipeId);
            }

            if (Permissions.UserAddedRecipe(newRecipeStep.RecipeId, user) == false)
            {
                throw new PermissionsException("User does not have rights to add to recipe");
            }

            // validate the step is not already added to recipe
            if (Context.RecipeStepExists(newRecipeStep))
            {
                throw new InvalidOperationException($"Step with same ID already exists in recipe.");
            }

            newRecipeStep.SortOrder = GetNextSortOrderForRecipeStep(newRecipeStep.RecipeId);

            ValidateProperties(newRecipeStep);

            Context.RecipeStep.Add(newRecipeStep);
            Context.SaveChanges();

            return newRecipeStep;
        }

        #endregion


        #region Update Methods

        /// <summary>
        /// Updates step in recipe if user has rights to it (i.e. added the recipe)
        /// </summary>
        public async Task UpdateRecipeStepAsync(RecipeStepDto updateDto, PantryPlannerUser userUpdating)
        {
            if (updateDto == null)
            {
                throw new ArgumentNullException(nameof(updateDto));
            }

            if (userUpdating == null)
            {
                throw new ArgumentNullException(nameof(userUpdating));
            }

            if (Context.RecipeStepExists(updateDto.RecipeStepId) == false)
            {
                throw new RecipeStepNotFoundException(updateDto.RecipeStepId);
            }

            if (Permissions.UserAddedRecipe(updateDto.RecipeId, userUpdating) == false)
            {
                throw new PermissionsException("You do not have rights to update this recipe");
            }

            RecipeStep stepToUpdate = Context.RecipeStep
                                             .Where(r => r.RecipeStepId == updateDto.RecipeStepId)
                                             .FirstOrDefault();


            // only update the properties that are not null in the DTO
            if (updateDto.SortOrder.HasValue)
            {
                stepToUpdate.SortOrder = updateDto.SortOrder.Value;
            }

            if (updateDto.Text != null)
            {
                stepToUpdate.Text = updateDto.Text;
            }


            ValidateProperties(stepToUpdate);

            Context.Entry(stepToUpdate).State = EntityState.Modified;
            await Context.SaveChangesAsync().ConfigureAwait(false);

            return;
        }

        /// <summary>
        /// Updates step in recipe if user has rights to it (i.e. added the recipe)
        /// </summary>
        public async Task UpdateRecipeStepAsync(RecipeStep recipeStep, PantryPlannerUser userUpdating)
        {
            if (recipeStep == null)
            {
                throw new ArgumentNullException(nameof(recipeStep));
            }

            if (userUpdating == null)
            {
                throw new ArgumentNullException(nameof(userUpdating));
            }

            if (Context.RecipeStepExists(recipeStep) == false)
            {
                throw new RecipeStepNotFoundException(recipeStep.RecipeStepId);
            }

            if (Permissions.UserAddedRecipe(recipeStep.RecipeId, userUpdating) == false)
            {
                throw new PermissionsException("You do not have rights to update this recipe");
            }

            ValidateProperties(recipeStep);

            Context.Entry(recipeStep).State = EntityState.Modified;
            await Context.SaveChangesAsync().ConfigureAwait(false);

            return;
        }

        #endregion


        #region Delete Methods

        /// <summary>
        /// Deletes step from recipe if user has rights to it (i.e. added the recipe)
        /// </summary>
        public RecipeStep DeleteRecipeStep(RecipeStep stepToDelete, PantryPlannerUser userDeleting)
        {
            if (stepToDelete == null)
            {
                throw new ArgumentNullException(nameof(stepToDelete));
            }

            return DeleteRecipeStep(stepToDelete.RecipeStepId, userDeleting);
        }

        /// <summary>
        /// Deletes step from recipe if user has rights to it (i.e. added the recipe)
        /// </summary>
        public RecipeStep DeleteRecipeStep(long recipeStepId, PantryPlannerUser userDeleting)
        {
            if (userDeleting == null)
            {
                throw new ArgumentNullException(nameof(userDeleting));
            }

            if (Context.RecipeStepExists(recipeStepId) == false)
            {
                throw new RecipeStepNotFoundException(recipeStepId);
            }

            if (Permissions.UserHasEditRightsToRecipeStep(recipeStepId, userDeleting) == false)
            {
                throw new PermissionsException($"You do not have rights to modify this recipe");
            }

            RecipeStep stepToDelete = Context.RecipeStep.Where(r => r.RecipeStepId == recipeStepId).FirstOrDefault();

            Context.RecipeStep.Remove(stepToDelete);
            Context.SaveChanges();

            return stepToDelete;
        }

        #endregion


        #region Helper Methods


        /// <summary>
        /// Throws exception if properties fail validation
        /// </summary>
        /// <param name="recipeStep"></param>
        private static void ValidateProperties(RecipeStep recipeStep)
        {
            // validate sort order > 0
            if (recipeStep.SortOrder <= 0)
            {
                throw new InvalidOperationException($"Sort Order must be greater than zero");
            }

            int maxLength = 500;
            if (recipeStep.Text.Length > maxLength)
            {
                throw new InvalidOperationException($"Text length cannot exceed {maxLength} characters");
            }
        }

        private int GetNextSortOrderForRecipeStep(long recipeId)
        {
            if (Context.RecipeExists(recipeId) == false)
            {
                throw new RecipeNotFoundException(recipeId);
            }

            if (Context.RecipeStep.Any(r => r.RecipeId == recipeId) == false)
            {
                return 1;
            }

            int maxSortOrder = Context.RecipeStep.Where(r => r.RecipeId == recipeId)
                                                       .ToList() // brings the list of recipe ingredients to the client side (api) to finish calculation instead of doing Max() on the Database
                                                       .Max(r => r.SortOrder);

            return maxSortOrder + 1;
        }

        #endregion
    }
}
