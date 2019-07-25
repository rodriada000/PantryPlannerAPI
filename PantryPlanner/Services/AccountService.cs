using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
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
    public static class AccountService
    {
        public static object GenerateJwtToken(string email, PantryPlannerUser user, IConfiguration configuration)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
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

        public static async Task<PantryPlannerUser> AutoCreateAccountFromGoogleAsync(GoogleJsonWebSignature.Payload validPayload, UserManager<PantryPlannerUser> userManager, SignInManager<PantryPlannerUser> signInManager)
        {
            PantryPlannerUser user = new PantryPlannerUser()
            {
                UserName = validPayload.Email,
                Email = validPayload.Email
            };

            var passwordOptions = signInManager.Options.Password;
            string randomPassword = PasswordGenerator.GeneratePassword(passwordOptions.RequireLowercase, passwordOptions.RequireUppercase, passwordOptions.RequireDigit, passwordOptions.RequireNonAlphanumeric, false, passwordOptions.RequiredLength);

            IdentityResult result = await userManager.CreateAsync(user, randomPassword);

            if (result.Succeeded)
            {
                return user;
            }

            return null;
        }
    }


    /// <summary>
    /// Generate a random password for auto created accounts
    /// </summary>
    /// <remarks>
    /// thanks to: https://codeshare.co.uk/blog/how-to-create-a-random-password-generator-in-c/
    /// </remarks>
    public static class PasswordGenerator
    {
        /// <summary>
        /// Generates a random password based on the rules passed in the parameters
        /// </summary>
        /// <param name="includeLowercase">Bool to say if lowercase are required</param>
        /// <param name="includeUppercase">Bool to say if uppercase are required</param>
        /// <param name="includeNumeric">Bool to say if numerics are required</param>
        /// <param name="includeSpecial">Bool to say if special characters are required</param>
        /// <param name="includeSpaces">Bool to say if spaces are required</param>
        /// <param name="lengthOfPassword">Length of password required. Should be between 8 and 128</param>
        /// <returns></returns>
        public static string GeneratePassword(bool includeLowercase, bool includeUppercase, bool includeNumeric, bool includeSpecial, bool includeSpaces, int lengthOfPassword)
        {
            const int MAXIMUM_IDENTICAL_CONSECUTIVE_CHARS = 2;
            const string LOWERCASE_CHARACTERS = "abcdefghijklmnopqrstuvwxyz";
            const string UPPERCASE_CHARACTERS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string NUMERIC_CHARACTERS = "0123456789";
            const string SPECIAL_CHARACTERS = "!#$%&*@";
            const string SPACE_CHARACTER = " ";
            const int PASSWORD_LENGTH_MIN = 6;
            const int PASSWORD_LENGTH_MAX = 128;

            if (lengthOfPassword < PASSWORD_LENGTH_MIN || lengthOfPassword > PASSWORD_LENGTH_MAX)
            {
                throw new Exception($"Password length must be between {PASSWORD_LENGTH_MIN} and {PASSWORD_LENGTH_MAX}.");
            }

            if (!includeLowercase && !includeUppercase && !includeNumeric && !includeSpecial && !includeSpaces)
            {
                throw new Exception("atleast one option must be set to tru");
            }

            string characterSet = "";

            if (includeLowercase)
            {
                characterSet += LOWERCASE_CHARACTERS;
            }

            if (includeUppercase)
            {
                characterSet += UPPERCASE_CHARACTERS;
            }

            if (includeNumeric)
            {
                characterSet += NUMERIC_CHARACTERS;
            }

            if (includeSpecial)
            {
                characterSet += SPECIAL_CHARACTERS;
            }

            if (includeSpaces)
            {
                characterSet += SPACE_CHARACTER;
            }

            char[] password = new char[lengthOfPassword];
            int characterSetLength = characterSet.Length;

            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
            byte[] byteArray = new byte[4];

            for (int characterPosition = 0; characterPosition < lengthOfPassword; characterPosition++)
            {
                provider.GetBytes(byteArray);
                //convert 4 bytes to an integer
                int randomInteger = Math.Abs(BitConverter.ToInt32(byteArray, 0));
                int charIndex = randomInteger % characterSetLength;

                password[characterPosition] = characterSet[charIndex];

                bool moreThanTwoIdenticalInARow =
                    characterPosition > MAXIMUM_IDENTICAL_CONSECUTIVE_CHARS
                    && password[characterPosition] == password[characterPosition - 1]
                    && password[characterPosition - 1] == password[characterPosition - 2];

                if (moreThanTwoIdenticalInARow)
                {
                    characterPosition--;
                }
            }

            return string.Join(null, password);
        }

        /// <summary>
        /// Checks if the password created is valid
        /// </summary>
        /// <param name="includeLowercase">Bool to say if lowercase are required</param>
        /// <param name="includeUppercase">Bool to say if uppercase are required</param>
        /// <param name="includeNumeric">Bool to say if numerics are required</param>
        /// <param name="includeSpecial">Bool to say if special characters are required</param>
        /// <param name="includeSpaces">Bool to say if spaces are required</param>
        /// <param name="password">Generated password</param>
        /// <returns>True or False to say if the password is valid or not</returns>
        public static bool PasswordIsValid(bool includeLowercase, bool includeUppercase, bool includeNumeric, bool includeSpecial, bool includeSpaces, string password)
        {
            const string REGEX_LOWERCASE = @"[a-z]";
            const string REGEX_UPPERCASE = @"[A-Z]";
            const string REGEX_NUMERIC = @"[\d]";
            const string REGEX_SPECIAL = @"([!#$%&*@\\])+";
            const string REGEX_SPACE = @"([ ])+";

            bool lowerCaseIsValid = !includeLowercase || (includeLowercase && Regex.IsMatch(password, REGEX_LOWERCASE));
            bool upperCaseIsValid = !includeUppercase || (includeUppercase && Regex.IsMatch(password, REGEX_UPPERCASE));
            bool numericIsValid = !includeNumeric || (includeNumeric && Regex.IsMatch(password, REGEX_NUMERIC));
            bool symbolsAreValid = !includeSpecial || (includeSpecial && Regex.IsMatch(password, REGEX_SPECIAL));
            bool spacesAreValid = !includeSpaces || (includeSpaces && Regex.IsMatch(password, REGEX_SPACE));

            return (lowerCaseIsValid && upperCaseIsValid && numericIsValid && symbolsAreValid && spacesAreValid);
        }
    }
}
