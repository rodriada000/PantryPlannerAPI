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

            KitchenUser kitchenUser = Context.KitchenUser
                                             .Where(x => x.KitchenId == kitchen.KitchenId && x.UserId == user.Id)
                                             .FirstOrDefault();

            if (kitchenUser == null)
            {
                return false;
            }

            return kitchenUser.IsOwner;
        }

    }
}
