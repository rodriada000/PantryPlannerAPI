using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            Kitchen kitchen = Context.Kitchen.Find(kitchenId);
            return UserOwnsKitchen(user, kitchen);
        }

        internal bool UserOwnsKitchen(PantryPlannerUser user, Kitchen kitchen)
        {
            if (user == null || kitchen == null)
            {
                return false;
            }

            return Context.KitchenUser.Any(x => x.KitchenId == kitchen.KitchenId && x.UserId == user.Id && x.IsOwner);
        }

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

    }
}
