using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PantryPlanner.Models;
using PantryPlanner.Services;

namespace PantryPlannerApiUnitTests.Helpers
{
    /// <summary>
    /// Class to generate hardcoded data for in memory database testing
    /// </summary>
    /// <remarks> idea thanks to this article: https://exceptionnotfound.net/ef-core-inmemory-asp-net-core-store-database </remarks>
    class InMemoryDataGenerator
    {
        public static List<Kitchen> Kitchens
        {
            get
            {
                return new List<Kitchen>()
                {
                    new Kitchen
                    {
                        Name = "Bobs Burgers",
                        Description = "The best around",
                        UniquePublicGuid = Guid.NewGuid(),
                        DateCreated = DateTime.Now,
                        CreatedByUserId = "test12345"
                    },
                    new Kitchen
                    {
                        Name = "Cobra Kai Dojo",
                        Description = "kick ass kitchens",
                        UniquePublicGuid = Guid.NewGuid(),
                        DateCreated = DateTime.Now,
                    },
                    new Kitchen
                    {
                        Name = "My Kitchen",
                        Description = "generic kitchen description",
                        UniquePublicGuid = Guid.NewGuid(),
                        DateCreated = DateTime.Now,
                    }
                };
            }
        } 

        public static List<KitchenUser> KitchenUsers
        {
            get
            {
                return new List<KitchenUser>()
                {
                    new KitchenUser()
                    {
                        KitchenId = 1,
                        UserId = "test12345",
                        IsOwner = true,
                        DateAdded = DateTime.Now
                    },
                    new KitchenUser()
                    {
                        KitchenId = 2,
                        UserId = "test12345",
                        IsOwner = false,
                        DateAdded = DateTime.Now
                    }
                };
            }
        }


        public static void InitializeAll(PantryPlannerContext context, PantryPlannerUser testUser)
        {
            InitializeKitchen(context);
            InitializeKitchenUser(context, testUser);
        }

        public static void InitializeKitchen(PantryPlannerContext context)
        {
            // Look for any kitchens.
            if (context.Kitchen.Any())
            {
                return;   // Data was already seeded
            }

            context.Kitchen.AddRange(Kitchens);

            context.SaveChanges();
            return;
        }

        internal static void InitializeKitchenUser(PantryPlannerContext context, PantryPlannerUser testUser)
        {
            // Look for any kitchenusers.
            if (context.KitchenUser.Any())
            {
                return;   // Data was already seeded
            }

            // insert default kitchen user test data
            context.KitchenUser.AddRange(KitchenUsers);
            context.SaveChanges();


            // generate kitchen and KitchenUser relationship for test user passed in
            Kitchen testKitchen = new Kitchen()
            {
                Name = $"Kitchen for {testUser.UserName}",
                Description = "auto created for testing",
                CreatedByUserId = testUser.Id,
                DateCreated = DateTime.Now,
                UniquePublicGuid = Guid.NewGuid()
            };

            Kitchen notOwnedKitchen = new Kitchen()
            {
                Name = $"NOT OWNED Kitchen for {testUser.UserName}",
                Description = "auto created for testing",
                CreatedByUserId = null,
                DateCreated = DateTime.Now,
                UniquePublicGuid = Guid.NewGuid()
            };

            context.Kitchen.Add(testKitchen);
            context.Kitchen.Add(notOwnedKitchen);

            KitchenUser testKitchenUser = new KitchenUser()
            {
                KitchenId = testKitchen.KitchenId,
                UserId = testUser.Id,
                DateAdded = DateTime.Now,
                IsOwner = true
            };

            KitchenUser notOwnerKitchenUser = new KitchenUser()
            {
                KitchenId = notOwnedKitchen.KitchenId,
                UserId = testUser.Id,
                DateAdded = DateTime.Now,
                IsOwner = false
            };

            context.KitchenUser.Add(testKitchenUser);
            context.KitchenUser.Add(notOwnerKitchenUser);

            context.SaveChanges();

            return;
        }
    }

}
