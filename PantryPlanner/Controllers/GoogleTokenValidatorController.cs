using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PantryPlanner.Exceptions;
using PantryPlanner.Models;
using PantryPlanner.Services;

namespace PantryPlanner.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoogleTokenValidatorController : ControllerBase
    {
        private readonly AccountService _accountService;


        public GoogleTokenValidatorController(UserManager<PantryPlannerUser> userManager, SignInManager<PantryPlannerUser> signInManager, IConfiguration configuration)
        {
            _accountService = new AccountService(userManager, signInManager, configuration);
        }


        [HttpPost]
        [Route("Login")]
        public async Task<ActionResult<string>> ValidateTokenAndLoginAsync(string idToken)
        {
            try
            {
                string token = await _accountService.LoginUsingGoogleIdToken(idToken);
                return token;
            }
            catch (AccountException e)
            {
                return Unauthorized(e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }
    }
}