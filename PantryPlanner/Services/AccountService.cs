using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PantryPlanner.Classes;
using PantryPlanner.DTOs;
using PantryPlanner.Exceptions;
using PantryPlanner.Extensions;
using PantryPlanner.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PantryPlanner.Services
{
    public class AccountService
    {
        private readonly SignInManager<PantryPlannerUser> _signInManager;
        private readonly UserManager<PantryPlannerUser> _userManager;
        private readonly IConfiguration _configuration;

        public AccountService(UserManager<PantryPlannerUser> userManager, SignInManager<PantryPlannerUser> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }


        /// <summary>
        /// Generate a Jwt Token for the App to authorize later incoming requests
        /// </summary>
        /// <returns>Jwt Token serialized string </returns>
        private static string GenerateJwtToken(PantryPlannerUser user, IConfiguration configuration)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.Email)
            };

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtKey"]));
            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            DateTime expires = DateTime.Now.AddDays(Convert.ToDouble(configuration["JwtExpireDays"]));

            JwtSecurityToken token = new JwtSecurityToken(
                configuration["JwtIssuer"],
                configuration["JwtIssuer"],
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Returns true if the <paramref name="token"/> is signed by the API and the token is NOT expired.
        /// The ClaimsPrincipal for the validated token is returned in an out parameter.
        /// </summary>
        /// <param name="token"> token to validate </param>
        /// <param name="configuration"> IConfiguration section that contains 'JwtKey' and 'JwtIssuer' </param>
        /// <param name="tokenClaims"> Claims for the token, if valid </param>
        public static bool IsJwtTokenValid(string token, IConfiguration configuration, out ClaimsPrincipal tokenClaims)
        {
            SecurityToken returnedSecurityToken;
            JwtSecurityTokenHandler securityHandler = new JwtSecurityTokenHandler();
            TokenValidationParameters validationParams = new TokenValidationParameters()
            {
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtKey"])),
                ValidAudiences = new[] { configuration["JwtIssuer"] },
                ValidIssuer = configuration["JwtIssuer"],
            };


            tokenClaims = securityHandler.ValidateToken(token, validationParams, out returnedSecurityToken);

            return (tokenClaims != null && returnedSecurityToken.ValidTo >= DateTime.UtcNow);
        }

        /// <summary>
        /// Get user from a valid Jwt Token based on the claims in the token
        /// </summary>
        public async Task<PantryPlannerUser> GetUserForJwtTokenAsync(string token)
        {
            PantryPlannerUser user;
            ClaimsPrincipal tokenClaims;

            if (IsJwtTokenValid(token, _configuration, out tokenClaims) == false)
            {
                return null;
            }

            user = await _userManager.GetUserFromCookieOrJwtAsync(tokenClaims);

            return user;
        }

        /// <summary>
        /// Get ClaimsPrincipal from a valid Jwt Token.
        /// </summary>
        public ClaimsPrincipal GetClaimsPrincipalForJwtToken(string token)
        {
            PantryPlannerUser user;
            ClaimsPrincipal tokenClaims;

            if (IsJwtTokenValid(token, _configuration, out tokenClaims) == false)
            {
                return null;
            }

            return tokenClaims;
        }

        /// <summary>
        /// Generate a new Jwt Token for a user with an existing token. 
        /// This method will validate that the <paramref name="token"/> is owned by <paramref name="user"/>
        /// </summary>
        /// <param name="token"></param>
        /// <param name="user"></param>
        /// <remarks>
        /// This is used to generate a new Jwt token for a user that is already logged in. It is useful in third-party
        /// apps e.g. mobile apps to silently login the user and get a new Jwt Token so that the expiration date is renewed.
        /// </remarks>
        public async Task<string> ValidateAndGenerateNewJwtTokenAsync(string token, PantryPlannerUser user)
        {
            ClaimsPrincipal tokenClaims;


            if (IsJwtTokenValid(token, _configuration, out tokenClaims) == false)
            {
                throw new AccountException("token is not valid");
            }

            // validate user passed in matches the token claims
            PantryPlannerUser userFromClaims = await _userManager.GetUserFromCookieOrJwtAsync(tokenClaims);

            if (userFromClaims.Id != user.Id)
            {
                throw new AccountException("token is not assigned to user.");
            }

            string newToken = GenerateJwtToken(user, _configuration);
            return newToken;
        }


        /// <summary>
        /// Validates the <paramref name="id_token"/> and is signed by Google
        /// </summary>
        /// <param name="id_token"></param>
        /// <remarks>
        /// from: https://stackoverflow.com/questions/39061310/validate-google-id-token
        /// </remarks>
        /// <returns> true if token is valid; false otherwise </returns>
        public static async Task<bool> IsGoogleTokenValidAsync(string id_token)
        {
            try
            {
                GoogleJsonWebSignature.Payload tokenPayload = await GoogleJsonWebSignature.ValidateAsync(id_token);
                return (tokenPayload != null);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Register new user to API with username and password.
        /// New user will be signed in if successful and returns Jwt Token for authorizing later incoming requests
        /// </summary>
        /// <param name="model"> DTO with email and password for new user to create </param>
        /// <returns> Jwt Token for authorizing later requests to API </returns>
        /// <exception cref="AccountException"> Thrown when failed to create user. </exception>
        internal async Task<string> RegisterWithEmailAndPasswordAsync(LoginDto model)
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
                string token = GenerateJwtToken(user, _configuration);
                return token;
            }

            throw new AccountException(String.Join(',', result.Errors.Select(e => e.Description)));
        }

        /// <summary>
        /// Login to API with username and password. Jwt Token returned on success that can be used to authorize later requests.
        /// </summary>
        /// <param name="model"> DTO with email and password to login with </param>
        /// <returns> Jwt Token for logged in user </returns>
        /// <exception cref="AccountException"> Thrown when failed to login (cant find user, user is locked out, or invalid username/password) </exception>
        internal async Task<string> LoginWithEmailAndPasswordAsync(LoginDto model)
        {
            SignInResult result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);

            if (result.Succeeded)
            {
                PantryPlannerUser appUser = _userManager.Users.SingleOrDefault(r => r.Email == model.Email);

                if (appUser == null)
                {
                    throw new AccountException("Could not look up user.");
                }

                string token = GenerateJwtToken(appUser, _configuration);
                return token;
            }
            else if (result.IsLockedOut)
            {
                throw new AccountException("The user is locked out");
            }
            else if (result.IsNotAllowed)
            {
                throw new AccountException("The user is not allowed to login");
            }


            throw new AccountException("Failed to login.");
        }


        /// <summary>
        /// Validate <paramref name="idToken"/> is Google Id Token and then Login the user. An account will be auto-created if the google user does not have an account yet.
        /// Returns a Jwt Token for authorizing later requests.
        /// </summary>
        /// <param name="idToken"> Google Id Token provided by google when signing in with OAuth (e.g. from mobile app) </param>
        /// <returns> Jwt Token for authorizing later requests to API. </returns>
        /// <exception cref="AccountException"> Thrown when <paramref name="idToken"/> is not valid or user can not be created or login </exception>
        public async Task<string> LoginUsingGoogleIdToken(string idToken)
        {
            bool isValid = await IsGoogleTokenValidAsync(idToken).ConfigureAwait(false);

            if (!isValid)
            {
                throw new AccountException("Google Id token is invalid");
            }

            GoogleJsonWebSignature.Payload validPayload = await GoogleJsonWebSignature.ValidateAsync(idToken).ConfigureAwait(false);
            PantryPlannerUser appUser = _userManager.Users.SingleOrDefault(u => u.Email == validPayload.Email);


            if (appUser == null)
            {
                // user doesn't exist so we'll auto create them
                appUser = await AutoCreateAccountFromGoogleAsync(validPayload).ConfigureAwait(false);
            }


            if (appUser != null)
            {
                // sign the user in and return a Jwt Token
                await _signInManager.SignInAsync(appUser, false).ConfigureAwait(false);
                string token = GenerateJwtToken(appUser, _configuration);
                return token;
            }

            // reached here then the user could not be created/found
            throw new AccountException($"Could not login with google user for email {validPayload.Email}");
        }


        /// <summary>
        /// Create a PantryPlanner account based on the email used in the Google <paramref name="validPayload"/>
        /// </summary>
        /// <returns> the new PantryPlannerUser created; null if failed to create </returns>
        public async Task<PantryPlannerUser> AutoCreateAccountFromGoogleAsync(GoogleJsonWebSignature.Payload validPayload)
        {
            PantryPlannerUser user = new PantryPlannerUser()
            {
                UserName = validPayload.Email,
                Email = validPayload.Email
            };

            var passwordOptions = _signInManager.Options.Password;
            string randomPassword = PasswordGenerator.GeneratePassword(passwordOptions.RequireLowercase, passwordOptions.RequireUppercase, passwordOptions.RequireDigit, passwordOptions.RequireNonAlphanumeric, false, passwordOptions.RequiredLength);

            IdentityResult result = await _userManager.CreateAsync(user, randomPassword);

            if (result.Succeeded)
            {
                return user;
            }

            return null;
        }
    }
}
