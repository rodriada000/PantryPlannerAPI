using System;
using System.Collections.Generic;

namespace PantryPlanner.Models
{
    public partial class Category
    {
        public Category()
        {
            KitchenIngredient = new HashSet<KitchenIngredient>();
            KitchenList = new HashSet<KitchenList>();
            KitchenRecipe = new HashSet<KitchenRecipe>();
            MealPlan = new HashSet<MealPlan>();
        }

        public long CategoryId { get; set; }
        public int? CategoryTypeId { get; set; }
        public string Name { get; set; }
        public long CreatedByKitchenId { get; set; }

        public virtual CategoryType CategoryType { get; set; }
        public virtual Kitchen CreatedByKitchen { get; set; }
        public virtual ICollection<KitchenIngredient> KitchenIngredient { get; set; }
        public virtual ICollection<KitchenList> KitchenList { get; set; }
        public virtual ICollection<KitchenRecipe> KitchenRecipe { get; set; }
        public virtual ICollection<MealPlan> MealPlan { get; set; }
    }
}
