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
        public long KitchenUserId { get; set; }
        public string UserId { get; set; }
        public long KitchenId { get; set; }
        public bool IsOwner { get; set; }
        public bool HasAcceptedInvite { get; set; }
        public DateTime DateAdded { get; set; }

        #region Properties Not In Model

        public string Username { get; set; }
        public string KitchenName { get; set; }

        #endregion

        public KitchenUserDto(KitchenUser user)
        {
            if (user == null)
            {
                return;
            }
            
            KitchenUserId = user.KitchenUserId;
            UserId = user.UserId;
            KitchenId = user.KitchenId;
            IsOwner = user.IsOwner;
            DateAdded = user.DateAdded;
            HasAcceptedInvite = user.HasAcceptedInvite.Value;

            Username = user.User?.UserName;
            KitchenName = user.Kitchen?.Name;
        }


        public static List<KitchenUserDto> ToList(List<KitchenUser> users)
        {
            return users?.Select(k => new KitchenUserDto(k)).ToList();
        }

        public override string ToString()
        {
            return $"ku: {Username} in {KitchenName}";
        }
    }
}
