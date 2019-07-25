using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PantryPlanner.Models;
using PantryPlanner.Services;

namespace PantryPlanner.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoogleTokenValidatorController : ControllerBase
    {
        private readonly SignInManager<PantryPlannerUser> _signInManager;
        private readonly UserManager<PantryPlannerUser> _userManager;
        private readonly IConfiguration _configuration;


        public GoogleTokenValidatorController(UserManager<PantryPlannerUser> userManager, SignInManager<PantryPlannerUser> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }


        [HttpPost]
        [Route("Login")]
        public async Task<ActionResult<object>> ValidateTokenAndLoginAsync(string idToken)
        {
            bool isValid = await AccountService.IsGoogleTokenValidAsync(idToken);

            if (!isValid)
            {
                return Unauthorized();
            }

            GoogleJsonWebSignature.Payload validPayload = await GoogleJsonWebSignature.ValidateAsync(idToken);

            PantryPlannerUser appUser = _userManager.Users.SingleOrDefault(u => u.Email == validPayload.Email);

            if (appUser == null)
            {
                // user doesn't exist so we'll auto create them
                appUser = await AccountService.AutoCreateAccountFromGoogleAsync(validPayload, _userManager, _signInManager);
            }

            
            if (appUser != null)
            {
                // sign the user in and return a Jwt Token
                await _signInManager.SignInAsync(appUser, false);
                object token = AccountService.GenerateJwtToken(appUser.Email, appUser, _configuration);
                return token;
            }


            // reached here then the user could not be created/found
            return Unauthorized($"Could not login with google user for email {validPayload.Email}");
        }
    }
}