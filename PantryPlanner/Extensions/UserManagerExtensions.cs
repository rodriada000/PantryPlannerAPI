using Microsoft.AspNetCore.Identity;
using PantryPlanner.Exceptions;
using PantryPlanner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PantryPlanner.Extensions
{
    public static class UserManagerExtensions
    {
        /// <summary>
        /// Returns the PantryPlannerUser associated with the <paramref name="userClaimsPrincipal"/>. 
        /// Determines user based on Cookies or checking the Claims that are set by Jwt for authorization.
        /// </summary>
        /// <exception cref="PermissionsException"> Thrown when user can not be found and is null </exception>
        /// <param name="userManager"> </param>
        /// <param name="userClaimsPrincipal"></param>
        /// <returns> A valid <see cref="PantryPlannerUser"/> </returns>
        public static async Task<PantryPlannerUser> GetUserFromCookieOrJwtAsync(this UserManager<PantryPlannerUser> userManager, ClaimsPrincipal userClaimsPrincipal)
        {
            // first attempt to get the user based on Cookies
            PantryPlannerUser user = await userManager.GetUserAsync(userClaimsPrincipal);

            if (user != null)
            {
                return user;
            }


            // Second attempt to get the user based on Jwt claims (User Id is stored in Jwt)... reference: https://stackoverflow.com/questions/46112258/how-do-i-get-current-user-in-net-core-web-api-from-jwt-token
            IEnumerable<Claim> nameIdentifiers = userClaimsPrincipal.FindAll(ClaimTypes.NameIdentifier); 

            foreach (var claim in nameIdentifiers) // looping over nameidentifier claims because it could be the email or Id in the token
            {
                user = await userManager.FindByIdAsync(claim.Value);

                if (user != null)
                {
                    break;
                }
            }


            if (user == null)
            {
                // no user could be determined from Cookies passed in or from the Jwt token so throw a unauthorized result
                throw new PermissionsException("Could not authenticate user.");
            }

            return user;
        }
    }
}
