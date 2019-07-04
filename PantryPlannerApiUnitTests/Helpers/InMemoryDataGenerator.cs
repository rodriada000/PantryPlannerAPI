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

        public static void InitializeKitchen(PantryPlannerContext context)
        {
            // Look for any kitchens.
            if (context.Kitchen.Any())
            {
                return;   // Data was already seeded
            }

            context.Kitchen.AddRange(
                new Kitchen
                {
                    KitchenId = 1,
                    Name = "Bobs Burgers",
                    Description = "The best around",
                    UniquePublicGuid = Guid.NewGuid(),
                    DateCreated = DateTime.Now,
                },
                new Kitchen
                {
                    KitchenId = 2,
                    Name = "Cobra Kai Dojo",
                    Description = "kick ass kitchens",
                    UniquePublicGuid = Guid.NewGuid(),
                    DateCreated = DateTime.Now,
                },
                new Kitchen
                {
                    KitchenId = 3,
                    Name = "My Kitchen",
                    Description = "generic kitchen description",
                    UniquePublicGuid = Guid.NewGuid(),
                    DateCreated = DateTime.Now,
                });

            context.SaveChanges();
            return;
        }

    }

}
