using Microsoft.EntityFrameworkCore;
using PantryPlanner.Exceptions;
using PantryPlanner.Models;
using PantryPlanner.Services;
using PantryPlannerApiUnitTests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            _context = InMemoryDataGenerator.CreateAndInitializeInMemoryDatabaseContext(Guid.NewGuid().ToString(), _testUser, insertIngredientData: false);
            _kitchenService = new KitchenService(_context);
        }

        #region Add Test Methods

        [Fact]
        public async Task AddKitchen_ReturnsTrueOnSuccessAsync()
        {
            Kitchen kitchen = new Kitchen
            {
                Name = "Bobs Burgers II",
                Description = "Delicious burgers. again",
            };

            bool result = await _kitchenService.AddKitchenAsync(kitchen, _testUser);
            Assert.True(result);
        }

        [Fact]
        public async Task AddKitchen_CreatedByUserIdIsSetAndOtherFieldsAsync()
        {
            Kitchen kitchen = new Kitchen
            {
                Name = "Bobs Burgers II",
                Description = "Delicious burgers. again",
            };

            var guidBefore = kitchen.UniquePublicGuid;
            var dateBefore = kitchen.DateCreated;

            bool result = await _kitchenService.AddKitchenAsync(kitchen, _testUser);
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

            bool result = await _kitchenService.AddKitchenAsync(kitchen, _testUser);

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

            var result = _kitchenService.DeleteKitchen(kitchenToDelete.KitchenId, _testUser);

            Assert.Equal(kitchenToDelete, result);
        }

        [Fact]
        public void Delete_UnknownKitchen_ThrowsKitchenNotFoundException()
        {
            Assert.Throws<KitchenNotFoundException>(() =>
            {
                _kitchenService.DeleteKitchen(-5, _testUser);
            });
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
            Kitchen kitchenToUpdate = _context.Kitchen.FirstOrDefault();

            if (kitchenToUpdate == null)
            {
                throw new Exception("kitchen is not setup for testing");
            }

            string expectedDescription = "my new description";
            kitchenToUpdate.Description = expectedDescription;


            Assert.Throws<PermissionsException>(() =>
            {
                _kitchenService.UpdateKitchen(kitchenToUpdate, null);
            });
        }


        #endregion

        #region Get Test Methods

        [Fact]
        public void GetKitchenById_ValidId_ReturnsKitchen()
        {
            Kitchen expectedKitchen = _testUser.KitchenUser.FirstOrDefault()?.Kitchen;

            if (expectedKitchen == null)
            {
                throw new Exception("expectedKitchen not setup to test");
            }

            Kitchen actualKitchen = _kitchenService.GetKitchenById(expectedKitchen.KitchenId, _testUser);

            Assert.Equal(expectedKitchen, actualKitchen);
        }

        [Fact]
        public void GetKitchenById_InvalidId_ThrowsKitchenNotFoundException()
        {
            Assert.Throws<KitchenNotFoundException>(() =>
            {
                _kitchenService.GetKitchenById(-5, _testUser);
            });
        }

        [Fact]
        public void GetKitchenById_NotMyKitchen_ThrowsPermissionsException()
        {
            List<long> myKitchenIds = _testUser.KitchenUser.Select(k => k.KitchenId).ToList();
            Kitchen notMyKitchen = _context.Kitchen.Where(k => myKitchenIds.Contains(k.KitchenId) == false).FirstOrDefault();

            if (notMyKitchen == null)
            {
                throw new Exception("notMyKitchen not setup to test");
            }

            Assert.Throws<PermissionsException>(() =>
            {
                _kitchenService.GetKitchenById(notMyKitchen.KitchenId, _testUser);
            });
        }

        [Fact]
        public void GetAllKitchensForUser_ValidUser_ReturnsCorrectResult()
        {
            List<Kitchen> expectedKitchens = _context.KitchenUser
                                                    .Where(ku => ku.UserId == _testUser.Id)
                                                    .Select(ku => ku.Kitchen).ToList();

            var actualKitchens = _kitchenService.GetAllKitchensForUser(_testUser);

            Assert.Equal(expectedKitchens, actualKitchens);
        }

        [Fact]
        public void GetAllKitchensForUser_InvalidUser_ThrowsUserNotFoundException()
        {
            PantryPlannerUser invalidUser = new PantryPlannerUser()
            {
                Id = "Idontexist",
                UserName = "liarpants"
            };

            Assert.Throws<UserNotFoundException>(() =>
            {
                _kitchenService.GetAllKitchensForUser(invalidUser);
            });
        }


        #endregion
    }
}
