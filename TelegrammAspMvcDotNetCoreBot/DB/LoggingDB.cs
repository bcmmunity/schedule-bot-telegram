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
            SnUser user = _db.SnUsers.FirstOrDefault(u => u.SocialNetworkId == chatId);
            if (user != null)
            {
                ActivityLog activityLog = new ActivityLog()
                {
                    SnUser = _db.SnUsers.FirstOrDefault(u => u.SocialNetworkId == chatId),
                    MessageText = messageText,
                    MessageDateTime = recordDateTime
                };
            

            user.LastActiveDate = DateTime.Now;
            _db.ActivityLogs.Add(activityLog);
            //_db.Update(_db.SnUsers.FirstOrDefault(u => u.SocialNetworkId == chatId));
            _db.SaveChanges();
            }
        }

        public string[] GetDetails()
        {
            /*int allUsersCount = _db.SnUsers.Count();
           
*/
            List<University> universities = _db.Universities.ToList();
            string[] result = new string[universities.Count + 6 + 8];
            int total;
            result[5] = "Week listing";
            string[] week = DistictUsers(DateTime.Now.AddDays(-7), DateTime.Now, out total);
            for (int i = 0; i < week.Length; i++)
            {
                result[i + 6] = week[i];
            }
            result[1] = $"Total in last 7 days {total}";
            string[] trash = DistictUsers(DateTime.Now.AddDays(-3), DateTime.Now, out total);
            result[0] = $"Total in 3 days {total}";
            trash = DistictUsers(DateTime.Now.AddDays(-14), DateTime.Now.AddDays(-7), out total);
            result[2] = $"Total last week  {total}";
            trash = DistictUsers(DateTime.Now, DateTime.Now, out total, "all", "Telegram");
            result[3] = $"Total on Telegram today {total}";
            trash = DistictUsers(DateTime.Now, DateTime.Now, out total, "all", "Vk");
            result[4] = $"Total on VK today {total}";
            for (int i = 0; i < universities.Count; i++)
            {
                result[i + 6 + 8] = DistictUsers(DateTime.Now, DateTime.Now, out total, universities[i].Name)[0] + " " + universities[i].Name;
            }
            return result;
        }


        public string[] GetStatistic()
        {

            int allUsersCount = _db.SnUsers.Count();
            int vkUsersCount = _db.SnUsers.Count(n => n.SocialNetwork == "Vk");
            int telegramUsersCount = _db.SnUsers.Count(n => n.SocialNetwork == "Telegram");
            List<University> universities = _db.Universities.ToList();

            string[] result = new string[universities.Count + 3];
            result[0] = "Количество пользователей бота: " + allUsersCount;
            result[1] = "Пользователи VK: " + vkUsersCount;
            result[2] = "Пользователи Telegram: " + telegramUsersCount;

            for (int i = 3; i < result.Length; i++)
            {
                result[i] = "Пользователи из " + universities[i - 3].Name + ": " + _db.SnUsers.Include(n => n.University).Count(n => n.University.Name == universities[i - 3].Name);
            }
            return result;

        }

        public string[] DistictUsers(DateTime from, DateTime to, out int total, string universityFilter = "all", string networkFilter = "all")
        {
            string[] result = new string[(int)(to -from).TotalDays + 1];
            total = 0;
            int count = 0;
            for (DateTime i = from; i <= to; i = i.AddDays(1))
            {
                try
                {
                    var allUsers = _db.ActivityLogs.Include(u => u.SnUser).Where(p =>
                        (p.MessageDateTime.Date >= i.Date) && (p.MessageDateTime.Date < i.Date.AddDays(1))).ToList();
                    var validUsers = new List<ActivityLog>();
                    foreach (var user in allUsers)
                    {
                        if (user.SnUser != null)
                            validUsers.Add(user);
                    }

                    var Users = validUsers.GroupBy(p => p.SnUser.SnUserId).
                        Select(grp => grp.FirstOrDefault()).ToList();

                    if (universityFilter != "all")
                        Users = Users.Where(n => n != null && n.SnUser.University.Name == universityFilter).ToList();
                    if (networkFilter != "all")
                        Users = Users.Where(n => n != null && n.SnUser.SocialNetwork == networkFilter).ToList();

                    int numberOfUsers = Users.Count;
                    total += numberOfUsers;
                    result[count++] = $"{i.Date.ToShortDateString()} : {numberOfUsers}  Users ";
                }
                catch (Exception e)
                {
                    result[count++] = $"{i.Date.ToShortDateString()} : error ocured {e.Message}  ";
                }
            }
            return result;
        }


        public string[] MessagesCount(DateTime from, DateTime to, out int total, string universityFilter = "all", string networkFilter = "all")
        {
            string[] result = new string[(int)(to - from).TotalDays + 1];
            total = 0;
            int count = 0;
            for (DateTime i = from; i <= to; i = i.AddDays(1))
            {
                try
                {
                    var Messages = _db.ActivityLogs.
                        Where(p => (p.MessageDateTime.Date >= i.Date) && (p.MessageDateTime.Date < i.Date.AddDays(1)));

                    //if (universityFilter != "all") Users = Users.Where(n => n.SnUser.University.Name == universityFilter);
                    if (networkFilter != "all") Messages = Messages.Where(n => n.SnUser.SocialNetwork == networkFilter);

                    int numberOfMessages = Messages.Count();
                    total += numberOfMessages;
                    result[count++] = $"{i.Date.ToShortDateString()} : {numberOfMessages}  messages ";
                }
                catch (Exception e)
                {
                    result[count++] = $"{i.Date.ToShortDateString()} : error ocured {e.Message}  ";
                }
            }
            return result;
        }


        public string InactiveUsers(DateTime to, string universityFilter = "all", string networkFilter = "all")
        {            
            List<SnUser> users = _db.SnUsers.Where(n => (n.LastActiveDate.Date <= to.Date) ).ToList();
            int userCount = users.Where(n => (universityFilter == "all" ? true : n.University.Name == universityFilter) && (networkFilter == "all" ? true : n.SocialNetwork == networkFilter)).Count();
            return  $"From {universityFilter} in {networkFilter} {userCount} users inactive since {to.Date.ToShortDateString()}";                  
        }
       
    }
}
