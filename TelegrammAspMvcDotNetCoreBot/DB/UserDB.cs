using System.Linq;
using Microsoft.EntityFrameworkCore;
using TelegrammAspMvcDotNetCoreBot.Models;

namespace TelegrammAspMvcDotNetCoreBot.DB
{
    class UserDb
    {
        public UserDb()
        {
            Db = new DB().Connect();
        }

        private static MyContext Db;

        public void CreateUser(long userId)
        {
          if (CheckUser(userId)) return;
            User user = new User
            {
                TelegramId = userId
            };
            Db.Users.Add(user);
            Db.SaveChanges();
        }

        public bool CheckUser(long userId) //Проверка существования пользователя
        {
        

            if (Db.Users.FirstOrDefault(n => n.TelegramId == userId) != null)
                return true;
            return false;
        }

        public void RecreateUser(long userId)
        {
            if (!CheckUser(userId)) return;
            User user = Db.Users.FirstOrDefault(n => n.TelegramId == userId);
            Db.Users.Remove(user);
            Db.SaveChanges();
            CreateUser(userId);
        }

        public void EditUser(long userId, string type, string param)
        {
            User user = Db.Users.FirstOrDefault(n => n.TelegramId == userId);
            switch (type)
            {
                case "university": 
                    Db.Users.FirstOrDefault(n => n.TelegramId == userId).University = Db.Universities.FirstOrDefault(n => n.Name == param);
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
        public string CheckUserElements(long userId, string type)
        {

            switch (type)
            {
                case "university":
                    if (Db.Users.Include(y => y.University).FirstOrDefault(n => n.TelegramId == userId)?.University != null) return Db.Users.Include(y => y.University).FirstOrDefault(n => n.TelegramId == userId).University.Name;
                    break;
                case "facility":
                    if (Db.Users.Include(y => y.Facility).FirstOrDefault(n => n.TelegramId == userId)?.Facility != null) return Db.Users.Include(y => y.Facility).FirstOrDefault(n => n.TelegramId == userId).Facility.Name;
                    break;
                case "course":
                    if (Db.Users.Include(y => y.Course).FirstOrDefault(n => n.TelegramId == userId)?.Course != null) return Db.Users.Include(y => y.Course).FirstOrDefault(n => n.TelegramId == userId).Course.Name;
                    break;
                case "group":
                    if (Db.Users.Include(y => y.Group).FirstOrDefault(n => n.TelegramId == userId)?.Group != null) return Db.Users.Include(y => y.Group).FirstOrDefault(n => n.TelegramId == userId).Group.Name;
                    break;
            }
            return "";
        }

    }
}
