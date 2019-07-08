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

            return Context.KitchenUser.Where(x => x.KitchenId == id).Select(k => new KitchenUserDto(k)).ToList();
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

        public bool InviteUserToKitchenByUsername(string username, long id, PantryPlannerUser user)
        {
            if (String.IsNullOrEmpty(username))
            {
                throw new ArgumentNullException("username is required");
            }

            if (user == null)
            {
                throw new ArgumentNullException("user is null");
            }

            Kitchen kitchen = Context?.Kitchen.Find(id);

            if (kitchen == null)
            {
                throw new ArgumentNullException($"kitchen with ID {id} does not exist");
            }

            return true;
        }

        public bool InviteUserToKitchenByUserId()
        {
            return true;
        }
    }
}
