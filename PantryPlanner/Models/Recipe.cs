using System;
using System.Collections.Generic;

namespace PantryPlanner.Models
{
    public partial class Recipe
    {
        public Recipe()
        {
            KitchenRecipe = new HashSet<KitchenRecipe>();
            MealPlanRecipe = new HashSet<MealPlanRecipe>();
            RecipeIngredient = new HashSet<RecipeIngredient>();
            RecipeStep = new HashSet<RecipeStep>();
        }

        public long RecipeId { get; set; }
        public string CreatedByUserId { get; set; }
        public string RecipeUrl { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? PrepTime { get; set; }
        public int? CookTime { get; set; }
        public string ServingSize { get; set; }
        public DateTime DateCreated { get; set; }
        public bool? IsPublic { get; set; }

        public virtual PantryPlannerUser CreatedByUser { get; set; }
        public virtual ICollection<KitchenRecipe> KitchenRecipe { get; set; }
        public virtual ICollection<MealPlanRecipe> MealPlanRecipe { get; set; }
        public virtual ICollection<RecipeIngredient> RecipeIngredient { get; set; }
        public virtual ICollection<RecipeStep> RecipeStep { get; set; }
    }
}
