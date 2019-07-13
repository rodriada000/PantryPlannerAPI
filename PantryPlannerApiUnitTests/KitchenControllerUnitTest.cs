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
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace PantryPlannerApiUnitTests
{
    public class KitchenControllerUnitTest
    {
        PantryPlannerContext _context;
        PantryPlanner.Controllers.KitchenController _controller;
        FakeUserManager _userManager;

        public KitchenControllerUnitTest()
        {
            _userManager = new FakeUserManager();
            _context = InMemoryDataGenerator.CreateAndInitializeInMemoryDatabaseContext(Guid.NewGuid().ToString(), _userManager.TestUser);
            _controller = new PantryPlanner.Controllers.KitchenController(_context, _userManager);
        }

        [Fact]
        public async Task Post_ValidKitchen_ReturnsCreatedAtActionResultAsync()
        {
            Kitchen kitchen = new Kitchen
            {
                Name = "Bobs Burgers II",
                Description = "Delicious burgers. again",
            };

            var result = await _controller.PostKitchenAsync(kitchen);
            Assert.IsType<CreatedAtActionResult>(result.Result);
        }

        [Fact]
        public async Task Post_ValidKitchen_ReturnsKitcheninResultAsync()
        {
            Kitchen kitchen = new Kitchen
            {
                Name = "Bobs Burgers II",
                Description = "Delicious burgers. again",
            };

            var result = await _controller.PostKitchenAsync(kitchen);
            var actionResult = result.Result as CreatedAtActionResult;

            Assert.IsType<Kitchen>(actionResult.Value);
            Assert.Equal(kitchen.Name, (actionResult.Value as Kitchen).Name);
            Assert.Equal(kitchen.Description, (actionResult.Value as Kitchen).Description);
        }

        [Fact]
        public async Task Post_ValidKitchen_ReturnsKitchenWithFieldsSetAsync()
        {
            Kitchen kitchen = new Kitchen
            {
                Name = "Bobs Burgers II",
                Description = "Delicious burgers. again",
            };

            var guidBefore = kitchen.UniquePublicGuid;
            var dateBefore = kitchen.DateCreated;

            ActionResult<Kitchen> result = await _controller.PostKitchenAsync(kitchen);
            CreatedAtActionResult actionResult = result.Result as CreatedAtActionResult;

            Assert.Equal(_userManager.TestUser.Id, (actionResult.Value as Kitchen).CreatedByUserId);
            Assert.NotEqual(guidBefore, (actionResult.Value as Kitchen).UniquePublicGuid);
            Assert.NotEqual(dateBefore, (actionResult.Value as Kitchen).DateCreated);
        }

        [Fact]
        public async Task Delete_ValidKitchen_ReturnsKitchenDeletedAsync()
        {
            Kitchen kitchenToDelete = _userManager.TestUser.KitchenUser.Where(k => k.IsOwner == true).FirstOrDefault()?.Kitchen;

            if (kitchenToDelete == null)
            {
                throw new Exception("kitchen is not setup for testing");
            }

            ActionResult<Kitchen> result = await _controller.DeleteKitchenAsync(kitchenToDelete.KitchenId);

            Assert.Equal(kitchenToDelete.KitchenId, result.Value.KitchenId);
        }

        [Fact]
        public async Task Delete_UnknownKitchen_ReturnsNotFoundAsync()
        {
            ActionResult<Kitchen> result = await _controller.DeleteKitchenAsync(-5);

            Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal(StatusCodes.Status404NotFound, (result.Result as NotFoundObjectResult).StatusCode);
        }
    }
}
