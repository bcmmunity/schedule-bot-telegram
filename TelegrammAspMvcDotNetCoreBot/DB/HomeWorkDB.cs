using System;
using System.Collections.Generic;
using System.Linq;
using TelegrammAspMvcDotNetCoreBot.Models;
using Group = TelegrammAspMvcDotNetCoreBot.Models.Group;

namespace TelegrammAspMvcDotNetCoreBot.DB
{
    public class HomeWorkDB
    {
        private readonly MyContext _db;

        public HomeWorkDB()
        {
            _db = new DB().Connect();
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

                University universitym = _db.Universities.FirstOrDefault(m => m.Name == university);
                Facility facultym = _db.Facilities.Where(l => l.University == universitym)
                    .FirstOrDefault(t => t.Name == faculty);
                Course coursem = _db.Courses.Where(o => o.Facility == facultym)
                    .FirstOrDefault(t => t.Name == course);

                List<Group> @group = new List<Group>();
                @group = _db.Groups.Where(g => g.Course == coursem).Where(t => t.Name.Contains(mainGroup)).ToList();

                foreach (Group itemGroup in @group)
                {
                    HomeWork homeWork = new HomeWork
                    {
                        Date = date,
                        HomeWorkText = text,

                        Group = itemGroup
                    };
                    _db.HomeWorks.Add(homeWork);
                    _db.SaveChanges();
                }
            }
        }

        public string GetHomeWork(string university, string faculty, string course, string groupName,
            string date)
        {
            string result = "";
            if (new ScheduleDB().IsGroupExist(university, faculty, course, groupName))
            {
                List<HomeWork> homeWorks = _db.HomeWorks.Where(m => m.Date == date).Where(n => n.Group.Name == groupName).ToList();

                if (homeWorks.Count == 0)
                {
                    result = "Ничего не задано\n";
                }
                else
                {
                    foreach (HomeWork item in homeWorks)
                    {
                        result += item.HomeWorkText + "\n";
                    }

                }
            }

            return result;
        }

        public void DeleteOldHomeWork()
        {
            DateTime now = DateTime.Now;
            //  DateTime twoWeeksAgo;
            //            TimeSpan dif = now - twoWeeksAgo;
            foreach (HomeWork h in _db.HomeWorks)
            {
                DateTime homeWorkOnDelete = DateTime.Parse(h.Date);
                TimeSpan dif = now - homeWorkOnDelete;
                if (dif.Days > 14)
                {
                    _db.HomeWorks.Remove(h);
                }
            }

            _db.SaveChanges();

        }
    }
}