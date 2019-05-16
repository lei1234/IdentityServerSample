using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServerCenter.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServerCenter.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        //private readonly RoleManager<AppRole> _roleManager;
        private readonly SignInManager<AppUser> _signManager;

        public AccountController(UserManager<AppUser> userManager,
            SignInManager<AppUser> signManager)
        {
            _userManager = userManager;
            _signManager = signManager;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {


            var claims = new List<Claim>
                    {
                        new Claim("userName","adamson"),
                        new Claim("phone","13270739236")
                    };

            //SignIn(new ClaimsPrincipal(new ClaimsIdentity(claims)), "zfct");

            //await HttpContext.SignInAsync("test", new ClaimsPrincipal(new ClaimsIdentity(claims)), new AuthenticationProperties
            //{
            //    ExpiresUtc=DateTime.UtcNow.AddMinutes(30),
            //    IsPersistent=false,
            //    AllowRefresh=false
            //});


            await AuthenticationManagerExtensions.SignInAsync(HttpContext, "10", new AuthenticationProperties
            {
                ExpiresUtc=DateTime.UtcNow.AddMinutes(30),
                IsPersistent=false,
                AllowRefresh=false
            });

            return Redirect(model.ReturnUrl);



            var user = await _userManager.FindByNameAsync(model.UserName);

            if (user == null)
            {
                ModelState.AddModelError(nameof(model.UserName), "用户名不存在");
            }
            else
            {
                var result =await  _signManager.CheckPasswordSignInAsync(user, model.PassWord, false);

                if (result.Succeeded)
                {
                    var claims1 = new List<Claim>
                    {
                        new Claim("userName","adamson"),
                        new Claim("phone","13270739236")
                    };

                    SignIn(new ClaimsPrincipal(new ClaimsIdentity(claims)), "zfct");
                }
                else
                {
                    ModelState.AddModelError(nameof(model.PassWord), "用户名密码不匹配");
                }
            }

            return View();
        }
    }

    public class LoginViewModel
    {
        public string UserName { get; set; }

        public string PassWord { get; set; }

        public string ReturnUrl { get; set; }
    }
}