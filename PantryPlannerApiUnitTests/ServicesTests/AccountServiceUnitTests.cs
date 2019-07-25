using Google.Apis.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
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
    public class AccountServiceUnitTests
    {
        PantryPlannerContext _context;
        PantryPlannerUser _testUser;
        AccountService _service;
        UserManager<PantryPlannerUser> _userManager;
        SignInManager<PantryPlannerUser> _signInManager;


        public AccountServiceUnitTests()
        {
            _testUser = InMemoryDataGenerator.TestUser;
            _context = InMemoryDataGenerator.CreateAndInitializeInMemoryDatabaseContext(Guid.NewGuid().ToString(), _testUser, insertIngredientData: false);
            _userManager = UserManagerMock.MockUserManager<PantryPlannerUser>().Object;

            _signInManager = new SignInManager<PantryPlannerUser>(_userManager
                                                                , new Mock<IHttpContextAccessor>().Object
                                                                , new Mock<IUserClaimsPrincipalFactory<PantryPlannerUser>>().Object
                                                                , new Mock<IOptions<IdentityOptions>>().Object
                                                                , new Mock<ILogger<SignInManager<PantryPlannerUser>>>().Object
                                                                , new Mock<IAuthenticationSchemeProvider>().Object);

            _service = new AccountService(_userManager, _signInManager, null);
        }

        #region password generator test methods

        [Fact]
        public void GeneratePassword_IsCorrectLength()
        {
            int expectedLength = 8;

            string pwd = PasswordGenerator.GeneratePassword(true, true, true, true, false, expectedLength);
            Assert.Equal(expectedLength, pwd.Length);

            pwd = PasswordGenerator.GeneratePassword(true, true, true, false, false, expectedLength);
            Assert.Equal(expectedLength, pwd.Length);

            pwd = PasswordGenerator.GeneratePassword(true, true, false, false, false, expectedLength);
            Assert.Equal(expectedLength, pwd.Length);

            pwd = PasswordGenerator.GeneratePassword(true, false, false, false, false, expectedLength);
            Assert.Equal(expectedLength, pwd.Length);


            expectedLength = 32;
            pwd = PasswordGenerator.GeneratePassword(true, true, true, true, false, expectedLength);
            Assert.Equal(expectedLength, pwd.Length);

            expectedLength = 85;
            pwd = PasswordGenerator.GeneratePassword(true, true, true, true, false, expectedLength);
            Assert.Equal(expectedLength, pwd.Length);

            expectedLength = 100;
            pwd = PasswordGenerator.GeneratePassword(true, true, true, true, false, expectedLength);
            Assert.Equal(expectedLength, pwd.Length);

        }

        [Fact]
        public void GeneratePassword_AllOptionsFalse_ThrowsException()
        {
            int expectedLength = 8;

            Assert.Throws<Exception>(() =>
            {
                PasswordGenerator.GeneratePassword(false, false, false, false, false, expectedLength);
            });

        }

        [Fact]
        public void GeneratePassword_PasswordTooShortOrLong_ThrowsException()
        {
            Assert.Throws<Exception>(() =>
            {
                PasswordGenerator.GeneratePassword(false, false, false, false, false, 3);
            });

            Assert.Throws<Exception>(() =>
            {
                PasswordGenerator.GeneratePassword(false, false, false, false, false, 1000);
            });

        }

        #endregion


        [Fact]
        public async Task AutoCreateAccountFromGoogleAsync_Valid_UserReturnedAsync()
        {
            GoogleJsonWebSignature.Payload payload = new GoogleJsonWebSignature.Payload()
            {
                Email = "newGoogleUser@gmail.com"
            };

            PantryPlannerUser newUser = await _service.AutoCreateAccountFromGoogleAsync(payload);

            Assert.NotNull(newUser);
            Assert.Equal(payload.Email, newUser.Email);
        }
    }
}
