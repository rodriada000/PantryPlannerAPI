﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
        public async Task<ActionResult<string>> LoginAsync([FromBody] LoginDto model)
        {
            try
            {
                string token = await _accountService.LoginWithEmailAndPasswordAsync(model);
                return Ok(token);
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
        public async Task<ActionResult<string>> RegisterAsync([FromBody] LoginDto model)
        {
            try
            {
                string token = await _accountService.RegisterWithEmailAndPasswordAsync(model);
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

        [HttpGet]
        [Route("NewToken")]
        [Authorize]
        public async Task<ActionResult<string>> GetNewTokenForLoggedInUserAsync()
        {
            PantryPlannerUser user;
            string jwtTokenFromHeader;


            try
            {
                // get the current Jwt Token from Request Headers
                if (this.Request.Headers.ContainsKey("Authorization"))
                {
                    Microsoft.Extensions.Primitives.StringValues headerValue;
                    Request.Headers.TryGetValue("Authorization", out headerValue);

                    string bearerPrefix = "Bearer ";
                    jwtTokenFromHeader = headerValue.FirstOrDefault().Substring(bearerPrefix.Length);
                }
                else
                {
                    return BadRequest("Authorization header is missing");
                }

                user = await _accountService.GetUserForJwtTokenAsync(jwtTokenFromHeader);
                string newToken = await _accountService.ValidateAndGenerateNewJwtTokenAsync(jwtTokenFromHeader, user);

                return Ok(newToken);
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
