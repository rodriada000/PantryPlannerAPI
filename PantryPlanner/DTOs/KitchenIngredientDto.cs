using PantryPlanner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PantryPlanner.DTOs
{
    public class KitchenIngredientDto
    {
        public long KitchenIngredientId { get; set; }
        public long IngredientId { get; set; }
        public long KitchenId { get; set; }
        public long? AddedByKitchenUserId { get; set; }
        public long? CategoryId { get; set; }
        public DateTime LastUpdated { get; set; }
        public int? Quantity { get; set; }
        public string Note { get; set; }

        #region Additional Properties Not In Model

        public IngredientDto Ingredient { get; set; }

        public KitchenDto Kitchen { get; set; }

        public CategoryDto Category { get; set; }

        #endregion

        public KitchenIngredientDto()
        {

        }

        public KitchenIngredientDto(KitchenIngredient kitchenIngredient)
        {
            if (kitchenIngredient == null)
            {
                return;
            }

            KitchenIngredientId = kitchenIngredient.KitchenIngredientId;
            IngredientId = kitchenIngredient.IngredientId;
            KitchenId = kitchenIngredient.KitchenId;
            AddedByKitchenUserId = kitchenIngredient.AddedByKitchenUserId;
            CategoryId = kitchenIngredient.CategoryId;
            LastUpdated = kitchenIngredient.LastUpdated;
            Quantity = kitchenIngredient.Quantity;
            Note = kitchenIngredient.Note;


            Ingredient = new IngredientDto(kitchenIngredient.Ingredient);
            Kitchen = new KitchenDto(kitchenIngredient.Kitchen);

            if (kitchenIngredient.Category != null)
            {
                Category = new CategoryDto(kitchenIngredient.Category);
            }
        }

        public override string ToString()
        {
            return $"ki: {Ingredient.Name} x {Quantity}... in {Kitchen.Name}";
        }

        public static List<KitchenIngredientDto> ToList(List<KitchenIngredient> list)
        {
            return list?.Select(k => new KitchenIngredientDto(k))?.ToList();
        }

        internal static KitchenIngredient Create(KitchenIngredientDto kitchenIngredient)
        {
            if (kitchenIngredient == null)
            {
                return null;
            }

            return new KitchenIngredient()
            {
                KitchenIngredientId = kitchenIngredient.KitchenIngredientId,
                KitchenId = kitchenIngredient.KitchenId,
                IngredientId = kitchenIngredient.IngredientId,
                AddedByKitchenUserId = kitchenIngredient.AddedByKitchenUserId,
                CategoryId = kitchenIngredient.CategoryId,
                Note = kitchenIngredient.Note,
                Quantity = kitchenIngredient.Quantity,
                LastUpdated = kitchenIngredient.LastUpdated,
            };
        }
    }
}
