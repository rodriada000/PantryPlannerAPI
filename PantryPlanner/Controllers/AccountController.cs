using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PantryPlanner.DTOs;
using PantryPlanner.Models;
using PantryPlanner.Services;

namespace PantryPlanner.Controllers
{
    /// <summary>
    /// Login and Register users using Jwt Bearer Tokens
    /// </summary>
    /// <remarks>
    /// from: https://medium.com/@ozgurgul/asp-net-core-2-0-webapi-jwt-authentication-with-identity-mysql-3698eeba6ff8
    /// </remarks>
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly SignInManager<PantryPlannerUser> _signInManager;
        private readonly UserManager<PantryPlannerUser> _userManager;
        private readonly IConfiguration _configuration;

        public AccountController(UserManager<PantryPlannerUser> userManager, SignInManager<PantryPlannerUser> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("Login")]
        public async Task<object> LoginAsync([FromBody] LoginDto model)
        {
            Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);

            if (result.Succeeded)
            {
                PantryPlannerUser appUser = _userManager.Users.SingleOrDefault(r => r.Email == model.Email);
                object token = AccountService.GenerateJwtToken(model.Email, appUser, _configuration);
                return token;
            }
            else if (result.IsLockedOut)
            {
                return Unauthorized("The user is locked out");
            }
            else if (result.IsNotAllowed)
            {
                return Unauthorized("The user is not allowed to login");
            }

            return StatusCode(StatusCodes.Status500InternalServerError, "INVALID LOGIN ATTEMPT");
        }

        [HttpPost]
        [Route("Register")]
        public async Task<object> RegisterAsync([FromBody] RegisterDto model)
        {
            PantryPlannerUser user = new PantryPlannerUser()
            {
                UserName = model.Email,
                Email = model.Email
            };

            IdentityResult result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, false);
                object token = AccountService.GenerateJwtToken(model.Email, user, _configuration);
                return token;
            }

            return StatusCode(StatusCodes.Status500InternalServerError, String.Join(',', result.Errors.Select(e => e.Description)));
        }

    }
}
