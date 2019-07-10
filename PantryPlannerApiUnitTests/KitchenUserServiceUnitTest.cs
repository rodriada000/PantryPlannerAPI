using Microsoft.EntityFrameworkCore;
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
    public class KitchenUserServiceUnitTest
    {
        PantryPlannerContext _context;
        KitchenUserService _kitchenUserService;
        PantryPlannerUser _testUser;

        public KitchenUserServiceUnitTest()
        {
            _testUser = InMemoryDataGenerator.TestUser;
            _context = InMemoryDataGenerator.CreateAndInitializeInMemoryDatabaseContext(Guid.NewGuid().ToString(), _testUser);

            _kitchenUserService = new KitchenUserService(_context);
        }

        #region Invite Users Test Methods


        #endregion

        #region Accept Invite Test Methods

        #endregion

        #region Get Invites Test Methods

        #endregion

        #region Get Kitchen Users Test Methods

        [Fact]
        public void GetUsersForKitchen_ReturnsCorrectResult()
        {
            var kitchen = (from k in _context.Kitchen
                          join u in _context.KitchenUser on k.KitchenId equals u.KitchenId
                          where u.UserId == _testUser.Id && u.IsOwner
                          select k).FirstOrDefault();


            if (kitchen == null)
            {
                throw new Exception("kitchen is not setup for testing");
            }

            var expectedResult = kitchen.KitchenUser.Where(x => x.HasAcceptedInvite.HasValue && x.HasAcceptedInvite.HasValue == true).ToList();

            var actualResult = _kitchenUserService.GetUsersForKitchen(kitchen, _testUser);
            Assert.Equal(expectedResult, actualResult);
        }

        #endregion

        #region Delete Test Methods

        #endregion
    }
}
