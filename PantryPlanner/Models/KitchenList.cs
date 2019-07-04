using System;
using System.Collections.Generic;

namespace PantryPlanner.Models
{
    public partial class KitchenList
    {
        public KitchenList()
        {
            KitchenListIngredient = new HashSet<KitchenListIngredient>();
        }

        public long KitchenListId { get; set; }
        public long KitchenId { get; set; }
        public long? CategoryId { get; set; }
        public string Name { get; set; }

        public virtual Category Category { get; set; }
        public virtual Kitchen Kitchen { get; set; }
        public virtual ICollection<KitchenListIngredient> KitchenListIngredient { get; set; }
    }
}
