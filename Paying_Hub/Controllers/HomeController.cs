using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Paying_Hub.Models;
using Paying_Hub.Interface;

namespace Paying_Hub.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;
        private readonly ILogin _Login;
        private readonly IUser _User;

        public HomeController(ILogger<HomeController> logger, ILogin Login, AppDbContext context, IUser User)
        {
            _logger = logger;
            _Login = Login;
            _context = context;
            _User = User;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult DashBoard()
        {
            return View();
        }
		public IActionResult UserLoginPage()
		{
			return View();
		}

		public IActionResult UserDashBoard()
		{
			return View();
		}
		public IActionResult Home()
		{
			return View();
		}
		public IActionResult About()
		{
			return View();
		}
		public IActionResult Market()
		{
			return View();
		}
		public IActionResult Benefits()
		{
			return View();
		}
		public IActionResult FAQ()
		{
			return View();
		}
		public IActionResult Contact()
		{
			return View();
		}
		public IActionResult Login()
		{
			return View();
		}
		public IActionResult SignUp()
		{
            var states = _context.States.ToList();
            ViewBag.States = states;
            return View();
		}

        [HttpPost]
        public async Task<IActionResult> SignUp(MemberMaster model)
        {
            try
            {

                if (!string.IsNullOrEmpty(model.SponserId))
                {
                    bool isSponserValid = _User.DoesSponserIdExist(model.SponserId);
                    if (!isSponserValid)
                    {
                        ModelState.AddModelError("SponserId", "Sponsor ID does not exist. Please check and try again.");
                        return View(model);
                    }
                }

                string refcode = GenerateUniqueReferralCode();
                model.ReferralCode = refcode;

                string result = _User.AddMember(model);

                if (result.StartsWith("SQL Error"))
                {
                    var states = _context.States.ToList();
                    ViewBag.States = states;
                    TempData["Error"] = result;
                    return View(model);
                }

                TempData["Success"] = "Member Registered Successfully!";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Something went wrong. Please try again later.");
                return View(model);
            }
        }

        private string GenerateUniqueReferralCode()
        {
            string referralCode;
            bool isUnique = false;

            do
            {

                Random rnd = new Random();
                int randomNumber = rnd.Next(1000, 9999);


                int totalMembers = _User.GetTotalMemberCount();
                int newMemberNumber = totalMembers + 1;


                string lastTwoDigits = (newMemberNumber % 100).ToString("D2");

                referralCode = "PAY" + randomNumber + lastTwoDigits;

                isUnique = !_User.DoesReferralCodeExist(referralCode);

            } while (!isUnique);

            return referralCode;
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserLogin model)
        {


            if (!ModelState.IsValid)
                return View(model);

            var user = await _Login.ValidateUserLogin(model.LoginId, model.Password);
            if (!model.RememberMe)
            {
                ViewBag.RememberMeError = "You must check the Remember Me box.";
                return View(model);
            }

            if (user != null)
            {
                // Set session variables
                HttpContext.Session.SetString("SponserId", user.SponserId);
                HttpContext.Session.SetString("ReferralCode", user.ReferralCode ?? "");
                HttpContext.Session.SetString("UserName", user.Name ?? "");

                return RedirectToAction("UserDashBoard", "Home");
            }
            else
            {
                TempData["Message"] = "Invalid Login ID or Password.";
                return RedirectToAction("Login", "Home");

            }
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
