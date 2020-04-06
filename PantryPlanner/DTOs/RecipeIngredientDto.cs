using PantryPlanner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PantryPlanner.DTOs
{
    public class RecipeIngredientDto
    {
        public int RecipeIngredientId { get; set; }
        public long IngredientId { get; set; }
        public long RecipeId { get; set; }
        public decimal? Quantity { get; set; }
        public string UnitOfMeasure { get; set; }
        public string Method { get; set; }
        public int? SortOrder { get; set; }

        #region Additional Properties Not In Model

        public IngredientDto Ingredient { get; set; }

        #endregion

        public RecipeIngredientDto()
        {
            
        }

        public RecipeIngredientDto(RecipeIngredient ingredient)
        {
            if (ingredient == null)
            {
                return;
            }

            RecipeIngredientId = ingredient.RecipeIngredientId;
            IngredientId = ingredient.IngredientId;
            RecipeId = ingredient.RecipeId;
            Quantity = ingredient.Quantity;
            UnitOfMeasure = ingredient.UnitOfMeasure;
            Method = ingredient.Method;
            SortOrder = ingredient.SortOrder;
            Ingredient = new IngredientDto(ingredient.Ingredient);
        }

        public static List<RecipeIngredientDto> ToList(IEnumerable<RecipeIngredient> list)
        {
            return list?.Select(k => new RecipeIngredientDto(k))?.ToList();
        }

        public override string ToString()
        {
            return $"ri: i = {IngredientId} qty = {Quantity} uom = {UnitOfMeasure} m = {Method}";
        }

        internal RecipeIngredient Create()
        {
            return new RecipeIngredient()
            {
                RecipeIngredientId = this.RecipeIngredientId,
                IngredientId = this.IngredientId,
                RecipeId = this.RecipeId,
                Quantity = this.Quantity.GetValueOrDefault(0),
                UnitOfMeasure = this.UnitOfMeasure,
                Method = this.Method,
                SortOrder = this.SortOrder.GetValueOrDefault(-1)
            };
        }
    }
}
