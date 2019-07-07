using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public KitchenService(PantryPlannerContext context)
        {
            Context = context;
        }

        public List<Kitchen> GetAllKitchens()
        {
            return Context?.Kitchen.ToList();
        }

        public Kitchen GetKitchenById(long id)
        {
            return Context?.Kitchen.Find(id);
        }

        internal async Task<bool> UpdateKitchenAsync(Kitchen kitchen)
        {
            Context.Entry(kitchen).State = EntityState.Modified;

            try
            {
                await Context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!KitchenExists(kitchen.KitchenId))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }

        }

        internal List<Kitchen> GetAllKitchensForUser(PantryPlannerUser user)
        {
            if (Context == null || user == null)
            {
                return null;
            }

            // get all KitchenUsers for the user first
            user.KitchenUser = Context.KitchenUser.Where(u => u.UserId == user.Id).ToList();

            // join KitchenUsers to Kitchens to get kitchens they have rights to
            return Context.Kitchen.Join(user.KitchenUser, x => x.KitchenId, x => x.KitchenId, (k,u) => k).ToList();
        }

        internal void AddKitchen(Kitchen kitchen, PantryPlannerUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("cannot create kitchen; user is null");
            }

            // add the kitchen
            kitchen.CreatedByUserId = user.Id;
            kitchen.DateCreated = DateTime.Now;
            kitchen.UniquePublicGuid = Guid.NewGuid();

            Context.Kitchen.Add(kitchen);

            // create relationship between user and new kitchen
            KitchenUser kitchenUser = new KitchenUser()
            {
                KitchenId = kitchen.KitchenId,
                UserId = user.Id,
                User = user,
                Kitchen = kitchen,
                DateAdded = DateTime.Now,
                IsOwner = true // the user that created the kitchen is the owner
            };

            Context.KitchenUser.Add(kitchenUser);

            try
            {
                Context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (KitchenExists(kitchen.KitchenId))
                {
                    throw new Exception("Kitchen already exists");
                }
                else
                {
                    throw;
                }
            }

        }

        internal Kitchen DeleteKitchenById(long id)
        {
            var kitchen = Context.Kitchen.Find(id);
            if (kitchen == null)
            {
                return null;
            }


            Context.Kitchen.Remove(kitchen);
            Context.SaveChangesAsync();

            return kitchen;
        }

        public bool KitchenExists(long id)
        {
            return Context.Kitchen.Any(e => e.KitchenId == id);
        }
    }
}
