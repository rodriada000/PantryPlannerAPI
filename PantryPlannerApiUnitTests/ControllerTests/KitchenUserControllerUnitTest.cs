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
    public class KitchenUserControllerUnitTest
    {
        PantryPlannerContext _context;
        FakeUserManager _userManager;
        PantryPlanner.Controllers.KitchenUserController _controller;

        public KitchenUserControllerUnitTest()
        {
            _userManager = new FakeUserManager();
            _context = InMemoryDataGenerator.CreateAndInitializeInMemoryDatabaseContext(Guid.NewGuid().ToString(), _userManager.TestUser, insertIngredientData: false);
            _controller = new PantryPlanner.Controllers.KitchenUserController(_context, _userManager);
        }

        #region Get Test Methods

        [Fact]
        public async Task GetAllUsersForKitchen_ValidKitchen_ReturnsCorrectResult()
        {
            Kitchen kitchen = _userManager.TestUser.KitchenUser.FirstOrDefault()?.Kitchen;

            if (kitchen == null)
            {
                throw new Exception("kitchen is not setup for testing");
            }

            List<KitchenUserDto> expectedResult = KitchenUserDto.ToList(_context.KitchenUser.Where(k => k.KitchenId == kitchen.KitchenId).ToList());


            ActionResult<List<KitchenUserDto>> allUsersResult = await _controller.GetAllUsersForKitchen(kitchen.KitchenId);


            Assert.Equal(expectedResult.Count, allUsersResult.Value.Count);
        }

        [Fact]
        public async Task GetAllUsersForKitchen_InvalidKitchen_ReturnsNotFound()
        {
            ActionResult<List<KitchenUserDto>> allUsersResult = await _controller.GetAllUsersForKitchen(-1);

            Assert.IsType<NotFoundObjectResult>(allUsersResult.Result);
        }

        [Fact]
        public async Task GetAllUsersForKitchen_UserNotInKitchen_ReturnsUnauthorized()
        {
            List<long> kitchenIds = _userManager.TestUser.KitchenUser.Select(u => u.KitchenId).ToList();
            Kitchen kitchen = _context.KitchenUser.Where(u => kitchenIds.Contains(u.KitchenId) == false).FirstOrDefault()?.Kitchen;

            if (kitchen == null)
            {
                throw new Exception("kitchen is not setup for testing");
            }


            ActionResult<List<KitchenUserDto>> allUsersResult = await _controller.GetAllUsersForKitchen(kitchen.KitchenId);

            Assert.IsType<UnauthorizedObjectResult>(allUsersResult.Result);
        }



        [Fact]
        public async Task GetAcceptedUsersForKitchen_ValidKitchen_ReturnsCorrectResult()
        {
            Kitchen kitchen = _userManager.TestUser.KitchenUser.FirstOrDefault()?.Kitchen;

            if (kitchen == null)
            {
                throw new Exception("kitchen is not setup for testing");
            }

            List<KitchenUserDto> expectedResult = KitchenUserDto.ToList(_context.KitchenUser.Where(k => k.KitchenId == kitchen.KitchenId && k.HasAcceptedInvite.Value).ToList());


            ActionResult<List<KitchenUserDto>> allUsersResult = await _controller.GetAcceptedUsersForKitchen(kitchen.KitchenId);


            Assert.Equal(expectedResult.Count, allUsersResult.Value.Count);
        }

        [Fact]
        public async Task GetAcceptedUsersForKitchen_InvalidKitchen_ReturnsNotFound()
        {
            ActionResult<List<KitchenUserDto>> allUsersResult = await _controller.GetAcceptedUsersForKitchen(-1);

            Assert.IsType<NotFoundObjectResult>(allUsersResult.Result);
        }

        [Fact]
        public async Task GetAcceptedUsersForKitchen_UserNotInKitchen_ReturnsUnauthorized()
        {
            List<long> myKitchenIds = _userManager.TestUser.KitchenUser.Select(u => u.KitchenId).ToList();
            Kitchen notMyKitchen = _context.KitchenUser.Where(u => myKitchenIds.Contains(u.KitchenId) == false).FirstOrDefault()?.Kitchen;

            if (notMyKitchen == null)
            {
                throw new Exception("kitchen is not setup for testing");
            }


            ActionResult<List<KitchenUserDto>> allUsersResult = await _controller.GetAcceptedUsersForKitchen(notMyKitchen.KitchenId);

            Assert.IsType<UnauthorizedObjectResult>(allUsersResult.Result);
        }



        [Fact]
        public async Task GetNotAcceptedUsersForKitchen_ValidKitchen_ReturnsCorrectResult()
        {
            Kitchen kitchen = _userManager.TestUser.KitchenUser.FirstOrDefault()?.Kitchen;

            if (kitchen == null)
            {
                throw new Exception("kitchen is not setup for testing");
            }

            List<KitchenUserDto> expectedResult = KitchenUserDto.ToList(_context.KitchenUser.Where(k => k.KitchenId == kitchen.KitchenId && (!k.HasAcceptedInvite.Value || !k.HasAcceptedInvite.HasValue)).ToList());


            ActionResult<List<KitchenUserDto>> allUsersResult = await _controller.GetNotAcceptedUsersForKitchen(kitchen.KitchenId);


            Assert.Equal(expectedResult.Count, allUsersResult.Value.Count);
        }

        [Fact]
        public async Task GetNotAcceptedUsersForKitchen_InvalidKitchen_ReturnsNotFound()
        {
            ActionResult<List<KitchenUserDto>> allUsersResult = await _controller.GetNotAcceptedUsersForKitchen(-1);

            Assert.IsType<NotFoundObjectResult>(allUsersResult.Result);
        }

        [Fact]
        public async Task GetNotAcceptedUsersForKitchen_UserNotInKitchen_ReturnsUnauthorized()
        {
            List<long> kitchenIds = _userManager.TestUser.KitchenUser.Select(u => u.KitchenId).ToList();
            Kitchen kitchen = _context.KitchenUser.Where(u => kitchenIds.Contains(u.KitchenId) == false).FirstOrDefault()?.Kitchen;

            if (kitchen == null)
            {
                throw new Exception("kitchen is not setup for testing");
            }


            ActionResult<List<KitchenUserDto>> allUsersResult = await _controller.GetAcceptedUsersForKitchen(kitchen.KitchenId);

            Assert.IsType<UnauthorizedObjectResult>(allUsersResult.Result);
        }

        #endregion

        #region Invite Test Methods

        [Fact]
        public async Task InviteUserToKitchen_ValidKitchen_ReturnsOKAndInviteSent()
        {
            PantryPlannerUser userToInvite = InMemoryDataGenerator.AddNewRandomUser(_context);

            Kitchen kitchen = _userManager.TestUser.KitchenUser.FirstOrDefault()?.Kitchen;

            if (userToInvite == null || kitchen == null)
            {
                throw new Exception("user or kitchen is not setup for testing");
            }


            ActionResult inviteResult = await _controller.InviteUserToKitchen(userToInvite.UserName, kitchen.KitchenId);

            Assert.IsType<OkResult>(inviteResult);
            Assert.True(_context.KitchenUser.Any(k => k.KitchenId == kitchen.KitchenId && k.UserId == userToInvite.Id && !k.HasAcceptedInvite.Value));
        }

        [Fact]
        public async Task InviteUserToKitchen_InvalidKitchen_ReturnsNotFound()
        {
            List<string> usersInMyKitchen = _userManager.TestUser.Kitchen.FirstOrDefault().KitchenUser.Select(u => u.UserId).ToList();
            string userToInvite = _context.KitchenUser.Where(u => usersInMyKitchen.Contains(u.UserId) == false).FirstOrDefault()?.User?.UserName;

            if (userToInvite == null)
            {
                throw new Exception("user is not setup for testing");
            }


            ActionResult<bool> inviteResult = await _controller.InviteUserToKitchen(userToInvite, -5);


            Assert.IsType<NotFoundObjectResult>(inviteResult.Result);
        }

        [Fact]
        public async Task InviteUserToKitchen_InvalidUser_ReturnsNotFound()
        {
            Kitchen kitchen = _userManager.TestUser.KitchenUser.FirstOrDefault()?.Kitchen;

            if (kitchen == null)
            {
                throw new Exception("kitchen is not setup for testing");
            }


            ActionResult<bool> inviteResult = await _controller.InviteUserToKitchen("babysharktechnopop", kitchen.KitchenId);


            Assert.IsType<NotFoundObjectResult>(inviteResult.Result);
        }

        [Fact]
        public async Task InviteUserToKitchen_UserNotInKitchen_ReturnsUnauthorized()
        {
            List<long> myKitchenIds = _userManager.TestUser.KitchenUser.Select(u => u.KitchenId).ToList();
            Kitchen notMyKitchen = _context.KitchenUser.Where(u => myKitchenIds.Contains(u.KitchenId) == false).FirstOrDefault()?.Kitchen;

            List<string> usersInMyKitchen = _userManager.TestUser.Kitchen.FirstOrDefault().KitchenUser.Select(u => u.UserId).ToList();
            string userToInvite = _context.KitchenUser.Where(u => usersInMyKitchen.Contains(u.UserId) == false).FirstOrDefault()?.User?.UserName;


            if (notMyKitchen == null || userToInvite == null)
            {
                throw new Exception("kitchen or user is not setup for testing");
            }


            ActionResult<bool> inviteResult = await _controller.InviteUserToKitchen(userToInvite, notMyKitchen.KitchenId);


            Assert.IsType<UnauthorizedObjectResult>(inviteResult.Result);
        }

        [Fact]
        public async Task InviteUserToKitchen_UserAlreadyInvited_Returns405MethodNotAllowed()
        {
            Kitchen kitchen = _userManager.TestUser.KitchenUser.FirstOrDefault()?.Kitchen;

            if (kitchen == null)
            {
                throw new Exception("kitchen is not setup for testing");
            }


            ActionResult<bool> inviteResult = await _controller.InviteUserToKitchen(_userManager.TestUser.UserName, kitchen.KitchenId);


            Assert.IsType<ObjectResult>(inviteResult.Result);
            Assert.Equal(StatusCodes.Status405MethodNotAllowed, (inviteResult.Result as ObjectResult).StatusCode);

        }


        [Fact]
        public async Task AcceptKitchenInvite_ValidKitchen_ReturnsOKAndInviteAccepted()
        {
            List<long> myKitchenIds = _userManager.TestUser.KitchenUser.Select(u => u.KitchenId).ToList();
            Kitchen notMyKitchen = _context.KitchenUser.Where(u => myKitchenIds.Contains(u.KitchenId) == false).FirstOrDefault()?.Kitchen;

            PantryPlannerUser userSendingInvite = notMyKitchen.KitchenUser
                                                              .Where(k => k.UserId != _userManager.TestUser.Id)
                                                              .FirstOrDefault()?
                                                              .User;

            if (userSendingInvite == null || notMyKitchen == null)
            {
                throw new Exception("user or kitchen is not setup for testing");
            }

            // send invite to test user
            KitchenUserService service = new KitchenUserService(_context);
            service.InviteUserToKitchenByUsername(_userManager.TestUser.UserName, notMyKitchen.KitchenId, userSendingInvite);

            // have test user accept invite
            ActionResult inviteResult = await _controller.AcceptKitchenInvite(notMyKitchen.KitchenId);

            Assert.IsType<OkResult>(inviteResult);
            Assert.True(_context.KitchenUser.Any(k => k.KitchenId == notMyKitchen.KitchenId && k.UserId == _userManager.TestUser.Id && k.HasAcceptedInvite.Value));
        }

        [Fact]
        public async Task AcceptKitchenInvite_NoInvite_ReturnsNotFound()
        {
            List<long> myKitchenIds = _userManager.TestUser.KitchenUser.Select(u => u.KitchenId).ToList();
            Kitchen notMyKitchen = _context.KitchenUser.Where(u => myKitchenIds.Contains(u.KitchenId) == false).FirstOrDefault()?.Kitchen;

            if (notMyKitchen == null)
            {
                throw new Exception("user or kitchen is not setup for testing");
            }

            // have test user accept invite
            ActionResult<bool> inviteResult = await _controller.AcceptKitchenInvite(notMyKitchen.KitchenId);

            Assert.IsType<NotFoundObjectResult>(inviteResult.Result);
        }

        [Fact]
        public async Task AcceptKitchenInvite_InvalidKitchen_ReturnsNotFound()
        {
            ActionResult<bool> inviteResult = await _controller.AcceptKitchenInvite(-5);

            Assert.IsType<NotFoundObjectResult>(inviteResult.Result);
        }


        [Fact]
        public async Task DenyKitchenInvite_ValidKitchen_ReturnsOKAndInviteDenied()
        {
            List<long> myKitchenIds = _userManager.TestUser.KitchenUser.Select(u => u.KitchenId).ToList();
            Kitchen notMyKitchen = _context.KitchenUser.Where(u => myKitchenIds.Contains(u.KitchenId) == false).FirstOrDefault()?.Kitchen;

            PantryPlannerUser userSendingInvite = notMyKitchen.KitchenUser
                                                              .Where(k => k.UserId != _userManager.TestUser.Id)
                                                              .FirstOrDefault()?
                                                              .User;

            if (userSendingInvite == null || notMyKitchen == null)
            {
                throw new Exception("user or kitchen is not setup for testing");
            }

            // send invite to test user
            KitchenUserService service = new KitchenUserService(_context);
            service.InviteUserToKitchenByUsername(_userManager.TestUser.UserName, notMyKitchen.KitchenId, userSendingInvite);

            // have test user accept invite
            ActionResult inviteResult = await _controller.DenyKitchenInvite(notMyKitchen.KitchenId);

            Assert.IsType<OkResult>(inviteResult);
            Assert.False(_context.KitchenUser.Any(k => k.KitchenId == notMyKitchen.KitchenId && k.UserId == _userManager.TestUser.Id));
        }

        [Fact]
        public async Task DenyKitchenInvite_NoInvite_ReturnsNotFound()
        {
            List<long> myKitchenIds = _userManager.TestUser.KitchenUser.Select(u => u.KitchenId).ToList();
            Kitchen notMyKitchen = _context.KitchenUser.Where(u => myKitchenIds.Contains(u.KitchenId) == false).FirstOrDefault()?.Kitchen;

            if (notMyKitchen == null)
            {
                throw new Exception("user or kitchen is not setup for testing");
            }

            // have test user accept invite
            ActionResult<bool> inviteResult = await _controller.DenyKitchenInvite(notMyKitchen.KitchenId);

            Assert.IsType<NotFoundObjectResult>(inviteResult.Result);
        }

        [Fact]
        public async Task DenyKitchenInvite_InvalidKitchen_ReturnsNotFound()
        {
            ActionResult<bool> inviteResult = await _controller.DenyKitchenInvite(-5);

            Assert.IsType<NotFoundObjectResult>(inviteResult.Result);
        }



        #endregion

        #region Delete Test Methods

        [Fact]
        public async Task DeleteKitchenUserByKitchenUserId_ValidKitchenAndUser_ReturnsDeletedUser()
        {
            // generate test data to ensure atleast 1 user is in kitchen that can be removed
            InMemoryDataGenerator.AddNewRandomUsersToKitchens(_context, 1);

            Kitchen myKitchen = _userManager.TestUser.KitchenUser.Where(k => k.IsOwner).FirstOrDefault()?.Kitchen;

            // find a user in the kitchen to delete (that is not the test user)
            KitchenUser userToDelete = myKitchen?.KitchenUser.Where(k => k.UserId != _userManager.TestUser.Id && !k.IsOwner).FirstOrDefault();

            if (userToDelete == null)
            {
                throw new Exception("user not setup for testing");
            }


            int countBeforeDelete = myKitchen.KitchenUser.Count;

            ActionResult<KitchenUserDto> deleteResult = await _controller.DeleteKitchenUserByKitchenUserId(userToDelete.KitchenUserId);

            Assert.IsType<OkObjectResult>(deleteResult.Result);

            KitchenUserDto deletedKitchen = (deleteResult.Result as OkObjectResult).Value as KitchenUserDto;

            Assert.Equal(countBeforeDelete - 1, myKitchen.KitchenUser.Count);
            Assert.Equal(new KitchenUserDto(userToDelete).ToString(), deletedKitchen.ToString());
            Assert.Equal(new KitchenUserDto(userToDelete).UserId, deletedKitchen.UserId);
            Assert.Equal(new KitchenUserDto(userToDelete).KitchenId, deletedKitchen.KitchenId);
        }

        #endregion
    }
}
