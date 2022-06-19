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
            if (HttpContext.User.Identity == null || !HttpContext.User.Identity.IsAuthenticated)
            {
                _logger.LogInformation("challenging");
                await HttpContext.ChallengeAsync(new AuthenticationProperties() { RedirectUri = redirectUri });
            }
        }
    }
}