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
    public class KitchenListService : IPantryService
    {

        public PantryPlannerContext Context { get; set; }
        private PermissionService Permissions { get; set; }
        private KitchenService KitchenService { get; set; }

        public KitchenListService(PantryPlannerContext context, KitchenService kitchenService)
        {
            Context = context;
            Permissions = new PermissionService(Context);
            KitchenService = kitchenService;
        }

        #region Get Methods

        public KitchenList GetKitchenListById(long id, PantryPlannerUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (Context.KitchenListExists(id) == false)
            {
                throw new KitchenListNotFoundException(id);
            }

            if (Context.UserExists(user.Id) == false)
            {
                throw new UserNotFoundException(user.UserName);
            }

            KitchenList kitchen = Context.KitchenList.AsNoTracking().Where(k => k.KitchenListId == id).Include(k => k.Kitchen).FirstOrDefault();

            if (!Permissions.UserHasRightsToKitchen(user, kitchen.Kitchen))
            {
                throw new PermissionsException();
            }

            return kitchen;
        }

        public async Task<List<KitchenList>> GetAllKitchenListsForUser(PantryPlannerUser user)
        {

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (Context.UserExists(user.Id) == false)
            {
                throw new UserNotFoundException(user.UserName);
            }

            var availableKitchens = KitchenService.GetAllKitchensForUser(user);
            List<long> kitchenIds = availableKitchens?.Select(k => k.KitchenId).ToList();

            return await Context.KitchenList.AsNoTracking()
                                            .Where(k => kitchenIds.Contains(k.KitchenId))
                                            .ToListAsync();
        }

        #endregion


        #region Update Methods

        //public bool UpdateKitchen(Kitchen kitchen, PantryPlannerUser user)
        //{
        //    if (kitchen == null)
        //    {
        //        throw new ArgumentNullException(nameof(kitchen));
        //    }

        //    if (!Permissions.UserHasRightsToKitchen(user, kitchen))
        //    {
        //        throw new PermissionsException();
        //    }


        //    Context.Entry(kitchen).State = EntityState.Modified;
        //    Context.SaveChanges();
        //    return true;
        //}

        //public async Task<bool> UpdateKitchenAsync(Kitchen kitchen, PantryPlannerUser user)
        //{
        //    if (kitchen == null)
        //    {
        //        throw new ArgumentNullException(nameof(kitchen));
        //    }

        //    if (!Permissions.UserHasRightsToKitchen(user, kitchen))
        //    {
        //        throw new PermissionsException();
        //    }

        //    Context.Entry(kitchen).State = EntityState.Modified;
        //    await Context.SaveChangesAsync();
        //    return true;
        //}

        #endregion


        #region Add Methods

        public async Task<KitchenList> AddKitchenListAsync(KitchenList newList, PantryPlannerUser user)
        {
            if (newList == null)
            {
                throw new ArgumentNullException(nameof(newList));
            }

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (Context.UserExists(user.Id) == false)
            {
                throw new UserNotFoundException(user.UserName);
            }

            Context.KitchenList.Add(newList);

            try
            {
                await Context.SaveChangesAsync();
            }
            catch (Exception)
            {
                if (Context.KitchenListExists(newList.KitchenListId))
                {
                    throw new Exception("Kitchen List already exists");
                }
                else
                {
                    throw;
                }
            }

            return newList;
        }

        #endregion


        #region Delete Methods

        public KitchenList DeleteKitchenList(long id, PantryPlannerUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var kitchenList = Context.KitchenList.Find(id);
            if (kitchenList == null)
            {
                throw new KitchenListNotFoundException(id);
            }

            return DeleteKitchenList(kitchenList, user);
        }

        public KitchenList DeleteKitchenList(KitchenList kitchenList, PantryPlannerUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (kitchenList == null)
            {
                throw new ArgumentNullException(nameof(kitchenList));
            }

            if (!Permissions.UserHasRightsToKitchen(user, kitchenList.KitchenId))
            {
                throw new InvalidOperationException("You must be in the kitchen to delete the list.");
            }

            Context.KitchenList.Remove(kitchenList);
            Context.SaveChanges();

            return kitchenList;
        }

        #endregion
    }
}
