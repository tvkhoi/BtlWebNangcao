// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using BtlWebNangCao.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using BtlWebNangCao.Models;
using Microsoft.EntityFrameworkCore;

namespace BtlWebNangCao.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly ApplicationDbContext _context;

        public LoginModel(SignInManager<ApplicationUser> signInManager, ILogger<LoginModel> logger, ApplicationDbContext context)
        {
            _signInManager = signInManager;
            _logger = logger;
            _context = context;
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
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string ErrorMessage { get; set; }

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
            /// 
            
            [Required]
            [Display(Name = "Nhập username hoặc email của bạn")]
            [StringLength(100, MinimumLength = 1, ErrorMessage = "Nhập đúng thông tin")]
            public string UserNameorEmail { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Display(Name = "Nhớ tài khoản?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                // Kiểm tra nếu nhập admin
                if (Input.UserNameorEmail.Equals("admin", StringComparison.OrdinalIgnoreCase) ||
                    Input.UserNameorEmail.Equals("admin@example.com", StringComparison.OrdinalIgnoreCase))
                {
                    var adminUser = await _context.NguoiDungs
                        .FirstOrDefaultAsync(u => u.TenDangNhap == "admin" || u.Email == "admin@example.com");

                    if (adminUser != null)
                    {
                        var passwordHasher = new PasswordHasher<NguoiDung>();
                        var verifyResult = passwordHasher.VerifyHashedPassword(adminUser, adminUser.MatKhau, Input.Password);

                        if (verifyResult == PasswordVerificationResult.Success)
                        {
                            _logger.LogInformation("Admin đăng nhập thành công.");

                            // Lưu session
                            HttpContext.Session.SetString("UserRole", "Admin");
                            HttpContext.Session.SetString("UserEmail", adminUser.Email);

                            return RedirectToAction("Index", "Admin"); // Điều hướng đến trang Admin
                        }
                    }

                    ModelState.AddModelError(string.Empty, "Thông tin đăng nhập không hợp lệ.");
                    return Page();
                }


                // Nếu không phải admin, đăng nhập bằng Identity
                var user = await _signInManager.UserManager.FindByEmailAsync(Input.UserNameorEmail);
                string username = user?.UserName ?? Input.UserNameorEmail;

                var result = await _signInManager.PasswordSignInAsync(username, Input.Password, Input.RememberMe, lockoutOnFailure: false);

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
                    ModelState.AddModelError(string.Empty, "Thông tin đăng nhập không hợp lệ.");
                    return Page();
                }
            }

            // Nếu có lỗi, hiển thị lại form đăng nhập
            return Page();
        }


    }
}
