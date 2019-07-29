using PantryPlanner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PantryPlanner.DTOs
{
    public class CategoryDto
    {
        public long CategoryId { get; set; }
        public int? CategoryTypeId { get; set; }
        public long? CreatedByKitchenId { get; set; }
        public string Name { get; set; }

        #region Additional Properties Not In Model

        public string CategoryTypeName { get; set; }

        #endregion

        public CategoryDto(Category category)
        {
            if (category == null)
            {
                return;
            }

            CategoryId = category.CategoryId;
            CategoryTypeId = category.CategoryTypeId;
            CreatedByKitchenId = category.CreatedByKitchenId;
            Name = category.Name;

            CategoryTypeName = category.CategoryType?.Name;
        }

        public static List<CategoryDto> ToList(List<Category> list)
        {
            return list?.Select(k => new CategoryDto(k))?.ToList();
        }

        public override string ToString()
        {
            return $"c: {Name}... {CategoryTypeName}";
        }
    }
}
