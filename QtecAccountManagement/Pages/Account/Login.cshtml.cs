using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QtecAccountManagement.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace QtecAccountManagement.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly IAuthService _authService;

        [BindProperty]
        public Models.AuthModels.LoginModel Input { get; set; }

        public LoginModel(IAuthService authService)
        {
            _authService = authService;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _authService.Authenticate(Input.Username, Input.Password);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return Page();
            }

            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim("FullName", user.FullName ?? string.Empty)
        };

            foreach (var role in user.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
                claims.Add(new Claim(role, "true"));
            }

            var claimsIdentity = new ClaimsIdentity(claims, "CookieAuth");
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true
            };

            await HttpContext.SignInAsync(
                "CookieAuth",
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return RedirectToPage("/Index");
        }
    }
}
