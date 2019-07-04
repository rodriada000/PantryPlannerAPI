using System;
using System.Collections.Generic;

namespace PantryPlanner.Models
{
    public partial class User
    {
        public User()
        {
            Ingredient = new HashSet<Ingredient>();
            KitchenUser = new HashSet<KitchenUser>();
            Recipe = new HashSet<Recipe>();
        }

        public long UserId { get; set; }
        public Guid UniquePublicGuid { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }

        public virtual ICollection<Ingredient> Ingredient { get; set; }
        public virtual ICollection<KitchenUser> KitchenUser { get; set; }
        public virtual ICollection<Recipe> Recipe { get; set; }
    }
}
