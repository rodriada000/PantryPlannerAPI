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
            return context.KitchenUserExists(kitchen.KitchenId, user.Id);
        }

    }
}
