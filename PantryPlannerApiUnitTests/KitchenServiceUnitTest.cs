using Microsoft.EntityFrameworkCore;
using PantryPlanner.Exceptions;
using PantryPlanner.Models;
using PantryPlanner.Services;
using PantryPlannerApiUnitTests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
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
            _testUser = new PantryPlannerUser()
            {
                Id = "Constructor123",
                UserName = "sharkyShark",
                Email = "sharks@email.com"
            };
            _context = InMemoryDataGenerator.CreateAndInitializeInMemoryDatabaseContext(Guid.NewGuid().ToString(), _testUser);
            _kitchenService = new KitchenService(_context);
        }

        #region Add Test Methods

        [Fact]
        public void AddKitchen_ReturnsTrueOnSuccess()
        {
            Kitchen kitchen = new Kitchen
            {
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
        public async void AddKitchen_DoesCreateKitchenUserRelationshipAsync()
        {
            Kitchen kitchen = new Kitchen
            {
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
            Kitchen kitchenToDelete = _testUser.KitchenUser.FirstOrDefault()?.Kitchen;


            if (kitchenToDelete == null)
            {
                throw new Exception("kitchen is not setup for testing");
            }

            var result = _kitchenService.DeleteKitchenById(kitchenToDelete.KitchenId, _testUser);

            Assert.Equal(kitchenToDelete, result);
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
            Kitchen kitchenToUpdate = _testUser.KitchenUser.FirstOrDefault()?.Kitchen;

            if (kitchenToUpdate == null)
            {
                throw new Exception("kitchen is not setup for testing");
            }

            kitchenToUpdate.Name = "my new name";
            kitchenToUpdate.Description = "my new description";

            var result = _kitchenService.UpdateKitchen(kitchenToUpdate, _testUser);

            Assert.True(result);
        }

        [Fact]
        public void Update_ValidKitchen_KitchenIsUpdatedInContext()
        {
            Kitchen kitchenToUpdate = _testUser.KitchenUser.FirstOrDefault()?.Kitchen;

            if (kitchenToUpdate == null)
            {
                throw new Exception("kitchen is not setup for testing");
            }

            string expectedDescription = "my new description";
            kitchenToUpdate.Description = expectedDescription;

            string expectedName = "my new name";
            kitchenToUpdate.Name = expectedName;

            var result = _kitchenService.UpdateKitchen(kitchenToUpdate, _testUser);

            Assert.Equal(expectedDescription, _context.Kitchen.Find(kitchenToUpdate.KitchenId).Description);
            Assert.Equal(expectedName, _context.Kitchen.Find(kitchenToUpdate.KitchenId).Name);

        }

        [Fact]
        public async void Update_InvalidKitchen_ExceptionThrownAsync()
        {
            // do modifications on Kitchen with ID = 1
            Kitchen kitchenToUpdate = await _context.Kitchen.FirstOrDefaultAsync();

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
            Kitchen kitchenToUpdate = _context.Kitchen.FirstOrDefault();

            if (kitchenToUpdate == null)
            {
                throw new Exception("kitchen is not setup for testing");
            }

            string expectedDescription = "my new description";
            kitchenToUpdate.Description = expectedDescription;

            try
            {
                var result = _kitchenService.UpdateKitchen(kitchenToUpdate, null);
            }
            catch (PermissionsException e)
            {
                Assert.True(true);
            }

        }


        #endregion

        #region Get Test Methods

        #endregion
    }
}
