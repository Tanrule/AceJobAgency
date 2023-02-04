using AceJobAgency.Core.Entities.Ecosystem;
using AceJobAgency.Core.Services;
using AceJobAgency.Helpers;
using AceJobAgency.Infra.Entities.Identity;
using AceJobAgency.Infra.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace AceJobAgency.Pages
{
    [Authorize]
    [ValidateAntiForgeryToken]
    public class IndexModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly UserService _userService;
        private readonly IWebHostEnvironment _environment;
        private readonly FileHelper _fileHelper;

        [BindProperty]
        public InputModel InputModel { get; set; }
        public IndexModel(UserManager<User> userManager, UserService userService, IWebHostEnvironment environment, FileHelper fileHelper)
        {
            _userManager = userManager;
            _userService = userService;
            _environment = environment;
            _fileHelper = fileHelper;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user != null)
            {
                if (InputModel == null)
                    InputModel = new InputModel();
                if (user.PersonId > 0)
                {
                    Person person = _userService.GetPersonById(user.PersonId);
                    
                    if (person == null)
                    {
                        person = new Person();
                    }
                    else
                    {
                        var encodingNric = IdentityHelper.Decrypt(person.NRIC, user.SCode);
                        InputModel.NRIC = encodingNric;
                    }
                    InputModel.DateOfBirth = person.DateOfBirth;
                    InputModel.FirstName = person.FirstName;
                    InputModel.LastName = person.LastName;
                    InputModel.Gender = person.Gender;
                    var doc = await _userService.GetDocumentByPersonId(person.Id);
                    if (doc != null)
                    {
                        InputModel.FileName = doc.FileName;
                    }
                }
                return Page();
            }
            else
            {
                return LocalRedirect("~/Account/Login");
            }
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                Person person = _userService.GetPersonById(user.PersonId);
                if (person == null)
                    person = new Person();
                if (user.SCode != "")
                {
                    var encodingNric = IdentityHelper.Encrypt(InputModel.NRIC, user.SCode);
                    person.NRIC = encodingNric;
                }

                person.DateOfBirth = InputModel.DateOfBirth;
                person.FirstName = InputModel.FirstName;
                person.LastName = InputModel.LastName;
                person.Gender = InputModel.Gender;
                if (person.Id > 0)
                    await _userService.UpdatePerson(person);
                else
                {
                    person = await _userService.CreatePerson(user.Id, person.FirstName, person.LastName, person.DateOfBirth, person.Gender, person.NRIC);
                    user.PersonId = person.Id;
                    await _userManager.UpdateAsync(user);
                }
                if (InputModel.Resume != null)
                {
                    string file = await _fileHelper.UploadFile(user.Id, InputModel.Resume);
                    var document = await _userService.GetDocumentByPersonId(user.PersonId);
                    if (document == null)
                        document = new Document();
                    document.DocumentType = DocumentType.Resume;
                    document.FileName = InputModel.Resume.FileName;
                    document.FileUrl = file;
                    document.IsActive = true;
                    document.PersonId = user.PersonId;
                    document.Version = 1;
                    if (document.Id > 0)
                        await _userService.UpdateDocument(document);
                    else
                        await _userService.CreateDocument(document);
                }
                await _userService.InsertAudit(nameof(OnPostAsync),
                Convert.ToString(HttpContext.Connection.RemoteIpAddress),
                string.Empty, string.Empty, Convert.ToString(HttpContext.Request.Path),
                "Index", HttpContext.Session.Id, string.Empty, "POST", user.Id, "User updated successfully.");
                return LocalRedirect("~/");
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

    }

    public class InputModel : IValidatableObject
    {
        [DisplayName("First Name")]
        [DataType(DataType.Text)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "*First name is required")]
        public string FirstName { get; set; } = string.Empty;

        [DisplayName("Last Name")]
        public string? LastName { get; set; }
        [Required]
        public Gender Gender { get; set; }

        [DisplayName("NRIC")]
        [DataType(DataType.Text)]
        [RegularExpression("(?i)^[STFG]\\d{7}[A-Z]$", ErrorMessage = "*Invalid NRIC number!")]
        public string NRIC { get; set; } = string.Empty;

        [Required(AllowEmptyStrings = false, ErrorMessage = "*Date of Birth is required")]
        [DataType(DataType.Date, ErrorMessage = "*Please select valid date")]
        public DateTime DateOfBirth { get; set; }
        public IFormFile? Resume { get; set; }
        public string? FileName { get; set; }

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
            return results;
        }
    }
}