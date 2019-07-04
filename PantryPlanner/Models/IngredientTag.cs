using System;
using System.Collections.Generic;

namespace PantryPlanner.Models
{
    public partial class IngredientTag
    {
        public long IngredientTagId { get; set; }
        public long IngredientId { get; set; }
        public long KitchenId { get; set; }
        public long? CreatedByKitchenUserId { get; set; }
        public string TagName { get; set; }

        public virtual KitchenUser CreatedByKitchenUser { get; set; }
        public virtual Ingredient Ingredient { get; set; }
        public virtual Kitchen Kitchen { get; set; }
    }
}
