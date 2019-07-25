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



        #endregion


        #region Delete Test Methods


        #endregion


        #region Update Test Methods



        #endregion
    }
}
