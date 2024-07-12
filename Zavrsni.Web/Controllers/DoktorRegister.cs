// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Zavrsni.DAL;
using Zavrsni.Model;
using Zavrsni.Web.Models;
using Zavrsni.Web.Util;

namespace Zavrsni.Web.Areas.Identity.Pages.Account
{
    public class DoktorRegister : PageModel
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IUserStore<AppUser> _userStore;
        private readonly IUserEmailStore<AppUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly DataManagerDbContext _dbContext;
        private readonly IUrlHelper _urlHelper;

        public DoktorRegister(
            UserManager<AppUser> userManager,
            IUserStore<AppUser> userStore,
            SignInManager<AppUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            DataManagerDbContext _dbContext,
            IUrlHelper urlHelper)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            this._dbContext = _dbContext;
            _urlHelper = urlHelper;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            public string Ime { get; set; }
            [Required]
            public string Prezime { get; set; }
            [Required]
            public string Telefon { get; set; }
            [Required]
            public string Adresa { get; set; }
            [Required]
            public string Grad { get; set; }
            [Required]
            public string Drzava { get; set; }
            [Required]
            public string JMBG { get; set; }
            [Required]
            public string KorisnickoIme { get; set; }
            [Required]
            public DateTime DatumRodjenja { get; set; }
        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = CreateUser();

                Pacijent pacijent = new Pacijent
                {
                    Ime = Input.Ime,
                    Prezime = Input.Prezime,
                    Email = Input.Email,
                    Telefon = Input.Telefon,
                    Adresa = Input.Adresa,
                    Grad = Input.Grad,
                    Drzava = Input.Drzava,
                    JMBG = Input.JMBG,
                    KorisnickoIme = Input.KorisnickoIme,
                    DatumRodjenja = Input.DatumRodjenja
                };
                _dbContext.Pacijenti.Add(pacijent);
                _dbContext.SaveChanges();
                var pacijentID = _dbContext.Pacijenti.Where(pacijent => pacijent.KorisnickoIme == Input.KorisnickoIme).FirstOrDefault().PacijentID;
                user.Pacijent = pacijent;
                user.PacijentID = pacijentID;
                user.UserType = UserType.Pacijent;



                await _userStore.SetUserNameAsync(user, Input.KorisnickoIme, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);



                    string body = string.Empty;
                    using (StreamReader reader = new StreamReader("MailTemplate/AccountConfirmation.html"))
                    {
                        body = reader.ReadToEnd();
                    }
                    body = body.Replace("{ConfirmationLink}", callbackUrl);
                    body = body.Replace("{UserName}", user.UserName);
                    /*
                    bool IsSendEmail = EmailConfirmation.SendEmail(user.Email, "Confirm your account", body, true);
                    if (IsSendEmail)
                        return RedirectToAction("Login", "Account");
                    */


                    //await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                    //$"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                _dbContext.Pacijenti.Remove(_dbContext.Pacijenti.Where(pacijent => pacijent.KorisnickoIme == Input.KorisnickoIme).FirstOrDefault());
                _dbContext.SaveChanges();
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }


            // If we got this far, something failed, redisplay form
            return Page();
        }


        public async Task<IActionResult> KreirajNovogDoktora(Doktor doktor, DoktorKreiranje model, string returnUrl = null, string requestScheme = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = CreateUser();


                _dbContext.Doktori.Add(doktor);
                _dbContext.SaveChanges();
                var doktorID = _dbContext.Doktori.Where(dok => dok.KorisnickoIme == doktor.KorisnickoIme).FirstOrDefault().DoktorID;
                user.Doktor = doktor;
                user.DoktorID = doktorID;
                user.UserType = UserType.Doktor;



                await _userStore.SetUserNameAsync(user, user.Doktor.KorisnickoIme, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, user.Doktor.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    

                    var callbackUrl = _urlHelper.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: requestScheme);




                    string body = string.Empty;
                    using (StreamReader reader = new StreamReader("MailTemplate/AccountConfirmation.html"))
                    {
                        body = reader.ReadToEnd();
                    }
                    body = body.Replace("{ConfirmationLink}", callbackUrl);
                    body = body.Replace("{UserName}", user.UserName);
                    EmailConfirmation emailConfirmation = new EmailConfirmation();
                    await emailConfirmation.SendEmail(user.Email, "Confirm your account", body);

                    //bool IsSendEmail = EmailConfirmation.SendEmail(user.Email, "Confirm your account", body, true);
                    //if (IsSendEmail)
                        //return RedirectToAction("Login", "Account");


                    await _emailSender.SendEmailAsync(user.Doktor.Email, "Confirm your email",
                    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        //return RedirectToPage("RegisterConfirmation", new { email = user.Doktor.Email, returnUrl = returnUrl });
                        return RedirectToPage("/Identity/Account/RegisterConfirmation", new { email = user.Doktor.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                _dbContext.Doktori.Remove(_dbContext.Doktori.Where(dok => dok.KorisnickoIme == doktor.KorisnickoIme).FirstOrDefault());
                _dbContext.SaveChanges();
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }


            // If we got this far, something failed, redisplay form
            return Page();
        }

        private AppUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<AppUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(AppUser)}'. " +
                    $"Ensure that '{nameof(AppUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<AppUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<AppUser>)_userStore;
        }
    }
}
