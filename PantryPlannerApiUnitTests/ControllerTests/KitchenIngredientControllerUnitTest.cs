using System;
using Xunit;
using PantryPlanner.Models;
using Microsoft.AspNetCore.Mvc;
using PantryPlanner.Services;
using PantryPlannerApiUnitTests.Helpers;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Http;
using PantryPlanner.DTOs;
using PantryPlanner.Controllers;
using System.Collections.Generic;

namespace PantryPlannerApiUnitTests
{
    public class KitchenIngredientControllerUnitTest
    {
        PantryPlannerContext _context;
        KitchenIngredientController _controller;
        FakeUserManager _userManager;

        public KitchenIngredientControllerUnitTest()
        {
            _userManager = new FakeUserManager();
            _context = InMemoryDataGenerator.CreateAndInitializeInMemoryDatabaseContext(Guid.NewGuid().ToString(), _userManager.TestUser, insertIngredientData: true);
            _controller = new KitchenIngredientController(_context, _userManager);
        }

        #region Get Test Methods

        [Fact]
        public async Task GetIngredientsForKitchen_ValidKitchen_ReturnsOkAndCorrectResult()
        {
            Kitchen kitchen = _userManager.TestUser.KitchenUser.FirstOrDefault()?.Kitchen;

            if (kitchen == null)
            {
                throw new Exception("kitchen is not setup for testing");
            }

            var expectedIngredientsInKitchen = _context.KitchenIngredient.Where(ki => ki.KitchenId == kitchen.KitchenId).ToList();

            ActionResult<List<KitchenIngredientDto>> actualResult = await _controller.GetIngredientsForKitchen(kitchen.KitchenId);


            Assert.IsType<OkObjectResult>(actualResult.Result);

            var actualIngredientsInKitchen = (actualResult.Result as OkObjectResult).Value;
            Assert.IsType<List<KitchenIngredientDto>>(actualIngredientsInKitchen);


            Assert.Equal(expectedIngredientsInKitchen.Count, (actualIngredientsInKitchen as List<KitchenIngredientDto>).Count);
        }

        [Fact]
        public async Task GetIngredientsForKitchen_InvalidKitchen_ReturnsNotFound()
        {
            ActionResult<List<KitchenIngredientDto>> actualResult = await _controller.GetIngredientsForKitchen(-5);

            Assert.IsType<NotFoundObjectResult>(actualResult.Result);
        }


        [Fact]
        public async Task GetIngredientsForKitchen_NoRightsToKitchen_ReturnsUnauthorized()
        {
            List<long> myKitchenIds = _userManager.TestUser.KitchenUser.Select(k => k.KitchenId).ToList();
            Kitchen notMyKitchen = _context.Kitchen.Where(k => myKitchenIds.Contains(k.KitchenId) == false).FirstOrDefault();

            if (notMyKitchen == null)
            {
                throw new Exception("kitchen is not setup for testing");
            }

            ActionResult<List<KitchenIngredientDto>> actualResult = await _controller.GetIngredientsForKitchen(notMyKitchen.KitchenId);

            Assert.IsType<UnauthorizedObjectResult>(actualResult.Result);
        }

        [Fact]
        public async Task GetKitchenIngredient_ValidKitchenIngredient_ReturnsOkAndCorrectResult()
        {
            Kitchen kitchen = _userManager.TestUser.KitchenUser.FirstOrDefault()?.Kitchen;
            KitchenIngredient expectedIngredient = kitchen.KitchenIngredient.FirstOrDefault();

            if (kitchen == null || expectedIngredient == null)
            {
                throw new Exception("kitchen is not setup for testing");
            }

            ActionResult<KitchenIngredientDto> actualResult = await _controller.GetKitchenIngredient(expectedIngredient.KitchenIngredientId);


            Assert.IsType<OkObjectResult>(actualResult.Result);

            var actualKitchenIngredient = (actualResult.Result as OkObjectResult).Value;
            Assert.IsType<KitchenIngredientDto>(actualKitchenIngredient);

            Xunit.Asserts.Compare.DeepAssert.Equals(new KitchenIngredientDto(expectedIngredient), (actualKitchenIngredient as KitchenIngredientDto));
        }

        [Fact]
        public async Task GetKitchenIngredient_InvalidKitchenIngredient_ReturnsNotFound()
        {
            ActionResult<KitchenIngredientDto> actualResult = await _controller.GetKitchenIngredient(-5);

            Assert.IsType<NotFoundObjectResult>(actualResult.Result);
        }


        [Fact]
        public async Task GetKitchenIngredient_NoRightsToKitchenIngredient_ReturnsUnauthorized()
        {
            List<long> myKitchenIds = _userManager.TestUser.KitchenUser.Select(k => k.KitchenId).ToList();
            Kitchen notMyKitchen = _context.Kitchen.Where(k => myKitchenIds.Contains(k.KitchenId) == false).FirstOrDefault();
            KitchenIngredient notMyIngredient = notMyKitchen.KitchenIngredient.FirstOrDefault(); 


            if (notMyKitchen == null || notMyIngredient == null)
            {
                throw new Exception("kitchen or ingredient is not setup for testing");
            }

            ActionResult<KitchenIngredientDto> actualResult = await _controller.GetKitchenIngredient(notMyIngredient.KitchenIngredientId);

            Assert.IsType<UnauthorizedObjectResult>(actualResult.Result);
        }

        #endregion


        #region Add Test Method

        [Fact]
        public async Task AddKitchenIngredient_ValidKitchenIngredient_ReturnsOkAndNewKitchenIngredient()
        {
            KitchenUser kitchenUser = _userManager.TestUser.KitchenUser.FirstOrDefault();
            Ingredient newIngredient = InMemoryDataGenerator.AddNewRandomIngredient(_context, _userManager.TestUser);

            if (kitchenUser == null || newIngredient == null)
            {
                throw new Exception("not setup for testing");
            }

            KitchenIngredientDto kitchenIngredient = new KitchenIngredientDto()
            {
                KitchenId = kitchenUser.Kitchen.KitchenId,
                IngredientId = newIngredient.IngredientId,
                AddedByKitchenUserId = kitchenUser.KitchenUserId
            };

            ActionResult<KitchenIngredientDto> actualResult = await _controller.AddKitchenIngredientAsync(kitchenIngredient);
            Assert.IsType<OkObjectResult>(actualResult.Result);

            var actualIngredientsInKitchen = (actualResult.Result as OkObjectResult).Value;
            Assert.IsType<KitchenIngredientDto>(actualIngredientsInKitchen);


        }

        [Fact]
        public async Task AddKitchenIngredient_DuplicateKitchenIngredient_Returns405Result()
        {
            KitchenUser kitchenUser = _userManager.TestUser.KitchenUser.FirstOrDefault();
            Ingredient newIngredient = InMemoryDataGenerator.AddNewRandomIngredient(_context, _userManager.TestUser);

            if (kitchenUser == null || newIngredient == null)
            {
                throw new Exception("not setup for testing");
            }

            KitchenIngredientDto kitchenIngredient = new KitchenIngredientDto()
            {
                KitchenId = kitchenUser.Kitchen.KitchenId,
                IngredientId = newIngredient.IngredientId,
                AddedByKitchenUserId = kitchenUser.KitchenUserId
            };

            // add it once
            await _controller.AddKitchenIngredientAsync(kitchenIngredient);

            // add it again
            ActionResult<KitchenIngredientDto> actualResult = await _controller.AddKitchenIngredientAsync(kitchenIngredient);


            Assert.IsType<ObjectResult>(actualResult.Result);
            Assert.Equal(StatusCodes.Status405MethodNotAllowed, (actualResult.Result as ObjectResult).StatusCode);
        }

        [Fact]
        public async Task AddIngredientToKitchen_ValidKitchenIngredient_ReturnsOkAndNewKitchenIngredient()
        {
            KitchenUser kitchenUser = _userManager.TestUser.KitchenUser.FirstOrDefault();
            Ingredient newIngredient = InMemoryDataGenerator.AddNewRandomIngredient(_context, _userManager.TestUser);

            if (kitchenUser == null || newIngredient == null)
            {
                throw new Exception("not setup for testing");
            }

            ActionResult<KitchenIngredientDto> actualResult = await _controller.AddIngredientToKitchenAsync(kitchenUser.KitchenId, newIngredient.IngredientId);


            Assert.IsType<OkObjectResult>(actualResult.Result);

            var actualIngredientsInKitchen = (actualResult.Result as OkObjectResult).Value;
            Assert.IsType<KitchenIngredientDto>(actualIngredientsInKitchen);


        }

        [Fact]
        public async Task AddIngredientToKitchen_KitchenNotExists_ReturnsNotFound()
        {
            Ingredient newIngredient = InMemoryDataGenerator.AddNewRandomIngredient(_context, _userManager.TestUser);

            if (newIngredient == null)
            {
                throw new Exception("not setup for testing");
            }

            ActionResult<KitchenIngredientDto> actualResult = await _controller.AddIngredientToKitchenAsync(-5, newIngredient.IngredientId);
            Assert.IsType<NotFoundObjectResult>(actualResult.Result);
        }

        [Fact]
        public async Task AddIngredientToKitchen_IngredientNotExists_ReturnsNotFound()
        {
            KitchenUser kitchenUser = _userManager.TestUser.KitchenUser.FirstOrDefault();

            if (kitchenUser == null)
            {
                throw new Exception("not setup for testing");
            }

            ActionResult<KitchenIngredientDto> actualResult = await _controller.AddIngredientToKitchenAsync(kitchenUser.KitchenId, -5);
            Assert.IsType<NotFoundObjectResult>(actualResult.Result);
        }

        #endregion


        #region Delete Test Methods

        [Fact]
        public async Task DeleteKitchenIngredient_ValidKitchenIngredient_ReturnsOkAndDeletedKitchenIngredient()
        {
            KitchenUser kitchenUser = _userManager.TestUser.KitchenUser.FirstOrDefault();
            KitchenIngredient ingredientToDelete = kitchenUser?.Kitchen.KitchenIngredient.FirstOrDefault();

            if (ingredientToDelete == null)
            {
                throw new Exception("not setup for testing");
            }


            var deleteResult = await _controller.DeleteKitchenIngredientAsync(ingredientToDelete.KitchenIngredientId);
            Assert.IsType<OkObjectResult>(deleteResult.Result);

            var actualDeletedIngredient = (deleteResult.Result as OkObjectResult).Value;

            Assert.IsType<KitchenIngredientDto>(actualDeletedIngredient);
            Xunit.Asserts.Compare.DeepAssert.Equals(new KitchenIngredientDto(ingredientToDelete), (actualDeletedIngredient as KitchenIngredientDto));
        }


        [Fact]
        public async Task DeleteKitchenIngredient_KitchenIngredientNotExists_ReturnsNotFound()
        {
            var deleteResult = await _controller.DeleteKitchenIngredientAsync(-4);
            Assert.IsType<NotFoundObjectResult>(deleteResult.Result);
        }

        [Fact]
        public async Task DeleteKitchenIngredient_NoRightsToKitchenIngredient_ReturnsUnauthorized()
        {
            List<long> myKitchenIds = _userManager.TestUser.KitchenUser.Select(k => k.KitchenId).ToList();
            Kitchen notMyKitchen = _context.Kitchen.Where(k => myKitchenIds.Contains(k.KitchenId) == false).FirstOrDefault();
            KitchenIngredient ingredientToDelete = notMyKitchen.KitchenIngredient.FirstOrDefault();

            if (ingredientToDelete == null)
            {
                throw new Exception("not setup for testing");
            }


            var deleteResult = await _controller.DeleteKitchenIngredientAsync(ingredientToDelete.KitchenIngredientId);
            Assert.IsType<UnauthorizedObjectResult>(deleteResult.Result);
        }

        #endregion


        #region Update Test Methods

        [Fact]
        public async Task UpdateKitchenIngredientAsync_ValidKitchenIngredient_ReturnsOkAndKitchenIngredientUpdated()
        {
            KitchenUser kitchenUser = _userManager.TestUser.KitchenUser.FirstOrDefault();
            KitchenIngredient ingredientToUpdate = kitchenUser?.Kitchen.KitchenIngredient.FirstOrDefault();

            if (ingredientToUpdate == null)
            {
                throw new Exception("not setup for testing");
            }

            KitchenIngredientDto updateDto = new KitchenIngredientDto(ingredientToUpdate);

            string expectedNote = "froopyland is real";
            int expectedQty = 5;

            updateDto.Note = expectedNote;
            updateDto.Quantity = expectedQty;
            DateTime timeBeforeUpdate = DateTime.UtcNow;


            var updateResult = await _controller.UpdateKitchenIngredientAsync(updateDto.KitchenIngredientId, updateDto);

            KitchenIngredient updatedIngredient = _context.KitchenIngredient.Where(ki => ki.KitchenIngredientId == updateDto.KitchenIngredientId).FirstOrDefault();

            Assert.IsType<OkResult>(updateResult);
            Assert.Equal(expectedNote, updatedIngredient.Note);
            Assert.Equal(expectedQty, updatedIngredient.Quantity);
            Assert.True(updatedIngredient.LastUpdated >= timeBeforeUpdate);
        }

        [Fact]
        public async Task UpdateKitchenIngredientAsync_ValidKitchenIngredient_ReturnsOkAndOnlyUpdatesQty()
        {
            KitchenUser kitchenUser = _userManager.TestUser.KitchenUser.FirstOrDefault();
            KitchenIngredient ingredientToUpdate = kitchenUser?.Kitchen.KitchenIngredient.FirstOrDefault();

            if (ingredientToUpdate == null)
            {
                throw new Exception("not setup for testing");
            }

            KitchenIngredientDto updateDto = new KitchenIngredientDto(ingredientToUpdate);

            string noteBeforeUpdate = ingredientToUpdate.Note;
            int expectedQty = 5;

            updateDto.Note = null;
            updateDto.Quantity = expectedQty;
            DateTime timeBeforeUpdate = DateTime.UtcNow;


            var updateResult = await _controller.UpdateKitchenIngredientAsync(updateDto.KitchenIngredientId, updateDto);

            KitchenIngredient updatedIngredient = _context.KitchenIngredient.Where(ki => ki.KitchenIngredientId == updateDto.KitchenIngredientId).FirstOrDefault();

            Assert.IsType<OkResult>(updateResult);
            Assert.Equal(noteBeforeUpdate, updatedIngredient.Note);
            Assert.Equal(expectedQty, updatedIngredient.Quantity);
            Assert.True(updatedIngredient.LastUpdated >= timeBeforeUpdate);
        }

        [Fact]
        public async Task UpdateKitchenIngredientAsync_KitchenIngredientNotExists_ReturnsNotFound()
        {
            KitchenUser kitchenUser = _userManager.TestUser.KitchenUser.FirstOrDefault();
            KitchenIngredient ingredientToUpdate = kitchenUser?.Kitchen.KitchenIngredient.FirstOrDefault();

            if (ingredientToUpdate == null)
            {
                throw new Exception("not setup for testing");
            }

            KitchenIngredientDto updateDto = new KitchenIngredientDto(ingredientToUpdate);
            updateDto.KitchenIngredientId = -5;

            var updateResult = await _controller.UpdateKitchenIngredientAsync(updateDto.KitchenIngredientId, updateDto);
            Assert.IsType<NotFoundObjectResult>(updateResult);
        }

        [Fact]
        public async Task UpdateKitchenIngredientAsync_UserNoRightsToKitchenIngredient_ReturnsUnauthorized()
        {
            List<long> myKitchenIds = _userManager.TestUser.KitchenUser.Select(k => k.KitchenId).ToList();
            Kitchen notMyKitchen = _context.Kitchen.Where(k => myKitchenIds.Contains(k.KitchenId) == false).FirstOrDefault();
            KitchenIngredient ingredientToUpdate = notMyKitchen.KitchenIngredient.FirstOrDefault();

            if (ingredientToUpdate == null)
            {
                throw new Exception("not setup for testing");
            }

            KitchenIngredientDto updateDto = new KitchenIngredientDto(ingredientToUpdate);

            updateDto.Note = "froopyland is real";
            updateDto.Quantity = 3;

            var updateResult = await _controller.UpdateKitchenIngredientAsync(updateDto.KitchenIngredientId, updateDto);
            Assert.IsType<UnauthorizedObjectResult>(updateResult);
        }

        #endregion
    }
}
