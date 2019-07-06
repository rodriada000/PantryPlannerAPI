using System;
using System.Collections.Generic;

namespace PantryPlanner.Models
{
    public partial class Ingredient
    {
        public Ingredient()
        {
            IngredientTag = new HashSet<IngredientTag>();
            KitchenIngredient = new HashSet<KitchenIngredient>();
            KitchenListIngredient = new HashSet<KitchenListIngredient>();
            RecipeIngredient = new HashSet<RecipeIngredient>();
        }

        public long IngredientId { get; set; }
        public string AddedByUserId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public byte[] PreviewPicture { get; set; }
        public DateTime DateAdded { get; set; }
        public bool IsPublic { get; set; }

        public virtual PantryPlannerUser AddedByUser { get; set; }
        public virtual ICollection<IngredientTag> IngredientTag { get; set; }
        public virtual ICollection<KitchenIngredient> KitchenIngredient { get; set; }
        public virtual ICollection<KitchenListIngredient> KitchenListIngredient { get; set; }
        public virtual ICollection<RecipeIngredient> RecipeIngredient { get; set; }
    }
}
