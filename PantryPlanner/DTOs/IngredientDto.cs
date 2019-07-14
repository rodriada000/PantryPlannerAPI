using PantryPlanner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PantryPlanner.DTOs
{
    /// <summary>
    /// DTO of <see cref="Ingredient"/> that excludes any Collections.
    /// </summary>
    public class IngredientDto
    {
        public long IngredientId { get; set; }
        public string AddedByUserId { get; set; }
        public long? CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public byte[] PreviewPicture { get; set; }
        public DateTime DateAdded { get; set; }
        public bool IsPublic { get; set; }

        #region Additional Properties Not In Model

        public string CategoryName { get; set; }
        public string AddedByUserName { get; set; }

        #endregion

        public IngredientDto(Ingredient ingredient)
        {
            IngredientId = ingredient.IngredientId;
            AddedByUserId = ingredient.AddedByUserId;
            CategoryId = ingredient.CategoryId;
            Name = ingredient.Name;
            Description = ingredient.Description;
            PreviewPicture = ingredient.PreviewPicture;
            DateAdded = ingredient.DateAdded;
            IsPublic = ingredient.IsPublic;

            CategoryName = ingredient.Category?.Name;
            AddedByUserName = ingredient.AddedByUser?.UserName;
        }

        public static List<IngredientDto> ToList(List<Ingredient> list)
        {
            return list?.Select(i => new IngredientDto(i)).ToList();
        }

        public override string ToString()
        {
            return $"i: {Name} | {CategoryName}";
        }
    }
}
