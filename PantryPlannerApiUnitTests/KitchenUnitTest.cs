using System;
using Xunit;
using PantryPlanner.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using PantryPlanner.Services;
using PantryPlannerApiUnitTests.Helpers;

namespace PantryPlannerApiUnitTests
{
    public class KitchenUnitTest
    {
        PantryPlanner.Services.PantryPlannerContext _context;
        PantryPlanner.Controllers.KitchenController _controller;

        public KitchenUnitTest()
        {
            var options = new DbContextOptionsBuilder<PantryPlannerContext>().UseInMemoryDatabase("PantryDB").Options;
            _context = new PantryPlanner.Services.PantryPlannerContext(options);

            InMemoryDataGenerator.InitializeKitchen(_context);

            _controller = new PantryPlanner.Controllers.KitchenController(_context);
        }

        [Fact]
        public async void Post_ValidKitchen_ReturnsCreatedAtActionResult()
        {
            Kitchen kitchen = new Kitchen
            {
                KitchenId = 69,
                Name = "Bobs Burgers II",
                Description = "Delicious burgers. again",
                DateCreated = DateTime.Now,
                UniquePublicGuid = Guid.NewGuid()
            };

            var result = await _controller.PostKitchen(kitchen);
            Assert.IsType<CreatedAtActionResult>(result.Result);
        }

        [Fact]
        public async void Post_ValidKitchen_ReturnsKitcheninResult()
        {
            Kitchen kitchen = new Kitchen
            {
                KitchenId = 69,
                Name = "Bobs Burgers II",
                Description = "Delicious burgers. again",
                DateCreated = DateTime.Now,
                UniquePublicGuid = Guid.NewGuid()
            };

            var result = await _controller.PostKitchen(kitchen);
            var returnedItem = ((result.Result as CreatedAtActionResult).Value as Kitchen);

            Assert.Equal(kitchen, returnedItem);
        }

        [Fact]
        public async void Delete_ValidKitchen_ReturnsKitchenDeleted()
        {
            var result = await _controller.DeleteKitchen(2);

            Assert.Equal(2, result.Value.KitchenId);
        }

        [Fact]
        public async void Delete_UnknownKitchen_ReturnsNotFound()
        {
            var result = await _controller.DeleteKitchen(-5);

            Assert.IsType<NotFoundResult>(result.Result);
        }
    }
}
