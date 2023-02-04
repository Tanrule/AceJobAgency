using AceJobAgency.Core.Entities.Ecosystem;
using AceJobAgency.Core.Interfaces.Utility;
using AceJobAgency.Core.Services;
using AceJobAgency.Helpers;
using AceJobAgency.Infra.Entities.Identity;
using AceJobAgency.Infra.Helpers;
using AspNetCore.ReCaptcha;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;

namespace AceJobAgency.Pages.Account
{
    [ValidateAntiForgeryToken]
    public class RegisterModel : PageModel
    {
        private readonly UserService _userService;
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _environment;
        private readonly FileHelper _fileHelper;
        private readonly IEmailClient _emailClient;

        public RegisterModel(UserService userService, UserManager<User> userManager, IWebHostEnvironment environment,
            FileHelper fileHelper, IEmailClient emailClient)
        {
            _userService = userService;
            _userManager = userManager;
            _environment = environment;
            _fileHelper = fileHelper;
            _emailClient = emailClient;
        }
        public string ReturnUrl { get; set; }
        [BindProperty]
        public InputModel InputModel { get; set; }

        public void OnGetAsync()
        {
            ReturnUrl = Url.Content("~/");
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            ReturnUrl ??= Url.Content("~/");
            if (ModelState.IsValid)
            {
                if (_userManager.Users.Any(t => t.Email.ToLower() == InputModel.Email.ToLower()))
                {
                    ModelState.AddModelError("InputModel.Email", "User already exists");
                    return Page();
                }

                var keyNew = IdentityHelper.GeneratePassword();
                var password = IdentityHelper.EncodePassword(InputModel.Password, keyNew);
                var nric = IdentityHelper.Encrypt(InputModel.NRIC, keyNew);
                var user = new User()
                {
                    UserName = InputModel.Email,
                    Email = InputModel.Email,
                    SCode = keyNew,
                    EmailConfirmed = true,
                    TwoFactorEnabled = true
                };
                var result = await _userManager.CreateAsync(user, password);
                await _userManager.AddToRoleAsync(user, InputModel.Role);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("InputModel.Password", "Passwords must have at least one non alphanumeric character, one digit ('0'-'9'), one uppercase ('A'-'Z').");
                    return Page();
                }
                var user1 = await _userService.CreatePerson(user.Id, InputModel.FirstName,
                    InputModel.LastName, InputModel.DateOfBirth, InputModel.Gender, nric);

                if (InputModel.Resume != null)
                {
                    var path = await _fileHelper.UploadFile(user.Id, InputModel.Resume);
                    var document = new Document()
                    {
                        DocumentType = DocumentType.Resume,
                        FileUrl = path,
                        IsActive = true,
                        Version = 1,
                        PersonId = user1.Id,
                        FileName = InputModel.Resume.FileName
                    };
                    await _userService.CreateDocument(document);
                }

                await _userService.InsertAudit(nameof(OnPostAsync),
                    Convert.ToString(HttpContext.Connection.RemoteIpAddress),
                string.Empty, string.Empty, Convert.ToString(HttpContext.Request.Path),
                "Register", HttpContext.Session.Id, string.Empty, "POST", null, "User registered successfully.");

                if (_userManager.Options.SignIn.RequireConfirmedEmail)
                {
                    var xcode = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    xcode = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(xcode));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = xcode, returnUrl = returnUrl },
                        protocol: Request.Scheme);
                    _emailClient.SendMail(InputModel.Email, InputModel.FirstName,
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.", "Confirm your email");
                    return RedirectToPage("/Account/RegisterConfirmation", new { email = InputModel.Email, returnUrl = returnUrl });
                }



                return LocalRedirect("~/Account/Login");
            }
            return Page();
        }

        public List<SelectListItem> GetGenders()
        {
            List<SelectListItem> genders = new List<SelectListItem>();
            genders.Add(new SelectListItem("Male", "0"));
            genders.Add(new SelectListItem("Female", "1"));
            genders.Add(new SelectListItem("Other", "2"));
            return genders;
        }

        public List<SelectListItem> GetRoles()
        {
            List<SelectListItem> roles = new List<SelectListItem>();
            roles.Add(new SelectListItem("Admin", "Admin"));
            roles.Add(new SelectListItem("Applicant", "Applicant"));
            return roles;
        }


    }

    public class InputModel : IValidatableObject
    {
        [DisplayName("First Name")]
        [DataType(DataType.Text)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "*First name is required")]
        public string FirstName { get; set; }

        [DisplayName("Last Name")]
        public string? LastName { get; set; }
        [Required]
        public Gender Gender { get; set; }
        [Required]
        public string Role { get; set; }

        [DisplayName("NRIC")]
        [DataType(DataType.Text)]
        [RegularExpression("(?i)^[STFG]\\d{7}[A-Z]$", ErrorMessage = "*Invalid NRIC number! Enter valid 9 digit number")]
        public string NRIC { get; set; }

        [DisplayName("Email Address")]
        [EmailAddress]
        [Required(AllowEmptyStrings = false, ErrorMessage = "*Email Address is Required")]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "*Password is required")]
        [StringLength(20, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 12)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "*Confirm Password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "*Date of Birth is required")]
        [DataType(DataType.Date, ErrorMessage = "*Please select valid date")]
        public DateTime DateOfBirth { get; set; }

        public IFormFile? Resume { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            Regex reg = new Regex(@"[a-zA-Z0-9]");
            var results = new List<ValidationResult>();
            if (string.IsNullOrEmpty(FirstName) || string.IsNullOrWhiteSpace(FirstName))
                results.Add(new ValidationResult("First name folks", new string[] { nameof(FirstName) }));
            else
            {
                string str = reg.Replace(FirstName, "");
                if (str.Count() > 0)
                    results.Add(new ValidationResult("*First name is invalid.", new string[] { nameof(FirstName) }));
            }
            if (!string.IsNullOrEmpty(LastName) && !string.IsNullOrWhiteSpace(LastName))
            {
                string str = reg.Replace(LastName, "");
                if (str.Count() > 0)
                    results.Add(new ValidationResult("*Last name is invalid.", new string[] { nameof(LastName) }));
            }
            string regex = "^(?=.*[a-z])(?=."
                    + "*[A-Z])(?=.*\\d)"
                    + "(?=.*[-+_!@#$%^&*., ?]).+$";
            Regex pass = new Regex(regex);
            if (!pass.IsMatch(Password))
            {
                results.Add(new ValidationResult("Passwords must have at least one non alphanumeric character, one digit ('0'-'9'), one uppercase ('A'-'Z').",
                    new string[] { nameof(Password) }));
            }
            return results;
        }
    }


}
