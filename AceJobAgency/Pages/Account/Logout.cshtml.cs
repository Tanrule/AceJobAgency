using AceJobAgency.Core.Services;
using AceJobAgency.Infra.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AceJobAgency.Pages.Account
{
    [ValidateAntiForgeryToken]
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserService _userService;

        public LogoutModel(SignInManager<User> signInManager, UserService userService)
        {
            _signInManager = signInManager;
            _userService = userService;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/Account/Login");
            var user = _signInManager.UserManager.Users.SingleOrDefault(t => t.Email.ToLower() == User.Identity.Name.ToLower());
            if(user != null)
            {

                await _signInManager.SignOutAsync();
                await _userService.InsertAudit(nameof(OnPostAsync),
                   Convert.ToString(HttpContext.Connection.RemoteIpAddress),
                   string.Empty, DateTime.Now.ToString(), Convert.ToString(HttpContext.Request.Path),
                   "Logout", HttpContext.Session.Id, string.Empty, "POST", user.Id, "User logged out successfully.");
            }
            return LocalRedirect(returnUrl);
        }
    }
}
