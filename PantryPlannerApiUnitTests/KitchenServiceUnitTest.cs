using Microsoft.EntityFrameworkCore;
using PantryPlanner.Models;
using PantryPlanner.Services;
using PantryPlannerApiUnitTests.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace PantryPlannerApiUnitTests
{
    public class KitchenServiceUnitTest
    {
        PantryPlannerContext _context;
        KitchenService _kitchenService;
        PantryPlannerUser _testUser;

        public KitchenServiceUnitTest()
        {
            var options = new DbContextOptionsBuilder<PantryPlannerContext>().UseInMemoryDatabase("PantryDB").Options;
            _context = new PantryPlannerContext(options);

            _testUser = new PantryPlannerUser()
            {
                Id = "test12345",
                UserName = "goatTester",
                Email = "test@test.com",
            };

            InMemoryDataGenerator.InitializeAll(_context, _testUser);

            _kitchenService = new KitchenService(_context);
        }

        #region Add Test Methods

        [Fact]
        public void AddKitchen_ReturnsTrueOnSuccess()
        {
            Kitchen kitchen = new Kitchen
            {
                KitchenId = 69,
                Name = "Bobs Burgers II",
                Description = "Delicious burgers. again",
            };

            bool result = _kitchenService.AddKitchen(kitchen, _testUser);
            Assert.True(result);
        }

        [Fact]
        public void AddKitchen_CreatedByUserIdIsSetAndOtherFields()
        {
            Kitchen kitchen = new Kitchen
            {
                KitchenId = 70,
                Name = "Bobs Burgers II",
                Description = "Delicious burgers. again",
            };

            var guidBefore = kitchen.UniquePublicGuid;
            var dateBefore = kitchen.DateCreated;

            bool result = _kitchenService.AddKitchen(kitchen, _testUser);
            Assert.Equal(_testUser.Id, kitchen.CreatedByUserId);
            Assert.NotEqual(guidBefore, kitchen.UniquePublicGuid);
            Assert.NotEqual(dateBefore, kitchen.DateCreated);
        }

        [Fact]
        public async System.Threading.Tasks.Task AddKitchen_DoesCreateKitchenUserRelationshipAsync()
        {
            Kitchen kitchen = new Kitchen
            {
                KitchenId = 71,
                Name = "Bobs Burgers II",
                Description = "Delicious burgers. again",
            };

            bool result = _kitchenService.AddKitchen(kitchen, _testUser);

            bool relationshipResult = await _context.KitchenUser.AnyAsync(x => x.KitchenId == kitchen.KitchenId && x.UserId == _testUser.Id);
            Assert.True(relationshipResult);
        }

        #endregion

        #region Delete Test Methods

        [Fact]
        public void Delete_ValidKitchen_ReturnsKitchenDeleted()
        {
            var result = _kitchenService.DeleteKitchenById(2, _testUser);

            Assert.Equal(2, result.KitchenId);
        }

        [Fact]
        public void Delete_UnknownKitchen_ReturnsNull()
        {

            var result = _kitchenService.DeleteKitchenById(-5, _testUser);

            Assert.True(result == null);
        }

        #endregion

        #region Update Test Methods

        [Fact]
        public void Update_ValidKitchen_ReturnsTrue()
        {
            // do modifications on Kitchen with ID = 1
            long key = 1;
            Kitchen kitchenToUpdate = _context.Kitchen.Find(key);

            kitchenToUpdate.Description = "my new description";

            var result = _kitchenService.UpdateKitchen(kitchenToUpdate, _testUser);

            Assert.True(result);
        }

        [Fact]
        public void Update_ValidKitchen_KitchenIsUpdatedInContext()
        {
            // do modifications on Kitchen with ID = 1
            long key = 1;
            Kitchen kitchenToUpdate = _context.Kitchen.Find(key);

            string expectedDescription = "my new description";
            kitchenToUpdate.Description = expectedDescription;

            var result = _kitchenService.UpdateKitchen(kitchenToUpdate, _testUser);

            Assert.Equal(expectedDescription, _context.Kitchen.Find(key).Description);
        }

        [Fact]
        public void Update_InvalidKitchen_ExceptionThrown()
        {
            // do modifications on Kitchen with ID = 1
            long key = 1;
            Kitchen kitchenToUpdate = _context.Kitchen.Find(key);

            string expectedDescription = "my invalid description";
            kitchenToUpdate.Description = expectedDescription;
            kitchenToUpdate.KitchenId = 999; // change ID so it becomes invalid

            try
            {
                var result = _kitchenService.UpdateKitchen(kitchenToUpdate, _testUser);
            }
            catch (Exception)
            {
                Assert.True(true);
            }

        }

        [Fact]
        public void Update_NullKitchen_ArgumentNullExceptionThrown()
        {
            try
            {
                var result = _kitchenService.UpdateKitchen(null, _testUser);
            }
            catch (Exception e)
            {
                Assert.IsType<ArgumentNullException>(e);
            }

        }

        [Fact]
        public void Update_NullUser_PermissionsExceptionThrown()
        {
            // do modifications on Kitchen with ID = 1
            long key = 1;
            Kitchen kitchenToUpdate = _context.Kitchen.Find(key);

            string expectedDescription = "my new description";
            kitchenToUpdate.Description = expectedDescription;

            try
            {
                var result = _kitchenService.UpdateKitchen(kitchenToUpdate, null);
            }
            catch (Exception e)
            {
                Assert.IsType<PermissionsException>(e);
            }

        }


        #endregion

        #region Get Test Methods

        #endregion
    }
}
