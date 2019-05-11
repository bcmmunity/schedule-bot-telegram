using Microsoft.EntityFrameworkCore;
using System.Linq;
using TelegrammAspMvcDotNetCoreBot.Models;

namespace TelegrammAspMvcDotNetCoreBot.DB
{
    internal class SnUserDb
    {
        private readonly MyContext _db;
        private string SocialNetwork { get; }
        public SnUserDb(string socialNetwork)
        {
            _db = new DB().Connect();
            SocialNetwork = socialNetwork;
        }

        public void CreateUser(long userId)
        {
            if (CheckUser(userId)) return;
            SnUser user = new SnUser
            {
                SocialNetworkId = userId,
                SocialNetwork = SocialNetwork
            };
            _db.SnUsers.Add(user);
            _db.SaveChanges();
        }

        public bool CheckUser(long userId) //Проверка существования пользователя
        {


            if (_db.SnUsers.FirstOrDefault(n => n.SocialNetworkId == userId && n.SocialNetwork == SocialNetwork) != null)
                return true;
            return false;
        }













        public void RecreateUser(long userId)
        {
            if (!CheckUser(userId)) return;
            SnUser user = _db.SnUsers.FirstOrDefault(n => n.SocialNetworkId == userId && n.SocialNetwork == SocialNetwork);
            _db.SnUsers.Remove(user);
            _db.SaveChanges();
            CreateUser(userId);
        }

        public void EditUser(long userId, string type, string param)
        {
            SnUser user = _db.SnUsers.FirstOrDefault(n => n.SocialNetworkId == userId && n.SocialNetwork == SocialNetwork);
            switch (type)
            {
                case "university":
                    _db.SnUsers.FirstOrDefault(n => n.SocialNetworkId == userId && n.SocialNetwork == SocialNetwork).University = _db.Universities.FirstOrDefault(n => n.Name == param);
                    _db.SnUsers.Update(user);
                    break;
                case "facility":
                    Facility facility = _db.Facilities.FirstOrDefault(n => n.Name == param);
                    user.Facility = facility;
                    _db.SnUsers.Update(user);
                    break;
                case "course":
                    Course course = _db.Courses.FirstOrDefault(n => n.Name == param);
                    user.Course = course;
                    _db.SnUsers.Update(user);
                    break;
                case "group":
                    Group group = _db.Groups.FirstOrDefault(n => n.Name == param);
                    user.Group = group;
                    _db.SnUsers.Update(user);
                    break;
            }

            _db.SaveChanges();
        }
        public string CheckUserElements(long userId, string type)
        {

            switch (type)
            {
                case "university":
                    if (_db.SnUsers.Include(y => y.University).FirstOrDefault(n => n.SocialNetworkId == userId && n.SocialNetwork == SocialNetwork)?.University != null) return _db.SnUsers.Include(y => y.University).FirstOrDefault(n => n.SocialNetworkId == userId && n.SocialNetwork == SocialNetwork).University.Name;
                    break;
                case "facility":
                    if (_db.SnUsers.Include(y => y.Facility).FirstOrDefault(n => n.SocialNetworkId == userId && n.SocialNetwork == SocialNetwork)?.Facility != null) return _db.SnUsers.Include(y => y.Facility).FirstOrDefault(n => n.SocialNetworkId == userId && n.SocialNetwork == SocialNetwork).Facility.Name;
                    break;
                case "course":
                    if (_db.SnUsers.Include(y => y.Course).FirstOrDefault(n => n.SocialNetworkId == userId && n.SocialNetwork == SocialNetwork)?.Course != null) return _db.SnUsers.Include(y => y.Course).FirstOrDefault(n => n.SocialNetworkId == userId && n.SocialNetwork == SocialNetwork).Course.Name;
                    break;
                case "group":
                    if (_db.SnUsers.Include(y => y.Group).FirstOrDefault(n => n.SocialNetworkId == userId && n.SocialNetwork == SocialNetwork)?.Group != null) return _db.SnUsers.Include(y => y.Group).FirstOrDefault(n => n.SocialNetworkId == userId && n.SocialNetwork == SocialNetwork).Group.Name;
                    break;
            }
            return "";
        }

    }
}
