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

        internal bool UserHasRightsToKitchen(PantryPlannerUser user, Kitchen kitchen)
        {
            if (Context == null || user == null || kitchen == null || Context.KitchenUser == null)
            {
                return false;
            }

            return Context.KitchenUser.Any(x => x.KitchenId == kitchen.KitchenId && x.UserId == user.Id);
        }
    }
}
