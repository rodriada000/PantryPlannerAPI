using Microsoft.AspNetCore.Mvc;
using PantryPlanner.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PantryPlanner.DTOs
{
    /// <summary>
    /// DTO of <see cref="Kitchen"/> that excludes any Collections.
    /// </summary>
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

        internal static List<KitchenDto> ToList(List<Kitchen> list)
        {
            return list?.Select(k => new KitchenDto(k))?.ToList();
        }
    }
}
