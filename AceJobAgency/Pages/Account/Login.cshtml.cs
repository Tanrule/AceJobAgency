using AceJobAgency.Core.Services;
using AceJobAgency.Infra.Entities.Identity;
using AceJobAgency.Infra.Helpers;
using AspNetCore.ReCaptcha;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace AceJobAgency.Pages.Account
{
    [ValidateAntiForgeryToken]
    [ValidateReCaptcha]
    public class LoginModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserService _userService;
        private readonly UserManager<User> _userManager;

        public LoginModel(SignInManager<User> signInManager, UserService userService, UserManager<User> userManager)
        {
            _signInManager = signInManager;
            _userService = userService;
            _userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }
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
            ReturnUrl ??= Url.Content("~/");
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            await _userService.InsertAudit(nameof(OnGetAsync),
                Convert.ToString(HttpContext.Connection.RemoteIpAddress),
                string.Empty, string.Empty, Convert.ToString(HttpContext.Request.Path),
                "Login", HttpContext.Session.Id, string.Empty, "GET", null, "");
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            ReturnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user != null)
                {
                    var encodingPasswordString = IdentityHelper.EncodePassword(Input.Password, user.SCode);
                    var result = await _signInManager.PasswordSignInAsync(user, encodingPasswordString, Input.RememberMe, lockoutOnFailure: true);
                    if (result.Succeeded)
                    {
                        await _userService.InsertAudit(nameof(OnPostAsync),
                            Convert.ToString(HttpContext.Connection.RemoteIpAddress),
                            DateTime.Now.ToString(), string.Empty, Convert.ToString(HttpContext.Request.Path),
                            "Login", HttpContext.Session.Id, string.Empty, "POST", user.Id, "Logged in successfully.");
                        user.IpAddress = Convert.ToString(HttpContext.Connection.RemoteIpAddress);
                        await _userManager.UpdateAsync(user);

                        return LocalRedirect(returnUrl);
                    }
                    if (result.RequiresTwoFactor)
                    {
                        return RedirectToPage("/Account/LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                    }
                    if (result.IsLockedOut)
                    {
                        await _userService.InsertAudit(nameof(OnPostAsync),
                            Convert.ToString(HttpContext.Connection.RemoteIpAddress),
                            DateTime.Now.ToString(), string.Empty, Convert.ToString(HttpContext.Request.Path),
                            "Login", HttpContext.Session.Id, string.Empty, "POST", user.Id, "User locked out 3 failed attempts");
                        ModelState.AddModelError(string.Empty, result.ToString() == "NotAllowed" ? "Not allowed attempt" : "User locked out please again try in 1 minute.");
                        return Page();
                    }
                    else
                    {
                        await _userService.InsertAudit(nameof(OnPostAsync),
                            Convert.ToString(HttpContext.Connection.RemoteIpAddress),
                            DateTime.Now.ToString(), string.Empty, Convert.ToString(HttpContext.Request.Path),
                            "Login", HttpContext.Session.Id, string.Empty, "POST", user.Id, $"Invalid login attempt no. {user.AccessFailedCount}");
                        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                        return Page();
                    }
                }
            }
            return Page();
        }
    }
}
