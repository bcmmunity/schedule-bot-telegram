using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TelegrammAspMvcDotNetCoreBot.DB;
using TelegrammAspMvcDotNetCoreBot.Logic.Parsers;
using TelegrammAspMvcDotNetCoreBot.Models;

namespace TelegrammAspMvcDotNetCoreBot.Controllers
{
    public class HomeController : Controller
    {
        private readonly MyContext db;

        public HomeController(MyContext context)
        {
            db = context;
        }

        public IActionResult Index()
        {
            LoggingDB log = new LoggingDB();
            ViewBag.Statistic = log.GetStatistic();
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
