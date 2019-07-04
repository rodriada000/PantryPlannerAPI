using System;
using System.Collections.Generic;

namespace PantryPlanner.Models
{
    public partial class RecipeStep
    {
        public int RecipeStepId { get; set; }
        public long RecipeId { get; set; }
        public string Text { get; set; }
        public int SortOrder { get; set; }

        public virtual Recipe Recipe { get; set; }
    }
}
