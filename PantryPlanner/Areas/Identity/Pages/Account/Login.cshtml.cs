using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using PantryPlanner.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using System;
using PantryPlanner.Services;
using Microsoft.Extensions.Configuration;

namespace PantryPlanner.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly SignInManager<PantryPlannerUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly AccountService _accountService;

        public LoginModel(
            SignInManager<PantryPlannerUser> signInManager,
            UserManager<PantryPlannerUser> userManager,
            ILogger<LoginModel> logger,
            IConfiguration configuration)
        {
            _signInManager = signInManager;
            _logger = logger;
            _accountService = new AccountService(userManager, signInManager, configuration);
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl = returnUrl ?? Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                string jwtToken = await _accountService.LoginWithEmailAndPasswordAsync(new DTOs.LoginDto() { Email = Input.Email, Password = Input.Password });
                
                if (!string.IsNullOrEmpty(jwtToken))
                {
                    _logger.LogInformation("User logged in.");

                    var claims = new List<Claim>
                            {
                              new Claim(ClaimTypes.Name, Input.Email),
                              new Claim("jwt_token", jwtToken)
                            };

                    var authProperties = new AuthenticationProperties();
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                    return LocalRedirect(returnUrl);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
