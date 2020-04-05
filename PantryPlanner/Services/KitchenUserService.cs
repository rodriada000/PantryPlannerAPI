using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PantryPlanner.DTOs;
using PantryPlanner.Exceptions;
using PantryPlanner.Extensions;
using PantryPlanner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PantryPlanner.Services
{
    public class KitchenUserService : IPantryService
    {
        public PantryPlannerContext Context { get; set; }

        public PermissionService Permissions { get; set; }

        public KitchenUserService(PantryPlannerContext context)
        {
            Context = context;
            Permissions = new PermissionService(Context);
        }


        #region Get Methods

        public List<KitchenUser> GetAllUsersForKitchen(Kitchen kitchen, PantryPlannerUser userAccessing)
        {
            if (kitchen == null)
            {
                throw new ArgumentNullException("kitchen is null");
            }

            return GetAllUsersForKitchenById(kitchen.KitchenId, userAccessing);
        }

        public List<KitchenUser> GetAllUsersForKitchenById(long id, PantryPlannerUser userAccessing)
        {
            if (userAccessing == null)
            {
                throw new ArgumentNullException("user is null");
            }

            Kitchen kitchen = Context?.Kitchen.Find(id);

            if (kitchen == null)
            {
                throw new KitchenNotFoundException(id);
            }

            if (!Permissions.UserHasRightsToKitchen(userAccessing, kitchen))
            {
                throw new PermissionsException();
            }

            return Context.KitchenUser
                    .Where(x => x.KitchenId == id)
                    .Include(x => x.User)
                    .Include(x => x.Kitchen)
                    .Select(k => k).ToList();
        }


        public List<KitchenUser> GetAcceptedUsersForKitchen(Kitchen kitchen, PantryPlannerUser userAccessing)
        {
            if (kitchen == null)
            {
                throw new ArgumentNullException("kitchen is null");
            }

            return GetAcceptedUsersForKitchenById(kitchen.KitchenId, userAccessing);
        }

        public List<KitchenUser> GetAcceptedUsersForKitchenById(long id, PantryPlannerUser userAccessing)
        {
            if (userAccessing == null)
            {
                throw new ArgumentNullException("user is null");
            }

            Kitchen kitchen = Context?.Kitchen.Find(id);

            if (kitchen == null)
            {
                throw new KitchenNotFoundException(id);
            }

            if (!Permissions.UserHasRightsToKitchen(userAccessing, kitchen))
            {
                throw new PermissionsException();
            }

            return Context.KitchenUser
                    .Where(x => x.KitchenId == id && x.HasAcceptedInvite.Value)
                    .Include(x => x.User)
                    .Include(x => x.Kitchen)
                    .Select(k => k).ToList();
        }

        public List<KitchenUser> GetUsersThatHaveNotAcceptedInvite(Kitchen kitchen, PantryPlannerUser userAccessing)
        {
            if (kitchen == null)
            {
                throw new ArgumentNullException("kitchen is null");
            }

            return GetUsersThatHaveNotAcceptedInviteByKitchenId(kitchen.KitchenId, userAccessing);
        }

        public List<KitchenUser> GetUsersThatHaveNotAcceptedInviteByKitchenId(long kitchenId, PantryPlannerUser userAccessing)
        {
            if (userAccessing == null)
            {
                throw new ArgumentNullException("user is null");
            }

            Kitchen kitchen = Context?.Kitchen.Find(kitchenId);

            if (kitchen == null)
            {
                throw new KitchenNotFoundException(kitchenId);
            }

            if (!Permissions.UserHasRightsToKitchen(userAccessing, kitchen))
            {
                throw new PermissionsException();
            }

            return Context.KitchenUser
                    .Where(x => x.KitchenId == kitchen.KitchenId && (!x.HasAcceptedInvite.HasValue || !x.HasAcceptedInvite.Value))
                    .Include(x => x.User)
                    .Include(x => x.Kitchen)
                    .Select(k => k).ToList();
        }

        public List<KitchenUser> GetMyInvites(PantryPlannerUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return GetMyInvites(user.Id);
        }

        public List<KitchenUser> GetMyInvites(string userId)
        {
            if (Context.UserExists(userId) == false)
            {
                throw new UserNotFoundException();
            }

            return Context.KitchenUser.Where(u => u.UserId == userId && u.HasAcceptedInvite.Value == false)
                                      .Include(x => x.User)
                                      .Include(x => x.Kitchen)
                                      .ToList();
        }

        internal bool IsUserOwnerOfKitchen(PantryPlannerUser user, long kitchenId)
        {
            if (user == null)
            {
                return false;
            }

            if (!Context.KitchenExists(kitchenId))
            {
                return false;
            }

            if (!Context.KitchenUserExists(kitchenId, user.Id))
            {
                return false;
            }

            return Context.GetKitchenUser(kitchenId, user.Id).IsOwner;
        }


        #endregion


        #region Invite & Update Methods

        public bool InviteUserToKitchenByUsername(string usernameToInvite, Kitchen kitchenToJoin, PantryPlannerUser userEditing)
        {
            if (kitchenToJoin == null)
            {
                throw new ArgumentNullException("kitchen is null");
            }

            return InviteUserToKitchenByUsername(usernameToInvite, kitchenToJoin.KitchenId, userEditing);
        }

        public bool InviteUserToKitchenByUsername(string username, long kitchenId, PantryPlannerUser userEditing)
        {
            if (String.IsNullOrEmpty(username))
            {
                throw new ArgumentNullException("username is required");
            }

            if (userEditing == null)
            {
                throw new ArgumentNullException("user is null");
            }

            // validate user exists based on username
            if (Context.UserExists(username) == false)
            {
                throw new UserNotFoundException(username);
            }

            var userToInvite = Context.Users.Where(u => u.UserName == username).FirstOrDefault();

            return InviteUserToKitchenByUserId(userToInvite.Id, kitchenId, userEditing);
        }

        private bool InviteUserToKitchenByUserId(string userId, long kitchenId, PantryPlannerUser userEditing)
        {
            if (String.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException("userId is empty");
            }

            if (userEditing == null)
            {
                throw new ArgumentNullException("user is null");
            }

            if (Context.KitchenExists(kitchenId) == false)
            {
                throw new KitchenNotFoundException(kitchenId);
            }

            // validate user has rights to Kitchen
            if (!Permissions.UserHasRightsToKitchen(userEditing, kitchenId))
            {
                throw new PermissionsException();
            }

            // validate user is not already apart of kitchen
            if (Context.KitchenUserExists(kitchenId, userId))
            {
                throw new InvalidOperationException("User is already in the kitchen.");
            }

            // add user to KitchenUser
            Context.KitchenUser.Add(new KitchenUser
            {
                KitchenId = kitchenId,
                UserId = userId,
                HasAcceptedInvite = false, // user has not accepted invite 
                IsOwner = false,
                DateAdded = DateTime.Now
            });

            Context.SaveChanges();

            return true;
        }

        public bool AcceptInviteToKitchen(Kitchen kitchenToJoin, PantryPlannerUser userAccepting)
        {
            if (kitchenToJoin == null)
            {
                throw new ArgumentNullException("kitchen is null");
            }

            return AcceptInviteToKitchenByKitchenId(kitchenToJoin.KitchenId, userAccepting);
        }

        public bool AcceptInviteToKitchenByKitchenId(long kitchenId, PantryPlannerUser userAccepting)
        {
            if (userAccepting == null)
            {
                throw new ArgumentNullException("user is null");
            }

            if (Context.KitchenExists(kitchenId) == false)
            {
                throw new KitchenNotFoundException(kitchenId);
            }

            KitchenUser kitchenToAccept = Context.KitchenUser.Where(k => k.KitchenId == kitchenId && k.UserId == userAccepting.Id && k.HasAcceptedInvite.Value == false).FirstOrDefault();

            if (kitchenToAccept == null)
            {
                throw new InviteNotFoundException(kitchenId);
            }

            kitchenToAccept.HasAcceptedInvite = true;
            Context.Entry(kitchenToAccept).State = EntityState.Modified;
            Context.SaveChanges();
            return true;
        }

        public bool DenyInviteToKitchen(Kitchen kitchen, PantryPlannerUser user)
        {
            if (kitchen == null)
            {
                throw new ArgumentNullException(nameof(kitchen));
            }

            return DenyInviteToKitchen(kitchen.KitchenId, user);
        }

        public bool DenyInviteToKitchen(long kitchenId, PantryPlannerUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (Context.KitchenExists(kitchenId) == false)
            {
                throw new KitchenNotFoundException(kitchenId);
            }

            if (Context.UserExists(user.Id) == false)
            {
                throw new UserNotFoundException(user.UserName);
            }

            KitchenUser inviteToDeny = user.KitchenUser.Where(k => k.KitchenId == kitchenId && k.UserId == user.Id && k.HasAcceptedInvite.Value == false).FirstOrDefault();

            if (inviteToDeny == null)
            {
                throw new InviteNotFoundException("No invite found for kitchen.");
            }

            Context.KitchenUser.Remove(inviteToDeny);
            Context.SaveChangesAsync();

            return true;
        }

        private bool UpdateKitchenUser(KitchenUser kitchenUser, PantryPlannerUser userEditing)
        {
            if (kitchenUser == null)
            {
                throw new ArgumentNullException(nameof(kitchenUser));
            }

            if (!Permissions.UserHasRightsToKitchen(userEditing, kitchenUser.KitchenId))
            {
                throw new PermissionsException();
            }

            Context.Entry(kitchenUser).State = EntityState.Modified;
            Context.SaveChanges();
            return true;
        }

        #endregion


        #region Delete Methods

        public KitchenUser DeleteKitchenUserFromKitchenByUsername(long kitchenId, string usernameToDelete, PantryPlannerUser userDeleting)
        {
            if (Context.UserExists(usernameToDelete) == false)
            {
                throw new UserNotFoundException(usernameToDelete);
            }

            PantryPlannerUser userToDelete = Context.Users.Where(u => u.UserName == usernameToDelete).FirstOrDefault();

            return DeleteKitchenUserFromKitchen(kitchenId, userToDelete.Id, userDeleting);
        }

        public KitchenUser DeleteKitchenUserFromKitchenByUsername(Kitchen kitchen, string usernameToDelete, PantryPlannerUser userDeleting)
        {
            if (Context.UserExists(usernameToDelete) == false)
            {
                throw new UserNotFoundException(usernameToDelete);
            }

            PantryPlannerUser userToDelete = Context.Users.Where(u => u.UserName == usernameToDelete).FirstOrDefault();

            return DeleteKitchenUserFromKitchen(kitchen.KitchenId, userToDelete.Id, userDeleting);
        }

        public KitchenUser DeleteKitchenUserFromKitchen(Kitchen kitchen, PantryPlannerUser userToDelete, PantryPlannerUser userDeleting)
        {
            if (kitchen == null)
            {
                throw new ArgumentNullException(nameof(kitchen));
            }

            if (userToDelete == null)
            {
                throw new ArgumentNullException(nameof(userToDelete));
            }

            return DeleteKitchenUserFromKitchen(kitchen.KitchenId, userToDelete.Id, userDeleting);
        }

        public KitchenUser DeleteKitchenUserFromKitchen(long kitchenId, string userId, PantryPlannerUser userDeleting)
        {
            if (Context.KitchenExists(kitchenId) == false)
            {
                throw new KitchenNotFoundException(kitchenId);
            }

            if (Context.UserExists(userId) == false)
            {
                throw new UserNotFoundException(userId);
            }

            KitchenUser userToDelete = Context.KitchenUser.Where(k => k.KitchenId == kitchenId && k.UserId == userId).FirstOrDefault();

            if (userToDelete == null)
            {
                throw new KitchenUserNotFoundException();
            }

            return OwnerDeleteKitchenUserByKitchenUserId(userToDelete.KitchenUserId, userDeleting);
        }

        public KitchenUser OwnerDeleteKitchenUserByKitchenUserId(long kitchenUserId, PantryPlannerUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var kitchenUser = Context.KitchenUser.Where(k => k.KitchenUserId == kitchenUserId)
                                                 .Include(k => k.User)
                                                 .FirstOrDefault();
            if (kitchenUser == null)
            {
                throw new KitchenUserNotFoundException();
            }

            // validate user has rights and owns the kitchen
            if (!Permissions.UserOwnsKitchen(user, kitchenUser.KitchenId))
            {
                throw new PermissionsException($"you must be kitchen owner to remove the user '{kitchenUser.User.UserName}'");
            }


            // validate user is not removing themselves since they own the kitchen 
            if (kitchenUser.UserId == user.Id)
            {
                throw new InvalidOperationException("You can not remove yourself from the Kitchen because you own it. Transfer ownership or delete the kitchen instead.");
            }


            Context.KitchenUser.Remove(kitchenUser);
            Context.SaveChanges();

            return kitchenUser;
        }

        public KitchenUser DeleteMyselfFromKitchen(long kitchenId, PantryPlannerUser user)
        {
            if (Context.KitchenExists(kitchenId) == false)
            {
                throw new KitchenNotFoundException(kitchenId);
            }

            Kitchen kitchen = Context.Kitchen.Find(kitchenId);
            return DeleteMyselfFromKitchen(kitchen, user);
        }

        public KitchenUser DeleteMyselfFromKitchen(Kitchen kitchen, PantryPlannerUser user)
        {
            if (kitchen == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (Context.KitchenUserExists(kitchen.KitchenId, user.Id) == false)
            {
                throw new KitchenUserNotFoundException();
            }

            KitchenUser kitchenUser = Context.GetKitchenUser(kitchen.KitchenId, user.Id);

            return DeleteMyselfFromKitchenByKitchenUserId(kitchenUser.KitchenUserId, user);
        }

        public KitchenUser DeleteMyselfFromKitchenByKitchenUserId(long kitchenUserId, PantryPlannerUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var kitchenUser = Context.KitchenUser.Find(kitchenUserId);
            if (kitchenUser == null)
            {
                throw new KitchenUserNotFoundException();
            }

            // validate user is removing themselves
            if (kitchenUser.UserId != user.Id)
            {
                throw new InvalidOperationException("You can not remove other users with this method.");
            }

            // validate users still in kitchen after delete (to avoid orphaning kitchens)
            var usersBeforeDelete = Context.KitchenUser.Where(ku => ku.KitchenId == kitchenUser.KitchenId).ToList();
            if (usersBeforeDelete.Count == 1)
            {
                throw new InvalidOperationException("You can not remove yourself because there are no other users in the kitchen.");
            }


            Context.KitchenUser.Remove(kitchenUser);
            Context.SaveChanges();

            return kitchenUser;
        }

        #endregion
    }
}
