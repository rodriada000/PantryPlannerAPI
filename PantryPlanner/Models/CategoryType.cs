using System;
using System.Collections.Generic;

namespace PantryPlanner.Models
{
    public partial class CategoryType
    {
        public CategoryType()
        {
            Category = new HashSet<Category>();
        }

        public int CategoryTypeId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Category> Category { get; set; }
    }
}
