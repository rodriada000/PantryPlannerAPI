using PantryPlanner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PantryPlanner.DTOs
{
    /// <summary>
    /// DTO of <see cref="KitchenUser"/> that excludes any Collections.
    /// </summary>
    public class KitchenUserDto
    {
        public KitchenUserDto(KitchenUser user)
        {
            KitchenUserId = user.KitchenUserId;
            UserId = user.UserId;
            KitchenId = user.KitchenId;
            IsOwner = user.IsOwner;
            DateAdded = user.DateAdded;
            HasAcceptedInvite = user.HasAcceptedInvite;
        }

        public long KitchenUserId { get; set; }
        public string UserId { get; set; }
        public long KitchenId { get; set; }
        public bool IsOwner { get; set; }
        public bool HasAcceptedInvite { get; set; }
        public DateTime DateAdded { get; set; }
    }
}
