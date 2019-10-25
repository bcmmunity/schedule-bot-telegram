using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using TelegrammAspMvcDotNetCoreBot.Models;

namespace TelegrammAspMvcDotNetCoreBot.DB
{
    public class LoggingDB
    {
        private readonly MyContext _db;

        public LoggingDB()
        {
            _db = new DB().Connect();
        }

        public void AddRecordInLog(long chatId, string messageText, DateTime recordDateTime)
        {
            ActivityLog activityLog = new ActivityLog()
            {
                SnUser = _db.SnUsers.FirstOrDefault(u=>u.SocialNetworkId == chatId),
                MessageText = messageText,
                MessageDateTime = recordDateTime
            };
            _db.SnUsers.FirstOrDefault(u => u.SocialNetworkId == chatId).LastActiveDate = DateTime.Now;
            _db.ActivityLogs.Add(activityLog);
            //_db.Update(_db.SnUsers.FirstOrDefault(u => u.SocialNetworkId == chatId));
            _db.SaveChanges();
        }

        public string[] GetStatistic()
        {


            /*int allUsersCount = _db.SnUsers.Count();
            int vkUsersCount = _db.SnUsers.Count(n => n.SocialNetwork == "Vk");
            int telegramUsersCount = _db.SnUsers.Count(n => n.SocialNetwork == "Telegram");
            List<University> universities = _db.Universities.ToList();

            //int misisUsersCount = _db.SnUsers.Include(n => n.University).Count(n => n.University.Name == "НИТУ МИСиС");
            //int rhtuUsersCount = _db.SnUsers.Include(n => n.University).Count(n => n.University.Name == "РХТУ им.Менделеева");
            //int mgtuUsersCount = _db.SnUsers.Include(n => n.University).Count(n => n.University.Name == "");

            string[] result = new string[universities.Count+7];
            //result[0] = "Количество пользователей бота: " + allUsersCount;
            //result[1] = "Пользователи VK: " + vkUsersCount;
            //result[2] = "Пользователи Telegram: " + telegramUsersCount;

            
         //   string[] result = new[] { ("Количество пользователей бота: " + allUsersCount), ("Пользователи VK: " + vkUsersCount), ("Пользователи Telegram: " + telegramUsersCount), "Пользователи из НИТУ МИСиС: " + misisUsersCount, "Пользователи из РХТУ им.Менделеева: " + rhtuUsersCount };
*/
            List<University> universities = _db.Universities.ToList();

            string[] result = new string[universities.Count + 6];

            int total;
            result[5] = DistictUsers(DateTime.Now.AddDays(-7), DateTime.Now, out total);

            result[1] = $"Total in last 7 days {total}";
            string trash = DistictUsers(DateTime.Now.AddDays(-7), DateTime.Now, out total);
            result[0] = $"Total in 3 days {total}";

            trash = DistictUsers(DateTime.Now.AddDays(-14), DateTime.Now.AddDays(-7), out total);
            result[2] = $"Total last week  {total}";

            trash = DistictUsers(DateTime.Now, DateTime.Now, out total, "all", "Telegram");
            result[3] = $"Total on Telegram today {total}";

            trash = DistictUsers(DateTime.Now, DateTime.Now, out total, "all", "Vk");
            result[4] = $"Total on VK today {total}";


            for (int i = 0; i < universities.Count; i++)
            {
                result[i + 6] = DistictUsers(DateTime.Now.AddDays(-7), DateTime.Now, out total, universities[i].Name);
            }

            return result;

        }

        public string DistictUsers(DateTime from, DateTime to, out int total, string universityFilter = "all", string networkFilter = "all")
        {
            string result = "";
            total = 0;
            for (DateTime i = from; i <= to; i = i.AddDays(1))
            {
                int numberOfUsers = _db.ActivityLogs.Where(p => (p.MessageDateTime.Date >= i.Date)&&(p.MessageDateTime.Date < i.Date.AddDays(1)) ).GroupBy(p => p.SnUser.SnUserId).Select(grp => grp.FirstOrDefault()).Where(n => (universityFilter == "all" ? true : n.SnUser.University.Name == universityFilter) && (networkFilter == "all" ? true : n.SnUser.SocialNetwork == networkFilter)).Count();
                total += numberOfUsers;
                result += $"{i.Date.ToShortDateString()} : {numberOfUsers} unique users \n";
            }
            return result;
        }

        
        public string InactiveUsers(DateTime to, string universityFilter = "all", string networkFilter = "all")
        {            
            List<SnUser> users = _db.SnUsers.Where(n => (n.LastActiveDate.Date <= to.Date) ).ToList();
            int userCount = users.Where(n => (universityFilter == "all" ? true : n.University.Name == universityFilter) && (networkFilter == "all" ? true : n.SocialNetwork == networkFilter)).Count();
            return  $"From {universityFilter} in {networkFilter} {userCount} users inactive since {to.Date.ToShortDateString()}";                  
        }


       /* public string NumberOfMessages(DateTime from, DateTime to, string universityFilter = "all", string networkFilter = "all")
        {
            List<SnUser> users = _db.SnUsers.Where(n => (n.LastActiveDate.Date <= to.Date)).ToList();
            int userCount = users.Where(n => (universityFilter == "all" ? true : n.University.Name == universityFilter) && (networkFilter == "all" ? true : n.SocialNetwork == networkFilter)).Count();
            return $"From {universityFilter} in {networkFilter} {userCount} users inactive since {to.Date.ToShortDateString()}";
        }*/
    }
}
