using System;
using System.Collections.Generic;

namespace PantryPlanner.Models
{
    public partial class KitchenRecipe
    {
        public long KitchenRecipeId { get; set; }
        public long KitchenId { get; set; }
        public long RecipeId { get; set; }
        public long? CategoryId { get; set; }
        public bool IsFavorite { get; set; }
        public string Note { get; set; }

        public virtual Category Category { get; set; }
        public virtual Kitchen Kitchen { get; set; }
        public virtual Recipe Recipe { get; set; }
    }
}
