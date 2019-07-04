using System;
using System.Collections.Generic;

namespace PantryPlanner.Models
{
    public partial class MealPlanRecipe
    {
        public int MealPlanRecipeId { get; set; }
        public int SortOrder { get; set; }
        public long RecipeId { get; set; }
        public long MealPlanId { get; set; }

        public virtual MealPlan MealPlan { get; set; }
        public virtual Recipe Recipe { get; set; }
    }
}
