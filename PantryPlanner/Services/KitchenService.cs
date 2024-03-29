﻿using Microsoft.AspNetCore.Mvc;
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
    public class KitchenService : IPantryService
    {

        public PantryPlannerContext Context { get; set; }

        public PermissionService Permissions { get; set; }

        public KitchenService(PantryPlannerContext context)
        {
            Context = context;
            Permissions = new PermissionService(Context);
        }

        #region Get Methods

        public List<Kitchen> GetAllKitchens()
        {
            return Context?.Kitchen.ToList();
        }

        public Kitchen GetKitchenById(long id, PantryPlannerUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (Context.KitchenExists(id) == false)
            {
                throw new KitchenNotFoundException(id);
            }

            if (Context.UserExists(user.Id) == false)
            {
                throw new UserNotFoundException(user.UserName);
            }

            Kitchen kitchen = Context.Kitchen.Find(id);

            if (!Permissions.UserHasRightsToKitchen(user, kitchen))
            {
                throw new PermissionsException();
            }

            return kitchen;
        }

        public List<Kitchen> GetAllKitchensForUser(PantryPlannerUser user)
        {

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (Context.UserExists(user.Id) == false)
            {
                throw new UserNotFoundException(user.UserName);
            }

            return Context.KitchenUser
                    .Where(u => u.UserId == user.Id)
                    .Select(k => k.Kitchen)
                    .ToList();
        }

        #endregion


        #region Update Methods

        public bool UpdateKitchen(Kitchen kitchen, PantryPlannerUser user)
        {
            if (kitchen == null)
            {
                throw new ArgumentNullException(nameof(kitchen));
            }

            if (!Permissions.UserHasRightsToKitchen(user, kitchen))
            {
                throw new PermissionsException();
            }


            Context.Entry(kitchen).State = EntityState.Modified;
            Context.SaveChanges();
            return true;
        }

        public async Task<bool> UpdateKitchenAsync(Kitchen kitchen, PantryPlannerUser user)
        {
            if (kitchen == null)
            {
                throw new ArgumentNullException(nameof(kitchen));
            }

            if (!Permissions.UserHasRightsToKitchen(user, kitchen))
            {
                throw new PermissionsException();
            }

            Context.Entry(kitchen).State = EntityState.Modified;
            await Context.SaveChangesAsync();
            return true;
        }

        #endregion


        #region Add Methods

        public async Task<bool> AddKitchenAsync(long kitchenId, PantryPlannerUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var kitchen = Context.Kitchen.Find(kitchenId);
            if (kitchen == null)
            {
                throw new KitchenNotFoundException(kitchenId);
            }

            return await AddKitchenAsync(kitchen, user);
        }

        public async Task<bool> AddKitchenAsync(Kitchen kitchen, PantryPlannerUser user)
        {
            if (kitchen == null)
            {
                throw new ArgumentNullException(nameof(kitchen));
            }

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (Context.UserExists(user.Id) == false)
            {
                throw new UserNotFoundException(user.UserName);
            }

            // add the kitchen
            kitchen.CreatedByUserId = user.Id;
            kitchen.DateCreated = DateTime.Now;
            kitchen.UniquePublicGuid = Guid.NewGuid();

            Context.Kitchen.Add(kitchen);

            try
            {
                await Context.SaveChangesAsync();
            }
            catch (Exception)
            {
                if (Context.KitchenExists(kitchen.KitchenId))
                {
                    throw new Exception("Kitchen already exists");
                }
                else
                {
                    throw;
                }
            }

            // create relationship between user and new kitchen
            KitchenUser kitchenUser = new KitchenUser()
            {
                KitchenId = kitchen.KitchenId,
                UserId = user.Id,
                User = user,
                Kitchen = kitchen,
                DateAdded = DateTime.Now,
                IsOwner = true, // the user that created the kitchen is the owner
                HasAcceptedInvite = true
            };

            Context.KitchenUser.Add(kitchenUser);

            try
            {
                await Context.SaveChangesAsync();
            }
            catch (Exception)
            {
                if (Context.KitchenExists(kitchen.KitchenId))
                {
                    throw new Exception("Kitchen already exists");
                }
                else
                {
                    throw;
                }
            }


            return true;
        }

        #endregion


        #region Delete Methods

        public Kitchen DeleteKitchen(long id, PantryPlannerUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var kitchen = Context.Kitchen.Find(id);
            if (kitchen == null)
            {
                throw new KitchenNotFoundException(id);
            }

            return DeleteKitchen(kitchen, user);
        }

        public Kitchen DeleteKitchen(Kitchen kitchen, PantryPlannerUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (kitchen == null)
            {
                throw new ArgumentNullException(nameof(kitchen));
            }

            if (!Permissions.UserOwnsKitchen(user, kitchen))
            {
                throw new InvalidOperationException("You must own the kitchen to delete it.");
            }

            Context.Kitchen.Remove(kitchen);
            Context.SaveChanges();

            return kitchen;
        }

        #endregion
    }
}
