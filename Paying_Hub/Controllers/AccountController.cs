using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Paying_Hub.Interface;
using Paying_Hub.Models;
using Paying_Hub.Repository;
using System.Security.Claims;

namespace Paying_Hub.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogin _Login;
        //private object _account;

        public AccountController(ILogin Login)
        {
            _Login = Login;
        }



        [HttpPost]

        public async Task<IActionResult> AdminLogin(AdminLoginModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _Login.AdminLogin(model);
                if (user != null)
                {
                    HttpContext.Session.SetString("UserId", user.Id.ToString());
                    HttpContext.Session.SetString("Role", user.Role.ToLower());
                    HttpContext.Session.SetString("Name", user.FirstName + " " + user.LastName);
                    HttpContext.Session.SetString("UserName", user.Email);

                    // claims of Authentication

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Email,user.Email),
                        new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                        new Claim(ClaimTypes.Role,user.Role.ToLower())
                    };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    //Sign in with cookie
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    if (user.Role.ToLower() == "admin")
                        return RedirectToAction("DashBoard", "Home");
                    else
                        return RedirectToAction("Index", "Dashboard");
                }
                ViewBag.Message = "Invalid login credentials.";
            }

            return View();
        }



        [HttpPost]
        public async Task<IActionResult> UserLogin(UserLogin model)
        {


			if (!ModelState.IsValid)
				return View(model);

			var user = await _Login.ValidateUserLogin(model.LoginId, model.Password);

			if (user != null)
			{
				// Set session variables
				HttpContext.Session.SetString("MemberId", user.MemberId);
				HttpContext.Session.SetString("ReferralCode", user.ReferralCode ?? "");
				HttpContext.Session.SetString("UserName", user.Name ?? "");

				return RedirectToAction("UserDashBoard", "Home");
			}
			else
			{
				ViewBag.Message = "Invalid Login ID or Password.";
				return View(model);
			}
		}
    }
}
