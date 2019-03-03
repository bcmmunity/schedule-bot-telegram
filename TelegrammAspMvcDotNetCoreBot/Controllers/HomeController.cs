using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TelegrammAspMvcDotNetCoreBot.Models;
using Microsoft.EntityFrameworkCore;

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
			//ScheduleDay schedule = ScheduleController.GetSchedule("мисис", "ИТАСУ", "1", "БИВТ-18-1 1 подгруппа", 1, 3);

			//List<Lesson> listPar = schedule.Lesson;
			//sstring result = "";
			//foreach (Lesson item in ScheduleController.GetSchedule("мисис", "ИТАСУ", "1", "БИВТ-18-1 1 подгруппа", 1, 3).Lesson)
			//{
			//	result += item.Time + "\n" + item.Name + "\n" + item.Room + "\n\n";
			//}
			//ViewBag.n = result;

			return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
