using PantryPlanner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PantryPlanner.DTOs
{
    public class KitchenDto
    {
        public KitchenDto(Kitchen kitchen)
        {
            KitchenId = kitchen.KitchenId;
            UniquePublicGuid = kitchen.UniquePublicGuid;
            Name = kitchen.Name;
            Description = kitchen.Description;
            DateCreated = kitchen.DateCreated;
            CreatedByUserId = kitchen.CreatedByUserId;
        }

        public long KitchenId { get; set; }
        public Guid UniquePublicGuid { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DateCreated { get; set; }
        public string CreatedByUserId { get; set; }
    }
}
