using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace PantryPlanner.Models
{
    // Add profile data for application users by adding properties to the PantryPlannerUser class
    public class PantryPlannerUser : IdentityUser
    {
        public PantryPlannerUser()
        {
            Ingredient = new HashSet<Ingredient>();
            Kitchen = new HashSet<Kitchen>();
            KitchenUser = new HashSet<KitchenUser>();
            Recipe = new HashSet<Recipe>();
        }

        public virtual ICollection<Ingredient> Ingredient { get; set; }
        public virtual ICollection<Kitchen> Kitchen { get; set; }
        public virtual ICollection<KitchenUser> KitchenUser { get; set; }
        public virtual ICollection<Recipe> Recipe { get; set; }
    }
}
