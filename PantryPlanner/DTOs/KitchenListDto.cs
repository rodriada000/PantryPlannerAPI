using PantryPlanner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PantryPlanner.DTOs
{
    public class KitchenListDto
    {
        public long KitchenListId { get; set; }
        public long KitchenId { get; set; }
        public long? CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Name { get; set; }

        public KitchenListDto()
        {
        }

        public KitchenListDto(KitchenList model)
        {
            if (model == null)
            {
                return;
            }

            KitchenListId = model.KitchenListId;
            KitchenId = model.KitchenId;
            CategoryId = model.CategoryId;
            CategoryName = model.Category?.Name;
            Name = model.Name;
        }


        public static List<KitchenListDto> ToList(List<KitchenList> list)
        {
            return list?.Select(k => new KitchenListDto(k))?.ToList();
        }

        public override string ToString()
        {
            return $"kl: {Name} | {CategoryName} | k id {KitchenId}";
        }
    }
}
