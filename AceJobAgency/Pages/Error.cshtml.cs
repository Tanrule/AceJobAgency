using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;

namespace AceJobAgency.Pages
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [IgnoreAntiforgeryToken]
    public class ErrorModel : PageModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        public string ErrorCode { get; set; }
        public string? ErrorMessage { get; set; }
        public string? ErrorDescription { get; set; }

        private readonly ILogger<ErrorModel> _logger;
        private readonly IConfiguration _configuration;

        public ErrorModel(ILogger<ErrorModel> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<IActionResult> OnGet([FromQuery] string? xCode = null)
        {
            HandleError(HttpContext,xCode);
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            return Page();
        }

        private void HandleError(HttpContext httpContext, string? xCode = null)
        {
            if (!string.Equals(xCode, null))
            {
                var value = _configuration.GetValue<string>($"Errors:{xCode}");
                ErrorMessage = value;
                ErrorCode = xCode;
            }
            else
            {
                ErrorMessage = "Something went wrong, please try again.";
                ErrorCode = httpContext.Response.StatusCode.ToString();
            }
        }

        public async Task<IActionResult> OnPost()
        {
            HandleError(HttpContext);
            return Page();
        }
    }
}