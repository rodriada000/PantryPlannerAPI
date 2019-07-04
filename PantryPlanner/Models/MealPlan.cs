using System;
using System.Collections.Generic;

namespace PantryPlanner.Models
{
    public partial class MealPlan
    {
        public MealPlan()
        {
            MealPlanRecipe = new HashSet<MealPlanRecipe>();
        }

        public long MealPlanId { get; set; }
        public long KitchenId { get; set; }
        public long? CreatedByKitchenUserId { get; set; }
        public long? CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DateCreated { get; set; }
        public int SortOrder { get; set; }
        public bool IsFavorite { get; set; }

        public virtual Category Category { get; set; }
        public virtual KitchenUser CreatedByKitchenUser { get; set; }
        public virtual Kitchen Kitchen { get; set; }
        public virtual ICollection<MealPlanRecipe> MealPlanRecipe { get; set; }
    }
}
