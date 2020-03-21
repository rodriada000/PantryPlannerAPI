using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PantryPlanner.Models;
using PantryPlanner.Services;

namespace PantryPlanner.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ExternalLoginModel : PageModel
    {
        private readonly AccountService _accountService;
        private readonly SignInManager<PantryPlannerUser> _signInManager;
        private readonly ILogger<ExternalLoginModel> _logger;

        public ExternalLoginModel(
            SignInManager<PantryPlannerUser> signInManager,
            UserManager<PantryPlannerUser> userManager,
            ILogger<ExternalLoginModel> logger, 
            IConfiguration configuration)
        {
            _signInManager = signInManager;
            _accountService = new AccountService(userManager, signInManager, configuration);
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string LoginProvider { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public IActionResult OnGetAsync()
        {
            return RedirectToPage("./Login");
        }

        public IActionResult OnPost(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Page("./ExternalLogin", pageHandler: "Callback", values: new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> OnGetCallbackAsync(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (remoteError != null)
            {
                ErrorMessage = $"Error from external provider: {remoteError}";
                return RedirectToPage("./Login", new {ReturnUrl = returnUrl });
            }



            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ErrorMessage = "Error loading external login information.";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            // Sign in the user with this external login provider if the user already has a login.
            if (info.LoginProvider == "Google")
            {
                List<Microsoft.AspNetCore.Authentication.AuthenticationToken> tokens = info.AuthenticationTokens.ToList();
                string jwtToken = await _accountService.LoginUsingGoogleIdToken(tokens[0].Value);

                if (!string.IsNullOrEmpty(jwtToken))
                {
                    var jwtPrinciple = _accountService.GetClaimsPrincipalForJwtToken(jwtToken);

                    var claims = new List<Claim>
                            {
                              new Claim(ClaimTypes.Name, jwtPrinciple.FindFirstValue(ClaimTypes.Name)),
                              new Claim("jwt_token", jwtToken)
                            };

                    var authProperties = new AuthenticationProperties();
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                    return LocalRedirect(returnUrl);
                }
            }

            return Page(); // if got here, then something wrong happened
        }

        public async Task<IActionResult> OnPostConfirmationAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            // Get the information about the user from the external login provider
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ErrorMessage = "Error loading external login information during confirmation.";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            if (ModelState.IsValid)
            {
                //var user = new PantryPlannerUser { UserName = Input.Email, Email = Input.Email };
                //var result = await _userManager.CreateAsync(user);
                //if (result.Succeeded)
                //{
                //    result = await _userManager.AddLoginAsync(user, info);
                //    if (result.Succeeded)
                //    {
                //        await _signInManager.SignInAsync(user, isPersistent: false);
                //        _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);
                //        return LocalRedirect(returnUrl);
                //    }
                //}
                //foreach (var error in result.Errors)
                //{
                //    ModelState.AddModelError(string.Empty, error.Description);
                //}
            }

            LoginProvider = info.LoginProvider;
            ReturnUrl = returnUrl;
            return Page();
        }
    }
}
