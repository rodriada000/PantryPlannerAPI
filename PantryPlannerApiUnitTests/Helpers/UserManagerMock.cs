using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using PantryPlanner.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PantryPlannerApiUnitTests.Helpers
{
    class UserManagerMock
    {
        /// <summary>
        /// Create a UserManager that can be used for unit tests
        /// </summary>
        /// <typeparam name="TUser"> pass in list of users to user in UserManager </typeparam>
        /// <remarks>
        /// reference: https://stackoverflow.com/questions/49165810/how-to-mock-usermanager-in-net-core-testing
        /// </remarks>
        public static Mock<UserManager<TUser>> MockUserManager<TUser>(List<TUser> ls) where TUser : class
        {
            var store = new Mock<IUserStore<TUser>>();
            var mgr = new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
            mgr.Object.UserValidators.Add(new UserValidator<TUser>());
            mgr.Object.PasswordValidators.Add(new PasswordValidator<TUser>());

            mgr.Setup(x => x.DeleteAsync(It.IsAny<TUser>())).ReturnsAsync(IdentityResult.Success);
            mgr.Setup(x => x.CreateAsync(It.IsAny<TUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success).Callback<TUser, string>((x, y) => ls.Add(x));
            mgr.Setup(x => x.UpdateAsync(It.IsAny<TUser>())).ReturnsAsync(IdentityResult.Success);

            return mgr;
        }
    }

    /// <summary>
    /// Mocks up all components of a <see cref="UserManager{PantryPlannerUser}"/>
    /// and returns hardcoded user for testing purposes.
    /// </summary>
    /// <remarks> idea from https://stackoverflow.com/questions/21194324/mocking-new-microsoft-entity-framework-identity-usermanager-and-rolemanager </remarks>
    public class FakeUserManager : UserManager<PantryPlannerUser>
    {
        public PantryPlannerUser TestUser { get; set; }

        public FakeUserManager()
            : base(new Mock<IUserStore<PantryPlannerUser>>().Object,
                  new Mock<IOptions<IdentityOptions>>().Object,
                  new Mock<IPasswordHasher<PantryPlannerUser>>().Object,
                  new IUserValidator<PantryPlannerUser>[0],
                  new IPasswordValidator<PantryPlannerUser>[0],
                  new Mock<ILookupNormalizer>().Object,
                  new Mock<IdentityErrorDescriber>().Object,
                  new Mock<IServiceProvider>().Object,
                  new Mock<ILogger<UserManager<PantryPlannerUser>>>().Object)
        {
            TestUser = new PantryPlannerUser { Id = "test12345", Email = "test@test.com", UserName = "Iliketurtles" };
        }

        public override Task<PantryPlannerUser> FindByEmailAsync(string email)
        {
            return Task.FromResult(new PantryPlannerUser { Email = email });
        }

        public override Task<PantryPlannerUser> GetUserAsync(ClaimsPrincipal claim)
        {
            return Task.FromResult(TestUser);
        }

        public override Task<bool> IsEmailConfirmedAsync(PantryPlannerUser user)
        {
            return Task.FromResult(user.Email == TestUser.Email);
        }

        public override Task<string> GeneratePasswordResetTokenAsync(PantryPlannerUser user)
        {
            return Task.FromResult("---------------");
        }
    }
}
