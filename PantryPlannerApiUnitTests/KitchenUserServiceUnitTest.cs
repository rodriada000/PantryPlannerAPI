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

        [Fact]
        public void InviteUserToKitchenByUsername_ValidUserName_ReturnsTrue()
        {
            // add random user to test against
            PantryPlannerUser newUser = InMemoryDataGenerator.AddNewRandomUser(_context);

            Kitchen kitchen = _testUser.KitchenUser.FirstOrDefault()?.Kitchen;

            if (kitchen == null)
            {
                throw new Exception("kitchen is not setup for testing");
            }

            bool actualResult = _kitchenUserService.InviteUserToKitchenByUsername(newUser.UserName, kitchen, _testUser);
            Assert.True(actualResult);
        }

        [Fact]
        public void InviteUserToKitchenByUsername_UserAlreadyInKitchen_ThrowsInvalidOperationException()
        {
            Kitchen kitchen = _testUser.KitchenUser.FirstOrDefault().Kitchen;

            if (kitchen == null)
            {
                throw new Exception("kitchen not setup to test");
            }

            Assert.Throws<InvalidOperationException>(() =>
            {
                _kitchenUserService.InviteUserToKitchenByUsername(_testUser.UserName, kitchen, _testUser);
            });
        }

        [Fact]
        public void InviteUserToKitchenByUsername_ValidUserName_KitchenUserIsAdded()
        {
            // add random user to test against
            PantryPlannerUser newUser = InMemoryDataGenerator.AddNewRandomUser(_context);

            Kitchen kitchen = _testUser.KitchenUser.FirstOrDefault()?.Kitchen;

            if (kitchen == null)
            {
                throw new Exception("kitchen is not setup for testing");
            }

            bool result = _kitchenUserService.InviteUserToKitchenByUsername(newUser.UserName, kitchen, _testUser);

            KitchenUser actualResult = _context.KitchenUser
                                            .Where(u => u.UserId == newUser.Id && u.KitchenId == kitchen.KitchenId && u.HasAcceptedInvite == false && u.IsOwner == false)
                                            .FirstOrDefault();

            Assert.True(actualResult != null);
        }

        [Fact]
        public void InviteUserToKitchenByUsername_InvalidUserName_ThrowsUserNotFoundException()
        {
            Kitchen kitchen = _testUser.KitchenUser.FirstOrDefault()?.Kitchen;

            if (kitchen == null)
            {
                throw new Exception("kitchen is not setup for testing");
            }

            Assert.Throws<UserNotFoundException>(() =>
            {
                _kitchenUserService.InviteUserToKitchenByUsername("bobdylan_yo", kitchen, _testUser);
            });
        }

        [Fact]
        public void InviteUserToKitchenByUsername_NoUserName_ThrowsArgumentNullException()
        {
            Kitchen kitchen = _testUser.KitchenUser.FirstOrDefault()?.Kitchen;

            if (kitchen == null)
            {
                throw new Exception("kitchen is not setup for testing");
            }

            Assert.Throws<ArgumentNullException>(() =>
            {
                _kitchenUserService.InviteUserToKitchenByUsername("", kitchen, _testUser);
            });
        }

        [Fact]
        public void InviteUserToKitchenByUsername_NullUser_ThrowsArgumentNullException()
        {
            Kitchen kitchen = _testUser.KitchenUser.FirstOrDefault()?.Kitchen;

            if (kitchen == null)
            {
                throw new Exception("kitchen is not setup for testing");
            }

            Assert.Throws<ArgumentNullException>(() =>
            {
                _kitchenUserService.InviteUserToKitchenByUsername("bobdylan_yo", kitchen, null);
            });
        }

        [Fact]
        public void InviteUserToKitchenByUsername_NullKitchen_ThrowsArgumentNullException()
        {
            // add random user to test against
            PantryPlannerUser newUser = InMemoryDataGenerator.AddNewRandomUser(_context);

            Assert.Throws<ArgumentNullException>(() =>
            {
                _kitchenUserService.InviteUserToKitchenByUsername(newUser.UserName, null, _testUser);
            });
        }

        [Fact]
        public void InviteUserToKitchenByUsername_InvalidKitchenId_ThrowsKitchenNotFoundException()
        {
            // add random user to test against
            PantryPlannerUser newUser = InMemoryDataGenerator.AddNewRandomUser(_context);

            Assert.Throws<KitchenNotFoundException>(() =>
            {
                _kitchenUserService.InviteUserToKitchenByUsername(newUser.UserName, -5, _testUser);
            });
        }


        [Fact]
        public void InviteUserToKitchenByUsername_NotInKitchen_ThrowsPermissionsException()
        {
            // add random user to test against
            PantryPlannerUser newUser = InMemoryDataGenerator.AddNewRandomUser(_context);

            List<long> myKitchenIds = _testUser.KitchenUser.Select(k => k.KitchenId).Distinct().ToList();

            Kitchen notMyKitchen = _context.Kitchen.Where(k => myKitchenIds.Contains(k.KitchenId) == false).FirstOrDefault();

            if (notMyKitchen == null)
            {
                throw new Exception("kitchen is not setup for testing");
            }

            Assert.Throws<PermissionsException>(() =>
            {
                _kitchenUserService.InviteUserToKitchenByUsername(newUser.UserName, notMyKitchen.KitchenId, _testUser);
            });
        }

        [Fact]
        public void DenyInviteToKitchen_ValidKitchenUser_ReturnsTrue()
        {
            // add random user to test against
            PantryPlannerUser newUser = InMemoryDataGenerator.AddNewRandomUser(_context);

            Kitchen kitchenToJoin = _testUser.KitchenUser.FirstOrDefault()?.Kitchen;

            if (kitchenToJoin == null)
            {
                throw new Exception("kitchen is not setup for testing");
            }

            // send invite for new user
            _kitchenUserService.InviteUserToKitchenByUsername(newUser.UserName, kitchenToJoin.KitchenId, _testUser);


            // have new user accept invite
            Assert.True(_kitchenUserService.DenyInviteToKitchen(kitchenToJoin, newUser));
        }

        [Fact]
        public void DenyInviteToKitchen_NoInviteExists_ThrowsInviteNotFoundException()
        {
            // add random user to test against
            PantryPlannerUser newUser = InMemoryDataGenerator.AddNewRandomUser(_context);

            Kitchen kitchenToJoin = _testUser.KitchenUser.FirstOrDefault().Kitchen;


            Assert.Throws<InviteNotFoundException>(() =>
            {
                _kitchenUserService.DenyInviteToKitchen(kitchenToJoin, newUser);

            });
        }

        [Fact]
        public void DenyInviteToKitchen_KitchenNotExists_ThrowsKitchenNotFoundException()
        {
            // add random user to test against
            PantryPlannerUser newUser = InMemoryDataGenerator.AddNewRandomUser(_context);

            Assert.Throws<KitchenNotFoundException>(() =>
            {
                _kitchenUserService.DenyInviteToKitchen(-5, newUser);

            });
        }

        [Fact]
        public void DenyInviteToKitchen_UserNotExists_ThrowsUserNotFoundException()
        {
            Kitchen kitchenToJoin = _testUser.KitchenUser.FirstOrDefault().Kitchen;

            Assert.Throws<UserNotFoundException>(() =>
            {
                _kitchenUserService.DenyInviteToKitchen(kitchenToJoin, new PantryPlannerUser() { Id = "iaintrealyo" });
            });
        }

        [Fact]
        public void DenyInviteToKitchen_NullChecks_ThrowsArgumentNullException()
        {
            Kitchen kitchenToJoin = _testUser.KitchenUser.FirstOrDefault().Kitchen;

            Assert.Throws<ArgumentNullException>(() =>
            {
                _kitchenUserService.DenyInviteToKitchen(null, new PantryPlannerUser() { Id = "iaintrealyo" });
            });

            Assert.Throws<ArgumentNullException>(() =>
            {
                _kitchenUserService.DenyInviteToKitchen(kitchenToJoin, null);
            });
        }



        #endregion


        #region Accept Invite Test Methods

        [Fact]
        public void AcceptInviteToKitchen_ValidKitchen_ReturnsTrue()
        {
            // add random user to test against
            PantryPlannerUser newUser = InMemoryDataGenerator.AddNewRandomUser(_context);

            Kitchen kitchenToJoin = _testUser.KitchenUser.FirstOrDefault()?.Kitchen;

            if (kitchenToJoin == null)
            {
                throw new Exception("kitchen is not setup for testing");
            }

            // send invite for new user
            _kitchenUserService.InviteUserToKitchenByUsername(newUser.UserName, kitchenToJoin.KitchenId, _testUser);


            // have new user accept invite
            _kitchenUserService.AcceptInviteToKitchen(kitchenToJoin, newUser);
        }

        [Fact]
        public void AcceptInviteToKitchen_InvalidKitchen_ThrowsKitchenNotFoundException()
        {
            // add random user to test against
            PantryPlannerUser newUser = InMemoryDataGenerator.AddNewRandomUser(_context);

            Assert.Throws<KitchenNotFoundException>(() =>
            {
                _kitchenUserService.AcceptInviteToKitchenByKitchenId(-5, newUser);
            });
        }

        [Fact]
        public void AcceptInviteToKitchen_NoKitchenInvite_ThrowsInviteNotFoundException()
        {
            // add random user to test against
            PantryPlannerUser newUser = InMemoryDataGenerator.AddNewRandomUser(_context);

            // get valid kitchen from test user
            Kitchen kitchen = _testUser.KitchenUser.FirstOrDefault()?.Kitchen;

            Assert.Throws<InviteNotFoundException>(() =>
            {
                _kitchenUserService.AcceptInviteToKitchen(kitchen, newUser);
            });
        }

        #endregion


        #region Get Test Methods

        [Fact]
        public void GetUsersThatHaveNotAcceptedInvite_ValidKitchen_ReturnsCorrectResult()
        {
            var kitchen = (from k in _context.Kitchen
                           join u in _context.KitchenUser on k.KitchenId equals u.KitchenId
                           where u.UserId == _testUser.Id && u.IsOwner
                           select k).FirstOrDefault();


            if (kitchen == null)
            {
                throw new Exception("kitchen is not setup for testing");
            }

            var expectedResult = kitchen.KitchenUser.Where(x => !x.HasAcceptedInvite.HasValue || !x.HasAcceptedInvite.Value).ToList();

            var actualResult = _kitchenUserService.GetUsersThatHaveNotAcceptedInvite(kitchen, _testUser);
            Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public void GetMyInvites_ValidUser_ReturnsCorrectResult()
        {
            // add random user to test against
            PantryPlannerUser newUser = InMemoryDataGenerator.AddNewRandomUser(_context);

            Kitchen kitchen = _testUser.KitchenUser.FirstOrDefault()?.Kitchen;

            if (kitchen == null)
            {
                throw new Exception("kitchen is not setup for testing");
            }

            bool inviteResult = _kitchenUserService.InviteUserToKitchenByUsername(newUser.UserName, kitchen, _testUser);

            List<KitchenUser> actualResult = _kitchenUserService.GetMyInvites(newUser);

            Assert.True(actualResult.Count == 1);
            Assert.True(actualResult[0].KitchenId == kitchen.KitchenId);

        }

        [Fact]
        public void GetAcceptedUsersForKitchen_ReturnsCorrectResult()
        {
            // arrange
            var kitchen = (from k in _context.Kitchen
                           join u in _context.KitchenUser on k.KitchenId equals u.KitchenId
                           where u.UserId == _testUser.Id && u.IsOwner
                           select k).FirstOrDefault();

            if (kitchen == null)
            {
                throw new Exception("kitchen is not setup for testing");
            }

            var expectedResult = kitchen.KitchenUser.Where(x => x.HasAcceptedInvite.HasValue && x.HasAcceptedInvite.Value == true).ToList();

            // act
            var actualResult = _kitchenUserService.GetAcceptedUsersForKitchen(kitchen, _testUser);

            // assert
            Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public void GetAllUsersForKitchen_ReturnsCorrectResult()
        {
            // arrange
            var kitchen = _testUser.KitchenUser.FirstOrDefault().Kitchen;

            if (kitchen == null)
            {
                throw new Exception("kitchen is not setup for testing");
            }

            List<KitchenUser> expectedResult = kitchen.KitchenUser.ToList();

            // act
            List<KitchenUser> actualResult = _kitchenUserService.GetAllUsersForKitchen(kitchen, _testUser);

            // assert
            Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public void Get_UserNotInKitchen_ThrowsPermissionsException()
        {
            // get a Kitchen the test user does not have rights to
            List<long> myKitchens = _testUser.KitchenUser.Select(ku => ku.KitchenId).ToList();
            Kitchen kitchen = _context.Kitchen.Where(k => myKitchens.Contains(k.KitchenId) == false).FirstOrDefault();

            if (kitchen == null)
            {
                throw new Exception("kitchen is not setup for testing");
            }

            Assert.Throws<PermissionsException>(() =>
            {
                _kitchenUserService.GetAllUsersForKitchen(kitchen, _testUser);
            });

            Assert.Throws<PermissionsException>(() =>
            {
                _kitchenUserService.GetAcceptedUsersForKitchen(kitchen, _testUser);
            });

            Assert.Throws<PermissionsException>(() =>
            {
                _kitchenUserService.GetUsersThatHaveNotAcceptedInvite(kitchen, _testUser);
            });
        }

        [Fact]
        public void Get_InvalidKitchen_ThrowsKitchenNotFoundException()
        {
            Kitchen kitchen = new Kitchen()
            {
                KitchenId = -999,
                Name = "invalid kitchen"
            };

            if (kitchen == null)
            {
                throw new Exception("kitchen is not setup for testing");
            }

            Assert.Throws<KitchenNotFoundException>(() =>
            {
                _kitchenUserService.GetAllUsersForKitchen(kitchen, _testUser);
            });

            Assert.Throws<KitchenNotFoundException>(() =>
            {
                _kitchenUserService.GetAcceptedUsersForKitchen(kitchen, _testUser);
            });

            Assert.Throws<KitchenNotFoundException>(() =>
            {
                _kitchenUserService.GetUsersThatHaveNotAcceptedInvite(kitchen, _testUser);
            });
        }



        #endregion


        #region Delete Test Methods

        [Fact]
        public void DeleteKitchenUserFromKitchenByUsername_DeleteMyself_ThrowsInvalidOperationException()
        {
            KitchenUser myKitchenUser = _testUser.KitchenUser.Where(u => u.IsOwner).FirstOrDefault();


            if (myKitchenUser == null)
            {
                throw new Exception("kKitchenUser is not setup for testing");
            }

            Assert.Throws<InvalidOperationException>(() =>
            {
                _kitchenUserService.DeleteKitchenUserFromKitchenByUsername(myKitchenUser.KitchenId, _testUser.UserName, _testUser);
            });
        }

        [Fact]
        public void DeleteKitchenUserFromKitchenByUsername_NotOwnerDelete_ThrowsPermissionsException()
        {
            KitchenUser notMyKitchenUser = _testUser.KitchenUser.Where(u => !u.IsOwner).FirstOrDefault();


            if (notMyKitchenUser == null)
            {
                throw new Exception("KitchenUser is not setup for testing");
            }

            Assert.Throws<PermissionsException>(() =>
            {
                _kitchenUserService.DeleteKitchenUserFromKitchenByUsername(notMyKitchenUser.Kitchen, _testUser.UserName, _testUser);
            });
        }

        [Fact]
        public void DeleteKitchenUserFromKitchenByUsername_ValidKitchenUser_ReturnsDeletedUser()
        {
            KitchenUser myKitchenUser = _testUser.KitchenUser.Where(u => u.IsOwner).FirstOrDefault();

            if (myKitchenUser == null)
            {
                throw new Exception("KitchenUser is not setup for testing");
            }

            // setup user to be in kitchen to test against
            var newUser = InMemoryDataGenerator.AddNewRandomUser(_context);

            _kitchenUserService.InviteUserToKitchenByUsername(newUser.UserName, myKitchenUser.Kitchen, _testUser);
            _kitchenUserService.AcceptInviteToKitchenByKitchenId(myKitchenUser.KitchenId, newUser);


            KitchenUser expectedUserToDelete = myKitchenUser.Kitchen.KitchenUser.Where(u => u.UserId == newUser.Id).FirstOrDefault();

            if (expectedUserToDelete == null)
            {
                throw new Exception("expectedUserToDelete is not setup for testing");
            }

            // do deletion
            KitchenUser actualDeletedUser = _kitchenUserService.DeleteKitchenUserFromKitchenByUsername(expectedUserToDelete.Kitchen, newUser.UserName, _testUser);

            Assert.Equal(expectedUserToDelete, actualDeletedUser);
        }



        [Fact]
        public void DeleteKitchenUserByKitchenUserId_InvalidID_ThrowsKitchenUserNotFound()
        {
            Assert.Throws<KitchenUserNotFoundException>(() =>
            {
                _kitchenUserService.OwnerDeleteKitchenUserByKitchenUserId(-5, _testUser);
            });
        }

        [Fact]
        public void DeleteKitchenUserByKitchenUserId_ValidKitchenUser_ReturnsDeletedUser()
        {
            KitchenUser myKitchenUser = _testUser.KitchenUser.Where(u => u.IsOwner).FirstOrDefault();

            if (myKitchenUser == null)
            {
                throw new Exception("kKitchenUser is not setup for testing");
            }

            var newUser = InMemoryDataGenerator.AddNewRandomUser(_context);

            _kitchenUserService.InviteUserToKitchenByUsername(newUser.UserName, myKitchenUser.Kitchen, _testUser);
            _kitchenUserService.AcceptInviteToKitchenByKitchenId(myKitchenUser.KitchenId, newUser);


            KitchenUser expectedUserToDelete = myKitchenUser.Kitchen.KitchenUser.Where(u => u.UserId == newUser.Id).FirstOrDefault();

            if (expectedUserToDelete == null)
            {
                throw new Exception("expectedUserToDelete is not setup for testing");
            }

            KitchenUser actualDeletedUser = _kitchenUserService.OwnerDeleteKitchenUserByKitchenUserId(expectedUserToDelete.KitchenUserId, _testUser);

            Assert.Equal(expectedUserToDelete, actualDeletedUser);
        }


        [Fact]
        public void DeleteMyselfFromKitchen_OnlyOneUserInKitchen_ThrowsInvalidOperationException()
        {
            KitchenUser expectedResult = _testUser.KitchenUser.Where(k => k.IsOwner == false).FirstOrDefault();
            int kitchenCountBefore = _testUser.KitchenUser.Count;

            Kitchen notOwnedKitchen = expectedResult?.Kitchen;

            if (notOwnedKitchen == null)
            {
                throw new Exception("kitchen not setup for testing");
            }

            Assert.Throws<InvalidOperationException>(() =>
            {
                _kitchenUserService.DeleteMyselfFromKitchen(notOwnedKitchen, _testUser);
            }); 
        }

        [Fact]
        public void DeleteMyselfFromKitchen_ValidKitchenUser_ReturnsKitchenUser()
        {
            KitchenUser expectedResult = _testUser.KitchenUser.Where(k => k.IsOwner == false).FirstOrDefault();
            int kitchenCountBefore = _testUser.KitchenUser.Count;

            Kitchen notOwnedKitchen = expectedResult?.Kitchen;

            if (notOwnedKitchen == null)
            {
                throw new Exception("kitchen not setup for testing");
            }

            // ensure a another user is in Kitchen
            var newUser = InMemoryDataGenerator.AddNewRandomUser(_context);
            _kitchenUserService.InviteUserToKitchenByUsername(newUser.UserName, notOwnedKitchen, _testUser);
            _kitchenUserService.AcceptInviteToKitchen(notOwnedKitchen, newUser);


            KitchenUser actualResult = _kitchenUserService.DeleteMyselfFromKitchen(notOwnedKitchen, _testUser);

            Assert.Equal(expectedResult, actualResult);
            Assert.Equal(kitchenCountBefore - 1, _testUser.KitchenUser.Count);
        }

        [Fact]
        public void DeleteMyselfFromKitchen_InvalidKitchenUser_ThrowsKitchenUserNotFoundException()
        {
            Assert.Throws<KitchenNotFoundException>(() =>
            {
                _kitchenUserService.DeleteMyselfFromKitchen(-5, _testUser);
            });

            Kitchen fakeKitchen = new Kitchen()
            {
                KitchenId = 99999,
                Name = "some fake kitchen"
            };

            Assert.Throws<KitchenUserNotFoundException>(() =>
            {
                _kitchenUserService.DeleteMyselfFromKitchen(fakeKitchen, _testUser);
            });
        }


        #endregion
    }
}
