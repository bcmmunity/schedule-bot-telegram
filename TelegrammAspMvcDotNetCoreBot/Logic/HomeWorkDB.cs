using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using TelegrammAspMvcDotNetCoreBot.Models;
using Group = TelegrammAspMvcDotNetCoreBot.Models.Group;

namespace TelegrammAspMvcDotNetCoreBot.Logic
{
    public class HomeWorkDB
    {
       public static MyContext Db;
        
        public HomeWorkDB()
        {
            var optionsBuilder = new DbContextOptionsBuilder<MyContext>();
             optionsBuilder.UseSqlServer("Server=localhost;Database=u0641156_studystat;User Id=u0641156_studystat;Password=Stdstt1!;");

//optionsBuilder.UseSqlServer("Server=studystat.ru;Database=u0641156_studystat;User Id=u0641156_studystat;Password=Stdstt1!;");
            Db = new MyContext(optionsBuilder.Options);
        }

        /// <summary>
        /// Добавление добашнего задания на определенный день
        /// </summary>
        public void AddHomeWork(string university, string faculty, string course, string groupName, string date,
            string text)
        {
            if (new ScheduleDB().IsGroupExist(university, faculty, course, groupName))
            {
                string mainGroup = groupName.Split(' ').First();

                University universitym = Db.Universities.FirstOrDefault(m => m.Name == university);
                Facility facultym = Db.Facilities.Where(l => l.University == universitym)
                    .FirstOrDefault(t => t.Name == faculty);
                Course coursem = Db.Courses.Where(o => o.Facility == facultym)
                    .FirstOrDefault(t => t.Name == course);

                List<Group> @group = new List<Group>();
                @group = Db.Groups.Where(g => g.Course == coursem).Where(t => t.Name.Contains(mainGroup)).ToList();

                foreach (var itemGroup in @group)
                {
                    HomeWork homeWork = new HomeWork();
                    homeWork.Date = date;
                    homeWork.HomeWorkText = text;

                    homeWork.Group = itemGroup;
                    Db.HomeWorks.Add(homeWork);
                    Db.SaveChanges();
                }
            }
        }

        ///// <summary>
        ///// Добавление домашшнего задания на сегодня
        ///// </summary>
        //public static void AddHomeWorkToday(string university, string faculty, string course, string groupName,
        //    string text)
        //{
        //    string date = DateTime.Now.ToString("d");
        //    AddHomeWork(university, faculty, course, groupName, date, text);
        //}
        
        ///// <summary>
        ///// Добавление домашнего задания на завтра
        ///// </summary>
        //public static void AddHomeWorkTomorrow(string university, string faculty, string course, string groupName,
        //    string text)
        //{
        //    string date = DateTime.Now.AddDays(1).ToString("d");
        //    AddHomeWork(university, faculty, course, groupName, date, text);
        //}
        
        public string GetHomeWork(string university, string faculty, string course, string groupName,
            string date)
        {
            string result = "";
            if (new ScheduleDB().IsGroupExist(university, faculty, course, groupName))
            {
                List<HomeWork> homeWorks = Db.HomeWorks.Where(m => m.Date == date).Where(n => n.Group.Name == groupName).ToList();


                //homeWork = (from kl in Db.HomeWorks
                //    where kl.Date == date && kl.Group.Name == groupName && kl.Group.Course.Name == course &&
                //          kl.Group.Course.Facility.Name == faculty &&
                //          kl.Group.Course.Facility.University.Name == university
                //    select kl).FirstOrDefault();
                if (homeWorks.Count == 0)
                    result = "Ничего не задано\n";
                else
                {
                    foreach (var item in homeWorks)
                    {
                        result += item.HomeWorkText+"\n";
                    }
                    
                }
            }

            return result;
        }

        
        //public static string GetHomeWorkToday(string university, string faculty, string course, string groupName)
        //{
        //    string date = DateTime.Now.ToString("d");
        //    return GetHomeWork(university, faculty, course, groupName, date);
        //}

        //public static string GetHomeWorkTomorrow(string university, string faculty, string course, string groupName)
        //{
        //    string date = DateTime.Now.AddDays(1).ToString("d");
        //    return GetHomeWork(university, faculty, course, groupName, date);
        //}

        //public static string GetHomeWorkYesterday(string university, string faculty, string course, string groupName)
        //{
        //    string date = DateTime.Now.AddDays(-1).ToString("d");
        //    return GetHomeWork(university, faculty, course, groupName, date);
        //}

        public void DeleteOldHomeWork()
        {
            DateTime now = DateTime.Now;
          //  DateTime twoWeeksAgo;
//            TimeSpan dif = now - twoWeeksAgo;
            foreach (var h in Db.HomeWorks)
            {
                DateTime homeWorkOnDelete = DateTime.Parse(h.Date);
                TimeSpan dif = now - homeWorkOnDelete;
                if (dif.Days > 14)
                {
                    Db.HomeWorks.Remove(h);
                }
            }

            Db.SaveChanges();

        }
    }
}