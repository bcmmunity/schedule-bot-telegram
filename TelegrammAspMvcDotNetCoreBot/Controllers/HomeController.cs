using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TelegrammAspMvcDotNetCoreBot.Models;

namespace TelegrammAspMvcDotNetCoreBot.Controllers
{ 
	public class HomeController : Controller
	{
		private MyContext db;
		
		public HomeController(MyContext context)
		{
			db = context;
		}

		public IActionResult Index()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
