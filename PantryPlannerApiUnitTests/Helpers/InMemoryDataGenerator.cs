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
        public static string TestUserID { get => "test12345"; }

        public static PantryPlannerUser TestUser
        {
            get
            {
                return new PantryPlannerUser()
                {
                    Id = TestUserID,
                    UserName = "goatTester",
                    Email = "test@test.com",
                };
            }
        }

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
                        CreatedByUserId = TestUserID
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
                    },
                    new Kitchen
                    {
                        Name = "Not Mine Kitchen",
                        Description = "some persons kitchen",
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
                        UserId = TestUserID,
                        IsOwner = true,
                        DateAdded = DateTime.Now,
                        HasAcceptedInvite = true
                    },
                    new KitchenUser()
                    {
                        KitchenId = 2,
                        UserId = TestUserID,
                        IsOwner = false,
                        DateAdded = DateTime.Now,
                        HasAcceptedInvite = true
                    }
                };
            }
        }


        /// <summary>
        /// Return a <see cref="PantryPlannerContext"/> set to use an InMemoryDatabase.
        /// Test data is hardcoded and added for testing. <see cref="TestUser"/> is used to generate test data (relationships to Kitchen, Kitchenuser, etc.)
        /// </summary>
        /// <param name="dbName"> name for In Memory Database </param>
        /// <returns> Context to use for testing</returns>
        public static PantryPlannerContext CreateAndInitializeInMemoryDatabaseContext(string dbName)
        {
            PantryPlannerContext context = CreateInMemoryDatabaseContext(dbName);
            InitializeAll(context, TestUser);
            return context;
        }

        /// <summary>
        /// Return a <see cref="PantryPlannerContext"/> set to use an InMemoryDatabase.
        /// Test data is hardcoded and added for testing.
        /// </summary>
        /// <param name="dbName"> name for In Memory Database </param>
        /// <param name="testUser"> user to use to generate test data (relationships to Kitchen, Kitchenuser, etc.)</param>
        /// <returns> Context to use for testing</returns>
        public static PantryPlannerContext CreateAndInitializeInMemoryDatabaseContext(string dbName, PantryPlannerUser testUser)
        {
            PantryPlannerContext context = CreateInMemoryDatabaseContext(dbName);
            InitializeAll(context, testUser);
            return context;
        }

        /// <summary>
        /// Return an empty <see cref="PantryPlannerContext"/> set to use an InMemoryDatabase.
        /// </summary>
        /// <param name="dbName"> name for In Memory Database </param>
        /// <returns> Context to use for testing; </returns>
        public static PantryPlannerContext CreateInMemoryDatabaseContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<PantryPlannerContext>().UseInMemoryDatabase(dbName).Options;
            return new PantryPlannerContext(options);
        }

        /// <summary>
        /// Adds test data to <paramref name="context"/> and initializes <paramref name="testUser"/> with relationships to the test data.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="testUser"></param>
        public static void InitializeAll(PantryPlannerContext context, PantryPlannerUser testUser)
        {
            InitializeUser(context, testUser);
            InitializeKitchen(context);
            InitializeKitchenUser(context, testUser);
            InitializeRandomKitchenUser(context);
        }

        private static void InitializeUser(PantryPlannerContext context, PantryPlannerUser testUser)
        {
            if (testUser == null)
            {
                testUser = TestUser;
            }

            // Look for any users with same ID as user to insert.
            if (context.Users.Any(u => u.Id == testUser.Id))
            {
                return;
            }

            context.Users.Add(testUser);
            context.SaveChanges();
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

            // generate test data. assumption is Kitchen is already populated
            bool markUserAsAccepted = true; // flag to have atleast one KitchenUser be generated with HasAcceptedInvite = true
            bool markUserAsOwner = true; // flag to have atleast one KitchenUser be generated with IsOwner = true


            foreach (Kitchen kitchen in context.Kitchen)
            {
                var r = new Random();

                bool hasAccepted = (r.Next(0, 2) == 0 || markUserAsAccepted);
                bool isOwner = (r.Next(0, 2) == 0 || markUserAsOwner);

                markUserAsAccepted = false;
                markUserAsOwner = false;

                KitchenUser user = new KitchenUser()
                {
                    KitchenId = kitchen.KitchenId,
                    UserId = TestUserID,
                    DateAdded = DateTime.Now,
                    HasAcceptedInvite = hasAccepted,
                    IsOwner = isOwner
                };

                context.KitchenUser.Add(user);
            }

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
                IsOwner = true,
                HasAcceptedInvite = true
            };

            KitchenUser notOwnerKitchenUser = new KitchenUser()
            {
                KitchenId = notOwnedKitchen.KitchenId,
                UserId = testUser.Id,
                DateAdded = DateTime.Now,
                IsOwner = false,
                HasAcceptedInvite = true
            };

            context.KitchenUser.Add(testKitchenUser);
            context.KitchenUser.Add(notOwnerKitchenUser);

            context.SaveChanges();

            return;
        }

        internal static void InitializeRandomKitchenUser(PantryPlannerContext context)
        {
            PantryPlannerUser randomUser = AddNewRandomUser(context);

            Kitchen testKitchen = new Kitchen()
            {
                Name = $"Kitchen for {randomUser.UserName}",
                Description = "auto created for testing",
                CreatedByUserId = randomUser.Id,
                DateCreated = DateTime.Now,
                UniquePublicGuid = Guid.NewGuid()
            };

            Kitchen notOwnedKitchen = new Kitchen()
            {
                Name = $"NOT OWNED Kitchen for {randomUser.UserName}",
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
                UserId = randomUser.Id,
                DateAdded = DateTime.Now,
                IsOwner = true,
                HasAcceptedInvite = true
            };

            KitchenUser notOwnerKitchenUser = new KitchenUser()
            {
                KitchenId = notOwnedKitchen.KitchenId,
                UserId = randomUser.Id,
                DateAdded = DateTime.Now,
                IsOwner = false,
                HasAcceptedInvite = false
            };

            context.KitchenUser.Add(testKitchenUser);
            context.KitchenUser.Add(notOwnerKitchenUser);

            context.SaveChanges();
        }

        internal static PantryPlannerUser AddNewRandomUser(PantryPlannerContext context)
        {
            string id = Guid.NewGuid().ToString();

            PantryPlannerUser user = new PantryPlannerUser()
            {
                Id = id,
                UserName = $"user{id.Substring(0, 8)}",
                Email = $"user{id.Substring(0, 4)}@test.com"
            };

            context.Users.Add(user);
            context.SaveChanges();

            return user;
        }

    }

}
