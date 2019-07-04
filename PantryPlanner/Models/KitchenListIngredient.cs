using System;
using System.Collections.Generic;

namespace PantryPlanner.Models
{
    public partial class KitchenListIngredient
    {
        public int Id { get; set; }
        public long KitchenListId { get; set; }
        public long IngredientId { get; set; }
        public int? Quantity { get; set; }
        public int SortOrder { get; set; }
        public bool IsChecked { get; set; }

        public virtual Ingredient Ingredient { get; set; }
        public virtual KitchenList KitchenList { get; set; }
    }
}
