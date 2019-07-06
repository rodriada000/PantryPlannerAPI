using System;
using System.Collections.Generic;

namespace PantryPlanner.Models
{
    public partial class Kitchen
    {
        public Kitchen()
        {
            Category = new HashSet<Category>();
            IngredientTag = new HashSet<IngredientTag>();
            KitchenIngredient = new HashSet<KitchenIngredient>();
            KitchenList = new HashSet<KitchenList>();
            KitchenRecipe = new HashSet<KitchenRecipe>();
            KitchenUser = new HashSet<KitchenUser>();
            MealPlan = new HashSet<MealPlan>();
        }

        public long KitchenId { get; set; }
        public Guid UniquePublicGuid { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DateCreated { get; set; }
        public string CreatedByUserId { get; set; }

        public virtual ICollection<Category> Category { get; set; }
        public virtual ICollection<IngredientTag> IngredientTag { get; set; }
        public virtual ICollection<KitchenIngredient> KitchenIngredient { get; set; }
        public virtual ICollection<KitchenList> KitchenList { get; set; }
        public virtual ICollection<KitchenRecipe> KitchenRecipe { get; set; }
        public virtual ICollection<KitchenUser> KitchenUser { get; set; }
        public virtual ICollection<MealPlan> MealPlan { get; set; }
    }
}
