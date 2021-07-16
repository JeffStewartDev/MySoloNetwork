using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using MSN.ViewModel;
using MSN.Model;
using MSN.ModelContext;

namespace MSN.BlazorServer.Pages
{
    [AllowAnonymous]
    public class LoginPageModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<LoginPageModel> _logger;

        public LoginPageModel(SignInManager<IdentityUser> signInManager,
            ILogger<LoginPageModel> logger,
            UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            public string Email { get; set; }

            [Required]
            public string UserName { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string loginid = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }


            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (!string.IsNullOrWhiteSpace(loginid))
            {
                if (Guid.TryParse(loginid, out Guid _Id))
                    return Redirect(await LoginUser(_Id));
            }

                return Page();
        }

        protected async Task<string> LoginUser(Guid _Id)
        {
            string uriresult = "/";
            using (MSNContext context = new MSNContext())
            {
                var attempts =context.LoginAttempt.Where(x => x.Id == _Id && !x.IsProcessed).ToList();
                foreach (LoginAttempt attempt in attempts)
                    try
                    {
                        var result = await _signInManager.PasswordSignInAsync(attempt.UserName, attempt.Password, false, false);
                        if (result.Succeeded)
                        {
                            uriresult =  attempt.ReturnUri;
                        }
                        else
                        if (result.IsLockedOut)
                        {
                            throw new Exception("User account locked out.");
                        }
                        else
                            throw new Exception("Invalid login attempt.");
                    }
                    catch (Exception ex)
                    {
                        ViewData["ErrorMsg"] = (ex.Message);
                    }
                    finally{
                        attempt.IsProcessed=true;
                        context.SaveChanges();
                    }
                return  uriresult;
            }
        }
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(Input.UserName, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
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
