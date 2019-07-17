using PantryPlanner.Models;
using PantryPlanner.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PantryPlanner.Extensions
{
    public static class PantryPlannerContextExtensions
    {
        public static bool UserExists(this PantryPlannerContext context, string usernameOrId)
        {
            return context.Users.Any(u => u.UserName == usernameOrId || u.Id == usernameOrId);
        }

        public static bool KitchenExists(this PantryPlannerContext context, long kitchenId)
        {
            return context.Kitchen.Any(e => e.KitchenId == kitchenId);
        }

        public static bool KitchenUserExists(this PantryPlannerContext context, long kitchenUserId)
        {
            return context.KitchenUser.Any(e => e.KitchenUserId == kitchenUserId);
        }

        public static bool KitchenUserExists(this PantryPlannerContext context, long kitchenId, string userId)
        {
            return context.KitchenUser.Any(e => e.KitchenId == kitchenId && e.UserId == userId);
        }

        public static bool KitchenUserExists(this PantryPlannerContext context, Kitchen kitchen, PantryPlannerUser user)
        {
            if (kitchen == null || user == null)
            {
                return false;
            }

            return context.KitchenUserExists(kitchen.KitchenId, user.Id);
        }

        public static bool CategoryExists(this PantryPlannerContext context, long categoryId)
        {
            return context.Category.Any(c => c.CategoryId == categoryId);
        }

        public static bool IngredientExists(this PantryPlannerContext context, long ingredientId)
        {
            return context.Ingredient.Any(i => i.IngredientId == ingredientId);
        }


        public static bool IngredientExistsPublicly(this PantryPlannerContext context, Ingredient ingredient)
        {
            if (ingredient == null)
            {
                return false;
            }

            return context.Ingredient.Any(i => i.Name == ingredient.Name && i.CategoryId == ingredient.CategoryId && i.IsPublic);
        }

        public static bool IngredientExistsForUser(this PantryPlannerContext context, Ingredient ingredient, PantryPlannerUser user)
        {
            if (ingredient == null || user == null)
            {
                return false;
            }


            // check public ingredients if ingredient being checked is marked as public
            if (ingredient.IsPublic && context.IngredientExistsPublicly(ingredient))
            {
                return true;
            }

            // check that user added the ingredient and it is non-public
            return context.Ingredient.Any(i => i.Name == ingredient.Name && i.CategoryId == ingredient.CategoryId && i.AddedByUserId == user.Id && i.IsPublic == false);
        }

        public static bool KitchenIngredientExists(this PantryPlannerContext context, long kitchenIngredientId)
        {
            return context.KitchenIngredient.Any(i => i.KitchenIngredientId == kitchenIngredientId);
        }

        public static bool IngredientExistsForKitchen(this PantryPlannerContext context, KitchenIngredient ingredient)
        {
            if (ingredient == null)
            {
                return false;
            }

            return context.IngredientExistsForKitchen(ingredient.IngredientId, ingredient.KitchenId);
        }

        public static bool IngredientExistsForKitchen(this PantryPlannerContext context, Ingredient ingredient, Kitchen kitchen)
        {
            if (ingredient == null || kitchen == null)
            {
                return false;
            }

            return context.IngredientExistsForKitchen(ingredient.IngredientId, kitchen.KitchenId);
        }

        public static bool IngredientExistsForKitchen(this PantryPlannerContext context, long ingredientId, long kitchenId)
        {
            return context.KitchenIngredient.Any(i => i.KitchenId == kitchenId && i.IngredientId == ingredientId);
        }

    }
}
