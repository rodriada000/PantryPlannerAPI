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
using PantryPlanner.Exceptions;
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
        private readonly AccountService _accountService;

        public AccountController(UserManager<PantryPlannerUser> userManager, SignInManager<PantryPlannerUser> signInManager, IConfiguration configuration)
        {
            _accountService = new AccountService(userManager, signInManager, configuration);
        }

        [HttpPost]
        [Route("Login")]
        public async Task<object> LoginAsync([FromBody] LoginDto model)
        {
            try
            {
                object token = await _accountService.LoginWithEmailAndPasswordAsync(model);
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

        [HttpPost]
        [Route("Register")]
        public async Task<object> RegisterAsync([FromBody] RegisterDto model)
        {
            try
            {
                object token = await _accountService.RegisterWithEmailAndPasswordAsync(model);
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
