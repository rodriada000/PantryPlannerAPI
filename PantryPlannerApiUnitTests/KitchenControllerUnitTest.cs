using System;
using Xunit;
using PantryPlanner.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using PantryPlanner.Services;
using PantryPlannerApiUnitTests.Helpers;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using Moq;
using System.Threading.Tasks;

namespace PantryPlannerApiUnitTests
{
    public class KitchenControllerUnitTest
    {
        PantryPlannerContext _context;
        PantryPlanner.Controllers.KitchenController _controller;
        FakeUserManager _userManager;

        public KitchenControllerUnitTest()
        {
            var options = new DbContextOptionsBuilder<PantryPlannerContext>().UseInMemoryDatabase("PantryDB").Options;
            _context = new PantryPlannerContext(options);

            _userManager = new FakeUserManager();

            InMemoryDataGenerator.InitializeAll(_context, _userManager.TestUser);

            _controller = new PantryPlanner.Controllers.KitchenController(_context, _userManager);
        }

        [Fact]
        public void Post_ValidKitchen_ReturnsCreatedAtActionResult()
        {
            Kitchen kitchen = new Kitchen
            {
                Name = "Bobs Burgers II",
                Description = "Delicious burgers. again",
            };

            var result = _controller.PostKitchenAsync(kitchen);
            Assert.IsType<CreatedAtActionResult>(result.Result.Result);
        }

        [Fact]
        public void Post_ValidKitchen_ReturnsKitcheninResult()
        {
            Kitchen kitchen = new Kitchen
            {
                KitchenId = 70,
                Name = "Bobs Burgers II",
                Description = "Delicious burgers. again",
            };

            var result = _controller.PostKitchenAsync(kitchen);
            var actionResult = result.Result.Result as CreatedAtActionResult;

            Assert.Equal(kitchen, (actionResult.Value as Kitchen));
        }

        [Fact]
        public void Post_ValidKitchen_ReturnsKitchenWithFieldsSet()
        {
            Kitchen kitchen = new Kitchen
            {
                Name = "Bobs Burgers II",
                Description = "Delicious burgers. again",
            };

            var guidBefore = kitchen.UniquePublicGuid;
            var dateBefore = kitchen.DateCreated;

            var result = _controller.PostKitchenAsync(kitchen);
            var actionResult = result.Result.Result as CreatedAtActionResult;

            Assert.Equal(_userManager.TestUser.Id, (actionResult.Value as Kitchen).CreatedByUserId);
            Assert.NotEqual(guidBefore, (actionResult.Value as Kitchen).UniquePublicGuid);
            Assert.NotEqual(dateBefore, (actionResult.Value as Kitchen).DateCreated);
        }

        [Fact]
        public async Task Delete_ValidKitchen_ReturnsKitchenDeletedAsync()
        {
            Kitchen kitchenToDelete = await _context.Kitchen.FirstOrDefaultAsync();
            var result = await _controller.DeleteKitchenAsync(kitchenToDelete.KitchenId);

            Assert.Equal(kitchenToDelete.KitchenId, (result.Value as Kitchen).KitchenId);
        }

        [Fact]
        public void Delete_UnknownKitchen_ReturnsNotFound()
        {

            var result = _controller.DeleteKitchenAsync(-5);

            Assert.IsType<NotFoundResult>(result.Result.Result);
        }
    }
}
