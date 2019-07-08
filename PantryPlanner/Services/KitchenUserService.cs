using Microsoft.EntityFrameworkCore;
using PantryPlanner.DTOs;
using PantryPlanner.Exceptions;
using PantryPlanner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PantryPlanner.Services
{
    public class KitchenUserService : IPantryService
    {
        public PantryPlannerContext Context { get; set ; }

        public PermissionService Permissions { get; set; }

        public KitchenUserService(PantryPlannerContext context)
        {
            Context = context;
            Permissions = new PermissionService(Context);
        }

        public List<KitchenUserDto> GetUsersForKitchen(Kitchen kitchen, PantryPlannerUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user is null");
            }

            if (kitchen == null)
            {
                throw new ArgumentNullException("kitchen is null");
            }

            return GetUsersForKitchenById(kitchen.KitchenId, user);
        }

        public List<KitchenUserDto> GetUsersForKitchenById(long id, PantryPlannerUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user is null");
            }

            Kitchen kitchen = Context?.Kitchen.Find(id);

            if (kitchen == null)
            {
                throw new ArgumentNullException($"kitchen with ID {id} does not exist");
            }

            if (!Permissions.UserHasRightsToKitchen(user, kitchen))
            {
                throw new PermissionsException("User does not have rights to kitchen.");
            }

            return Context.KitchenUser.Where(x => x.KitchenId == id && (x.HasAcceptedInvite.HasValue && x.HasAcceptedInvite.Value)).Select(k => new KitchenUserDto(k)).ToList();
        }

        public bool InviteUserToKitchenByUsername(string username, Kitchen kitchen, PantryPlannerUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user is null");
            }

            if (kitchen == null)
            {
                throw new ArgumentNullException("kitchen is null");
            }

            if (String.IsNullOrEmpty(username))
            {
                throw new ArgumentNullException("username is required");
            }

            return InviteUserToKitchenByUsername(username, kitchen.KitchenId, user);
        }

        public bool InviteUserToKitchenByUsername(string username, long kitchenId, PantryPlannerUser user)
        {
            if (String.IsNullOrEmpty(username))
            {
                throw new ArgumentNullException("username is required");
            }

            if (user == null)
            {
                throw new ArgumentNullException("user is null");
            }

            Kitchen kitchen = Context?.Kitchen.Find(kitchenId);

            if (kitchen == null)
            {
                throw new ArgumentNullException($"kitchen with ID {kitchenId} does not exist");
            }

            // validate user exists based on username
            PantryPlannerUser userToInvite = Context.Users.Where(u => u.UserName == username).FirstOrDefault();

            if (userToInvite == null)
            {
                throw new UserNotFoundException($"No user found with the username {username}");
            }

            return InviteUserToKitchenByUserId(userToInvite.Id, kitchenId, user);
        }

        public bool InviteUserToKitchenByUserId(string userId, long kitchenId, PantryPlannerUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user is null");
            }

            Kitchen kitchen = Context?.Kitchen.Find(kitchenId);

            if (kitchen == null)
            {
                throw new ArgumentNullException($"kitchen with ID {kitchenId} does not exist");
            }

            // validate user has rights to Kitchen
            if (!Permissions.UserHasRightsToKitchen(user, kitchen))
            {
                throw new PermissionsException("you do not have rights to this kitchen");
            }

            // add user to KitchenUser
            Context.KitchenUser.Add(new KitchenUser
            {
                KitchenId = kitchen.KitchenId,
                UserId = userId,
                HasAcceptedInvite = false, // user has not accepted invite 
                IsOwner = false,
                DateAdded = DateTime.Now
            });

            return true;
        }

        public bool UpdateKitchenUser(KitchenUser kitchenUser, PantryPlannerUser user)
        {
            if (kitchenUser == null)
            {
                throw new ArgumentNullException("cannot update; object is null");
            }

            if (!Permissions.UserHasRightsToKitchen(user, kitchenUser.KitchenId))
            {
                throw new PermissionsException("you do not have rights to this kitchen");
            }


            Context.Entry(kitchenUser).State = EntityState.Modified;
            Context.SaveChanges();
            return true;
        }

        public KitchenUser DeleteKitchenUserById(long kitchenUserId, PantryPlannerUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("cannot delete; user is null");
            }

            var kitchenUser = Context.KitchenUser.Find(kitchenUserId);
            if (kitchenUser == null)
            {
                throw new ArgumentNullException($"cannot delete id {kitchenUserId} because it does not exist");
            }

            // validate user has rights to kitchen
            if (!Permissions.UserHasRightsToKitchen(user, kitchenUser.KitchenId))
            {
                throw new PermissionsException("you do not have rights to this kitchen");
            }

            // validate user owns the kitchen
            if (!Permissions.UserOwnsKitchen(user, kitchenUser.KitchenId))
            {
                throw new PermissionsException($"you do not have rights to delete the kitchen '{kitchenUser.Kitchen.Name}'");
            }

            int numberOfUsers = kitchenUser.Kitchen.KitchenUser.Count;

            // validate user is not removing themselves since they own the kitchen 
            if (kitchenUser.UserId == user.Id)
            {
                throw new InvalidOperationException("You can not remove yourself from the Kitchen because you own it. Transfer ownership or delete the kitchen instead.");
            }


            Context.KitchenUser.Remove(kitchenUser);
            Context.SaveChanges();

            return kitchenUser;
        }
    }
}
