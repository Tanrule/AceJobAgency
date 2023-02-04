using AceJobAgency.Core.Interfaces.Utility;
using AceJobAgency.Core.Services;
using AceJobAgency.Infra.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;

namespace AceJobAgency.Pages.Account
{
    [ValidateAntiForgeryToken]
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly IEmailClient _emailClient;
        private readonly UserService _userService;

        public ForgotPasswordModel(UserManager<User> userManager, IEmailClient emailClient, UserService userService)
        {
            _userManager = userManager;
            _emailClient = emailClient;
            _userService = userService;
        }
        [BindProperty]
        public ForgotPasswordInputModel? InputModel { get; set; }
        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(InputModel.Email);
                if (user == null)
                {
                    return RedirectToPage("./ForgotPasswordConfirmation");
                }
                var person = _userService.GetPersonById(user.PersonId);
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ResetPassword",
                    pageHandler: null,
                    values: new { area = "Identity", code, email = InputModel.Email },
                    protocol: Request.Scheme);

                _emailClient.SendMail(
                    user.Email,
                    person?.FirstName,
                    $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.",
                    "Reset Password");

                return RedirectToPage("./ForgotPasswordConfirmation");
            }

            return Page();
        }
    }

    public class ForgotPasswordInputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
