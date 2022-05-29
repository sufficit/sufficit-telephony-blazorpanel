using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;

namespace Sufficit.Telephony.BlazorPanel.Pages
{
    public class LoginModel : PageModel
    {
        private readonly ILogger _logger;

        public LoginModel(ILogger<LoginModel> logger)
        {
            _logger = logger;
            _logger.LogInformation("created");
        }

        public async Task OnGet(string redirectUri)
        {
            _logger.LogInformation("challenging");
            await HttpContext.ChallengeAsync(OpenIdConnectDefaults.AuthenticationScheme, new AuthenticationProperties() { RedirectUri = redirectUri });
        }
    }
}