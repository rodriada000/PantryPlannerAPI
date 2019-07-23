using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PantryPlanner.Migrations;
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
        /// <param name="insertIngredientData"> true - populates Ingredient and KitchenIngredient tables; false - skip populating Ingredient & KitchenIngredinet </param>
        /// <returns> Context to use for testing</returns>
        public static PantryPlannerContext CreateAndInitializeInMemoryDatabaseContext(string dbName, PantryPlannerUser testUser, bool insertIngredientData)
        {
            PantryPlannerContext context = CreateInMemoryDatabaseContext(dbName);

            if (insertIngredientData)
            {
                InitializeAll(context, testUser);
            }
            else
            {
                InitializeUsersAndKitchens(context, testUser);
            }

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
        public static void InitializeUsersAndKitchens(PantryPlannerContext context, PantryPlannerUser testUser)
        {
            InitializeUser(context, testUser);
            InitializeKitchen(context);
            InitializeKitchenAndKitchenUserForUser(context, testUser);
            InitializeRandomKitchenAndKitchenUser(context);
            AddNewRandomUsersToKitchens(context, numOfUsersToAddPerKitchen: 3);
        }

        /// <summary>
        /// Adds test data to <paramref name="context"/> and initializes <paramref name="testUser"/> with relationships to the test data.
        /// Inserts ingredient data from USDA food composition database and initialize kitchens with ingredients.
        /// </summary>
        public static void InitializeAll(PantryPlannerContext context, PantryPlannerUser testUser)
        {
            InitializeUsersAndKitchens(context, testUser);
            InitializeIngredientsFromUSDA(context);
            InitializeKitchenIngredients(context);
        }

        /// <summary>
        /// Adds <paramref name="testUser"/> to <paramref name="context"/>
        /// </summary>
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

        /// <summary>
        /// adds the hardcoded List of <see cref="Kitchens"/> to the <paramref name="context"/>.
        /// </summary>
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

        /// <summary>
        /// Creates two Kitchens for <paramref name="testUser"/> and adds the correct KitchenUser data for the <paramref name="testUser"/>.
        /// The <paramref name="testUser"/> will own one kitchen, and NOT own the second.
        /// </summary>
        internal static void InitializeKitchenAndKitchenUserForUser(PantryPlannerContext context, PantryPlannerUser testUser)
        {
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

        /// <summary>
        /// Adds a new <see cref="PantryPlannerUser"/> and two <see cref="Kitchen"/>s for the new user...
        /// i.e. generates <see cref="PantryPlannerUser"/>, <see cref="Kitchen"/>, and <see cref="KitchenUser"/> data
        /// </summary>
        /// <param name="context"> DbContext to add new user (and other generated data) to </param>
        internal static void InitializeRandomKitchenAndKitchenUser(PantryPlannerContext context)
        {
            PantryPlannerUser randomUser = AddNewRandomUser(context);

            InitializeKitchenAndKitchenUserForUser(context, randomUser);
        }

        /// <summary>
        /// Adds a new random user to the <paramref name="context"/>.
        /// user data is generated based off a Guid.
        /// </summary>
        /// <returns> New user added. </returns>
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

        /// <summary>
        /// Creates users and adds them to already existing kitchens.
        /// </summary>
        /// <param name="context"> create users using this DbContext and adds them Kitchens in it </param>
        /// <param name="numOfUsersToAddPerKitchen"> amount of users to add per Kitchen </param>
        internal static void AddNewRandomUsersToKitchens(PantryPlannerContext context, int numOfUsersToAddPerKitchen)
        {
            if (context.Kitchen.Count() == 0)
            {
                throw new Exception("Cannot add users because Kitchen is not populated");
            }


            Random randGenerator = new Random();

            foreach (Kitchen kitchen in context.Kitchen)
            {
                bool hasAccepted = true;  // this will guarantee that atleast one new user has accepted the Invite (i.e. HasAcceptedInvite = true)

                for (int i = 0; i < numOfUsersToAddPerKitchen; i++)
                {
                    PantryPlannerUser newUser = AddNewRandomUser(context);

                    KitchenUser newKitchenUser = new KitchenUser()
                    {
                        KitchenId = kitchen.KitchenId,
                        UserId = newUser.Id,
                        DateAdded = DateTime.Now,
                        IsOwner = false,
                        HasAcceptedInvite = hasAccepted
                    };

                    context.KitchenUser.Add(newKitchenUser);
                    context.SaveChanges();

                    hasAccepted = (randGenerator.Next(0, 2) == 0);
                }
            }
        }

        /// <summary>
        /// Inserts a subset (250 records) of Ingredient test data into <paramref name="context"/> using the USDA Food Composition ETL process.
        /// </summary>
        /// <param name="context"></param>
        internal static void InitializeIngredientsFromUSDA(PantryPlannerContext context)
        {
            // load ingredient data into in-memory database
            USDAFoodCompositionDbETL etl = new USDAFoodCompositionDbETL(FoodCompositionETLUnitTest.FoodCompositionFolderLocation);
            etl.StartEtlProcess(context, 250);

            // mark a few ingredients as created by TestUser for test purposes
            for (int i = 0; i < 5; i++)
            {
                Ingredient ingredient = context.Ingredient.Skip(i).FirstOrDefault();
                ingredient.AddedByUserId = TestUserID;
                context.Entry(ingredient).State = EntityState.Modified;
            }
            context.SaveChanges();
        }


        /// <summary>
        /// Loop over every Kitchen available and add a random amount of ingredients to each one
        /// </summary>
        internal static void InitializeKitchenIngredients(PantryPlannerContext context)
        {
            if (context.Ingredient.Count() == 0)
            {
                throw new Exception("Ingredient must be populated to initialize");
            }

            Random randomGen = new Random();

            foreach (Kitchen kitchen in context.Kitchen)
            {
                int numOfIngredientsToAdd = randomGen.Next(1, 25);
                int randomOffset = randomGen.Next(100);
                KitchenUser someUserInKitchen = kitchen.KitchenUser.FirstOrDefault();

                for (int i = 0; i < numOfIngredientsToAdd; i++)
                {
                    Ingredient ingredient = context.Ingredient.Skip(randomOffset)?.ToList()[i];

                    if (ingredient == null)
                    {
                        break;
                    }

                    KitchenIngredient kitchenIngredient = new KitchenIngredient()
                    {
                        KitchenId = kitchen.KitchenId,
                        IngredientId = ingredient.IngredientId,
                        LastUpdated = DateTime.Now,
                        AddedByKitchenUserId = someUserInKitchen?.KitchenUserId
                    };

                    context.KitchenIngredient.Add(kitchenIngredient);
                }
            }

            context.SaveChanges();
        }




    }

}
