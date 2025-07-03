using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Paying_Hub.Models;

namespace Paying_Hub.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
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

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
