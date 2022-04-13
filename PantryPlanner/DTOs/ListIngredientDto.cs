using PantryPlanner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PantryPlanner.DTOs
{
    public class ListIngredientDto
    {
        public int Id { get; set; }
        public long KitchenListId { get; set; }
        public long IngredientId { get; set; }
        //public long? AddedFromRecipeId { get; set; }
        public int? Quantity { get; set; }
        public string Note { get; set; }

        public int? SortOrder { get; set; }
        public bool IsChecked { get; set; }
        public long? CategoryId { get; set; }


        #region Additional Properties Not In Model

        public CategoryDto Category { get; set; }
        public IngredientDto Ingredient { get; set; }
        public long KitchenId { get; set; }


        #endregion

        public ListIngredientDto()
        {

        }

        public ListIngredientDto(KitchenListIngredient listIngredient)
        {
            if (listIngredient == null)
            {
                return;
            }

            Id = listIngredient.Id;
            KitchenListId = listIngredient.KitchenListId;
            KitchenId = listIngredient.KitchenList.KitchenId;
            IngredientId = listIngredient.IngredientId;
            Quantity = listIngredient.Quantity;
            SortOrder = listIngredient.SortOrder;
            IsChecked = listIngredient.IsChecked;
            Note = listIngredient.Note;

            Ingredient = new IngredientDto(listIngredient.Ingredient);

            if (listIngredient.CategoryId != null)
            {
                Category = new CategoryDto(listIngredient.Category);
            }

            if (listIngredient.Recipe != null)
            {
                // TODO: add RecipeDto
            }
        }

        internal static KitchenListIngredient Create(ListIngredientDto ingredient)
        {
            if (ingredient == null)
            {
                return null;
            }

            return new KitchenListIngredient()
            {
                Id = ingredient.Id,
                KitchenListId = ingredient.KitchenListId,
                IsChecked = ingredient.IsChecked,
                SortOrder = ingredient.SortOrder ?? 0,
                IngredientId = ingredient.IngredientId,
                Quantity = ingredient.Quantity
            };
        }

        public override string ToString()
        {
            return $"kli: {Ingredient.Name} x {Quantity} ...";
        }

        public static List<ListIngredientDto> ToList(List<KitchenListIngredient> list)
        {
            return list?.Select(k => new ListIngredientDto(k))?.ToList();
        }
    }
}
