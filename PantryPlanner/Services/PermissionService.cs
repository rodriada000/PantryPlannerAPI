using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PantryPlanner.Extensions;
using PantryPlanner.Models;

namespace PantryPlanner.Services
{
    public class PermissionService : IPantryService
    {
        public PantryPlannerContext Context { get; set; }

        public PermissionService(PantryPlannerContext context)
        {
            Context = context;
        }


        #region Kitchen Related Permissions Checks

        internal bool UserHasRightsToKitchen(PantryPlannerUser user, long kitchenId)
        {
            Kitchen kitchen = Context.Kitchen.Find(kitchenId);
            return UserHasRightsToKitchen(user, kitchen);
        }

        internal bool UserHasRightsToKitchen(PantryPlannerUser user, Kitchen kitchen)
        {
            if (Context == null || user == null || kitchen == null || Context.KitchenUser == null)
            {
                return false;
            }

            return Context.KitchenUser.Any(x => x.KitchenId == kitchen.KitchenId && x.UserId == user.Id);
        }

        internal bool UserOwnsKitchen(PantryPlannerUser user, long kitchenId)
        {
            if (Context.KitchenExists(kitchenId) == false)
            {
                return false;
            }

            Kitchen kitchen = Context.Kitchen.Find(kitchenId);
            return Context.KitchenUser.Any(x => x.KitchenId == kitchen.KitchenId && x.UserId == user.Id && x.IsOwner);
        }

        internal bool UserOwnsKitchen(PantryPlannerUser user, Kitchen kitchen)
        {
            if (user == null || kitchen == null)
            {
                return false;
            }

            return UserOwnsKitchen(user, kitchen.KitchenId);
        }

        #endregion


        #region Ingredient Related Permissions Checks

        internal bool UserAddedIngredient(Ingredient ingredient, PantryPlannerUser user)
        {
            if (ingredient == null)
            {
                return false;
            }

            return UserAddedIngredient(ingredient.IngredientId, user);
        }

        internal bool UserAddedIngredient(long ingredientId, PantryPlannerUser user)
        {
            if (user == null)
            {
                return false;
            }

            return UserAddedIngredient(ingredientId, user.Id);
        }

        internal bool UserAddedIngredient(long ingredientId, string userId)
        {
            if (String.IsNullOrWhiteSpace(userId))
            {
                return false;
            }

            return Context.Ingredient.Any(i => i.IngredientId == ingredientId && i.AddedByUserId == userId);
        }

        /// <summary>
        /// User has rights to ingredient if they added it or it is public.
        /// </summary>
        internal bool UserHasViewRightsToIngredient(long ingredientId, PantryPlannerUser user)
        {
            if (Context.IngredientExists(ingredientId) == false)
            {
                return false;
            }

            return UserAddedIngredient(ingredientId, user) || Context.Ingredient.Any(i => i.IngredientId == ingredientId && i.IsPublic);
        }

        /// <summary>
        /// User has rights to ingredient if they added it or it is public.
        /// </summary>
        internal bool UserHasViewRightsToIngredient(Ingredient ingredient, PantryPlannerUser user)
        {
            if (ingredient == null)
            {
                return false;
            }

            return UserHasViewRightsToIngredient(ingredient.IngredientId, user);
        }

        #endregion


        #region Recipe Related Permissions Checks

        internal bool UserAddedRecipe(Recipe recipe, PantryPlannerUser user)
        {
            if (recipe == null)
            {
                return false;
            }

            return UserAddedRecipe(recipe.RecipeId, user);
        }

        internal bool UserAddedRecipe(long recipeId, PantryPlannerUser user)
        {
            if (user == null)
            {
                return false;
            }

            return Context.Recipe.Any(r => r.RecipeId == recipeId && r.CreatedByUserId == user.Id);
        }


        /// <summary>
        /// User has rights to recipe if they added it or it is public.
        /// </summary>
        internal bool UserHasViewRightsToRecipe(long recipeId, PantryPlannerUser user)
        {
            if (user == null || Context.RecipeExists(recipeId) == false)
            {
                return false;
            }

            return UserAddedRecipe(recipeId, user) || Context.Recipe.Any(r => r.RecipeId == recipeId && r.IsPublic.GetValueOrDefault(false));
        }


        internal bool UserHasViewRightsToRecipeIngredient(RecipeIngredient recipeIngredient, PantryPlannerUser user)
        {
            if (user == null || recipeIngredient == null  || Context.RecipeIngredientExists(recipeIngredient) == false)
            {
                return false;
            }

            return UserHasViewRightsToRecipe(recipeIngredient.RecipeId, user);
        }

        internal bool UserHasViewRightsToRecipeIngredient(long recipeIngredientId, PantryPlannerUser user)
        {
            if (user == null || Context.RecipeIngredientExists(recipeIngredientId) == false)
            {
                return false;
            }

            RecipeIngredient recipeIngredient = Context.RecipeIngredient.Where(r => r.RecipeIngredientId == recipeIngredientId).FirstOrDefault();

            return UserHasViewRightsToRecipeIngredient(recipeIngredient, user);
        }

        internal bool UserHasEditRightsToRecipeIngredient(long recipeIngredientId, PantryPlannerUser user)
        {
            if (user == null || Context.RecipeIngredientExists(recipeIngredientId) == false)
            {
                return false;
            }

            RecipeIngredient recipeIngredient = Context.RecipeIngredient.Where(r => r.RecipeIngredientId == recipeIngredientId).FirstOrDefault();

            return UserAddedRecipe(recipeIngredient.RecipeId, user);
        }


        internal bool UserHasViewRightsToRecipeStep(RecipeStep recipeStep, PantryPlannerUser user)
        {
            if (user == null || recipeStep == null || Context.RecipeStepExists(recipeStep) == false)
            {
                return false;
            }

            return UserHasViewRightsToRecipe(recipeStep.RecipeId, user);
        }

        internal bool UserHasViewRightsToRecipeStep(long recipeStepId, PantryPlannerUser user)
        {
            if (user == null || Context.RecipeStepExists(recipeStepId) == false)
            {
                return false;
            }

            RecipeStep recipeStep = Context.RecipeStep.Where(r => r.RecipeStepId == recipeStepId).FirstOrDefault();

            return UserHasViewRightsToRecipeStep(recipeStep, user);
        }

        internal bool UserHasEditRightsToRecipeStep(long recipeStepId, PantryPlannerUser user)
        {
            if (user == null || Context.RecipeStepExists(recipeStepId) == false)
            {
                return false;
            }

            RecipeStep recipeStep = Context.RecipeStep.Where(r => r.RecipeStepId == recipeStepId).FirstOrDefault();

            return UserAddedRecipe(recipeStep.RecipeId, user);
        }

        #endregion

    }
}
