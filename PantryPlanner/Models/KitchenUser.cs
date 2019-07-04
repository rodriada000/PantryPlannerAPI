using System;
using System.Collections.Generic;

namespace PantryPlanner.Models
{
    public partial class KitchenUser
    {
        public KitchenUser()
        {
            IngredientTag = new HashSet<IngredientTag>();
            KitchenIngredient = new HashSet<KitchenIngredient>();
            MealPlan = new HashSet<MealPlan>();
        }

        public long KitchenUserId { get; set; }
        public long UserId { get; set; }
        public long KitchenId { get; set; }
        public bool IsOwner { get; set; }
        public DateTime DateAdded { get; set; }

        public virtual Kitchen Kitchen { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<IngredientTag> IngredientTag { get; set; }
        public virtual ICollection<KitchenIngredient> KitchenIngredient { get; set; }
        public virtual ICollection<MealPlan> MealPlan { get; set; }
    }
}
