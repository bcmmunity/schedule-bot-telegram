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

            //new UserDb().CheckUser(565656433);
            //new UserDb().EditUser(259762827, "facility", "ИТАСУ");
            string a = new UserDb().CheckUserElements(358243561, "facility");
            new UserDb().EditUser(358243561, "university", "мисис");

           bool b = new ScheduleController().IsFacilityExist(new UserDb().CheckUserElements(358243561, "university"), "ИТАСУ");

            //var optionsBuilder = new DbContextOptionsBuilder<MyContext>();
            //optionsBuilder.UseSqlServer("Server=studystat.ru;Database=u0641156_studystat;User Id=u0641156_studystat;Password=Stdstt1!;");
            ////optionsBuilder.UseSqlServer("Server=vladafon.ru;Database=schedule-bot;User Id=sa;Password=Pizza2135;");
            //MyContext Db = new MyContext(optionsBuilder.Options);
            //string a = Db.Users.FirstOrDefault(n => n.TelegramId == 358243561).University.Name;

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
