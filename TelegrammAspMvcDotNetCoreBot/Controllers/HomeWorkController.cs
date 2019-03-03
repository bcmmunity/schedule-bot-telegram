using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Office.Interop.Excel;
using TelegrammAspMvcDotNetCoreBot.Models;
using Group = TelegrammAspMvcDotNetCoreBot.Models.Group;

namespace TelegrammAspMvcDotNetCoreBot.Controllers
{
    public class HomeWorkController
    {
        static MyContext db;
        
        /// <summary>
        /// Инициализация базы данных
        /// </summary>
        public static void Unit()
        {
            var optionsBuilder = new DbContextOptionsBuilder<MyContext>();
            optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=test37;Trusted_Connection=True;");
            db = new MyContext(optionsBuilder.Options);
        }

        /// <summary>
        /// Добавление добашнего задания на определенный день
        /// </summary>
        public static void AddHomeWork(string university, string faculty, string course, string groupName, string date,
            string text)
        {
            if (ScheduleController.IsGroupExist(university, faculty, course, groupName))
            {
                Group gr = new Group();

                HomeWork d = new HomeWork();
                d.Date = date;
                d.HomeWorkText = text;

                University universitym = db.Universities.Where(m => m.Name == university).FirstOrDefault();
                Faculty facultym = db.Faculties.Where(l => l.University == universitym).Where(t => t.Name == faculty)
                    .FirstOrDefault();
                Course coursem = db.Courses.Where(o => o.Facultie == facultym).Where(t => t.Name == course)
                    .FirstOrDefault();
                gr = db.Groups.Where(g => g.Course == coursem).Where(t => t.Name == groupName).FirstOrDefault();
                d.Group = gr;

                db.HomeWorks.Add(d);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Добавление домашшнего задания на сегодня
        /// </summary>
        public static void AddHomeWorkToday(string university, string faculty, string course, string groupName,
            string text)
        {
            string date = DateTime.Now.ToString("d");
            AddHomeWork(university, faculty, course, groupName, date, text);
        }
        
        /// <summary>
        /// Добавление домашнего задания на завтра
        /// </summary>
        public static void AddHomeWorkTomorrow(string university, string faculty, string course, string groupName,
            string text)
        {
            string date = DateTime.Now.AddDays(1).ToString("d");
            AddHomeWork(university, faculty, course, groupName, date, text);
        }
        
        public static string GetHomeWork(string university, string faculty, string course, string groupName,
            string date)
        {
            if (ScheduleController.IsGroupExist(university, faculty, course, groupName))
            {
                HomeWork gr = new HomeWork();
                gr = (from kl in db.HomeWorks
                    where kl.Date == date && kl.Group.Name == groupName && kl.Group.Course.Name == course &&
                          kl.Group.Course.Facultie.Name == faculty &&
                          kl.Group.Course.Facultie.University.Name == university
                    select kl).FirstOrDefault();

                return gr.HomeWorkText;
            }

            return "";
        }

        
        public static string GetHomeWorkToday(string university, string faculty, string course, string groupName)
        {
            string date = DateTime.Now.ToString("d");
            return GetHomeWork(university, faculty, course, groupName, date);
        }

        public static string GetHomeWorkTomorrow(string university, string faculty, string course, string groupName)
        {
            string date = DateTime.Now.AddDays(1).ToString("d");
            return GetHomeWork(university, faculty, course, groupName, date);
        }

        public static string GetHomeWorkYesterday(string university, string faculty, string course, string groupName)
        {
            string date = DateTime.Now.AddDays(-1).ToString("d");
            return GetHomeWork(university, faculty, course, groupName, date);
        }

        public static void DeleteOldHomeWork()
        {
            DateTime now = DateTime.Now;
            DateTime twoWeeksAgo;
//            TimeSpan dif = now - twoWeeksAgo;
            foreach (var h in db.HomeWorks)
            {
                DateTime homeWorkOnDelete = DateTime.Parse(h.Date);
                TimeSpan dif = now - homeWorkOnDelete;
                if (dif.Days > 14)
                {
                    db.HomeWorks.Remove(h);
                }
            }

            db.SaveChanges();

        }
    }
}