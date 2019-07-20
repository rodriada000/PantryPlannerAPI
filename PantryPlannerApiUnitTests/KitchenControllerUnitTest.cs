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
using PantryPlanner.DTOs;

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
            _context = InMemoryDataGenerator.CreateAndInitializeInMemoryDatabaseContext(Guid.NewGuid().ToString(), _userManager.TestUser, insertIngredientData: false);
            _controller = new PantryPlanner.Controllers.KitchenController(_context, _userManager);
        }

        [Fact]
        public async Task Post_ValidKitchen_ReturnsNewKitchenDtoAsync()
        {
            Kitchen kitchen = new Kitchen
            {
                Name = "Bobs Burgers II",
                Description = "Delicious burgers. again",
            };

            ActionResult<KitchenDto> result = await _controller.AddNewKitchen(kitchen);
            Assert.IsType<KitchenDto>(result.Value);

            Assert.Equal(kitchen.Name, result.Value.Name);
            Assert.Equal(kitchen.Description, result.Value.Description);

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

            ActionResult<KitchenDto> result = await _controller.AddNewKitchen(kitchen);

            Assert.Equal(_userManager.TestUser.Id, result.Value.CreatedByUserId);
            Assert.NotEqual(guidBefore, result.Value.UniquePublicGuid);
            Assert.NotEqual(dateBefore, result.Value.DateCreated);
        }

        [Fact]
        public async Task Delete_ValidKitchen_ReturnsKitchenDeletedAsync()
        {
            Kitchen kitchenToDelete = _userManager.TestUser.KitchenUser.Where(k => k.IsOwner == true).FirstOrDefault()?.Kitchen;

            if (kitchenToDelete == null)
            {
                throw new Exception("kitchen is not setup for testing");
            }

            ActionResult<KitchenDto> result = await _controller.DeleteKitchenAsync(kitchenToDelete.KitchenId);

            Assert.Equal(kitchenToDelete.KitchenId, result.Value.KitchenId);
        }

        [Fact]
        public async Task Delete_UnknownKitchen_ReturnsNotFoundAsync()
        {
            ActionResult<KitchenDto> result = await _controller.DeleteKitchenAsync(-5);

            Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal(StatusCodes.Status404NotFound, (result.Result as NotFoundObjectResult).StatusCode);
        }
    }
}
