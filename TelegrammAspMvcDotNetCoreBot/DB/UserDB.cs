using System.Linq;
using Microsoft.EntityFrameworkCore;
using TelegrammAspMvcDotNetCoreBot.Models;

namespace TelegrammAspMvcDotNetCoreBot.DB
{
    class UserDb
    {
        private readonly MyContext _db;
        public UserDb()
        {
            _db = new DB().Connect();
        }

        public void CreateUser(long userId)
        {
          if (CheckUser(userId)) return;
            User user = new User
            {
                TelegramId = userId
            };
            _db.Users.Add(user);
            _db.SaveChanges();
        }

        public bool CheckUser(long userId) //Проверка существования пользователя
        {
        

            if (_db.Users.FirstOrDefault(n => n.TelegramId == userId) != null)
                return true;
            return false;
        }

        public void RecreateUser(long userId)
        {
            if (!CheckUser(userId)) return;
            User user = _db.Users.FirstOrDefault(n => n.TelegramId == userId);
            _db.Users.Remove(user);
            _db.SaveChanges();
            CreateUser(userId);
        }

        public void EditUser(long userId, string type, string param)
        {
            User user = _db.Users.FirstOrDefault(n => n.TelegramId == userId);
            switch (type)
            {
                case "university": 
                    _db.Users.FirstOrDefault(n => n.TelegramId == userId).University = _db.Universities.FirstOrDefault(n => n.Name == param);
                    _db.Users.Update(user);
                    break;
                case "facility":
                    Facility facility = _db.Facilities.FirstOrDefault(n => n.Name == param);
                    user.Facility = facility;
                    _db.Users.Update(user);
                    break;
                case "course":
                    Course course = _db.Courses.FirstOrDefault(n => n.Name == param);
                    user.Course = course;
                    _db.Users.Update(user);
                    break;
                case "group":
                    Group group = _db.Groups.FirstOrDefault(n => n.Name == param);
                    user.Group = group;
                    _db.Users.Update(user);
                    break;
            }

            _db.SaveChanges();
        }
        public string CheckUserElements(long userId, string type)
        {

            switch (type)
            {
                case "university":
                    if (_db.Users.Include(y => y.University).FirstOrDefault(n => n.TelegramId == userId)?.University != null) return _db.Users.Include(y => y.University).FirstOrDefault(n => n.TelegramId == userId).University.Name;
                    break;
                case "facility":
                    if (_db.Users.Include(y => y.Facility).FirstOrDefault(n => n.TelegramId == userId)?.Facility != null) return _db.Users.Include(y => y.Facility).FirstOrDefault(n => n.TelegramId == userId).Facility.Name;
                    break;
                case "course":
                    if (_db.Users.Include(y => y.Course).FirstOrDefault(n => n.TelegramId == userId)?.Course != null) return _db.Users.Include(y => y.Course).FirstOrDefault(n => n.TelegramId == userId).Course.Name;
                    break;
                case "group":
                    if (_db.Users.Include(y => y.Group).FirstOrDefault(n => n.TelegramId == userId)?.Group != null) return _db.Users.Include(y => y.Group).FirstOrDefault(n => n.TelegramId == userId).Group.Name;
                    break;
            }
            return "";
        }

    }
}
