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
            Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(StatusCodes.Status201Created, (result.Result as ObjectResult).StatusCode);

            var newKitchen = (result.Result as ObjectResult).Value;
            Assert.IsType<KitchenDto>(newKitchen);


            Assert.Equal(kitchen.Name, (newKitchen as KitchenDto).Name);
            Assert.Equal(kitchen.Description, (newKitchen as KitchenDto).Description);

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

            KitchenDto newKitchen = ((result.Result as ObjectResult).Value as KitchenDto);

            Assert.Equal(_userManager.TestUser.Id, newKitchen.CreatedByUserId);
            Assert.NotEqual(guidBefore, newKitchen.UniquePublicGuid);
            Assert.NotEqual(dateBefore, newKitchen.DateCreated);
        }

        [Fact]
        public async Task Delete_ValidKitchen_ReturnsOkAndKitchenDeletedAsync()
        {
            Kitchen kitchenToDelete = _userManager.TestUser.KitchenUser.Where(k => k.IsOwner == true).FirstOrDefault()?.Kitchen;

            if (kitchenToDelete == null)
            {
                throw new Exception("kitchen is not setup for testing");
            }

            ActionResult<KitchenDto> result = await _controller.DeleteKitchenAsync(kitchenToDelete.KitchenId);
            Assert.IsType<OkObjectResult>(result.Result);

            object actualKitchenDeleted = (result.Result as OkObjectResult).Value;
            Assert.IsType<KitchenDto>(actualKitchenDeleted);

            Xunit.Asserts.Compare.DeepAssert.Equals(new KitchenDto(kitchenToDelete), actualKitchenDeleted);
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
