using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using TelegrammAspMvcDotNetCoreBot.Models;

namespace TelegrammAspMvcDotNetCoreBot.Controllers
{
    class UserDb
    {
        public UserDb()
        {
            var optionsBuilder = new DbContextOptionsBuilder<MyContext>();
            optionsBuilder.UseSqlServer("Server=studystat.ru;Database=u0641156_studystat;User Id=u0641156_studystat;Password=Stdstt1!;");
            //optionsBuilder.UseSqlServer("Server=vladafon.ru;Database=schedule-bot;User Id=sa;Password=Pizza2135;");
            Db = new MyContext(optionsBuilder.Options);
        }

        private static MyContext Db;

        public void CreateUser(long UserId)
        {
          if (CheckUser(UserId)) return;
            User user = new User
            {
                TelegramId = UserId
            };
            Db.Users.Add(user);
            Db.SaveChanges();
        }

        public bool CheckUser(long UserId) //Проверка существования пользователя
        {
        

            if (Db.Users.FirstOrDefault(n => n.TelegramId == UserId) != null)
                return true;
            return false;
        }

        public void RecreateUser(long UserId)
        {
            if (!CheckUser(UserId)) return;
            User user = Db.Users.FirstOrDefault(n => n.TelegramId == UserId);
            Db.Users.Remove(user);
            Db.SaveChanges();
            CreateUser(UserId);
        }

        public void EditUser(long UserId, string type, string param)
        {
            User user = Db.Users.FirstOrDefault(n => n.TelegramId == UserId);
            switch (type)
            {
                case "university":
                    University university = Db.Universities.FirstOrDefault(n => n.Name == param);
                    user.University = university;
                    Db.Users.Update(user);
                    break;
                case "facility":
                    Facility facility = Db.Facilities.FirstOrDefault(n => n.Name == param);
                    user.Facility = facility;
                    Db.Users.Update(user);
                    break;
                case "course":
                    Course course = Db.Courses.FirstOrDefault(n => n.Name == param);
                    user.Course = course;
                    Db.Users.Update(user);
                    break;
                case "group":
                    Group group = Db.Groups.FirstOrDefault(n => n.Name == param);
                    user.Group = group;
                    Db.Users.Update(user);
                    break;
            }

            Db.SaveChanges();
        }
        public string CheckUserElements(long UserId, string type)
        {
            User user = Db.Users.FirstOrDefault(n => n.TelegramId == UserId);

            switch (type)
            {
                case "university":
                    if (user.University != null) return user.University.Name;
                    break;
                case "facility":
                    if (user.Facility != null) return user.Facility.Name;
                    break;
                case "course":
                    if (user.Course != null) return user.Course.Name;
                    break;
                case "group":
                    if (user.Group != null) return user.Group.Name;
                    break;
            }
            return "";
        }
    }
}
