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
            _db.ActivityLogs.Add(activityLog);
            _db.SaveChanges();
        }

        public string[] GetStatistic()
        {


            int allUsersCount = _db.SnUsers.Count();
            int vkUsersCount = _db.SnUsers.Count(n => n.SocialNetwork == "Vk");
            int telegramUsersCount = _db.SnUsers.Count(n => n.SocialNetwork == "Telegram");
            List<University> universities = _db.Universities.ToList();

            //int misisUsersCount = _db.SnUsers.Include(n => n.University).Count(n => n.University.Name == "НИТУ МИСиС");
            //int rhtuUsersCount = _db.SnUsers.Include(n => n.University).Count(n => n.University.Name == "РХТУ им.Менделеева");
            //int mgtuUsersCount = _db.SnUsers.Include(n => n.University).Count(n => n.University.Name == "");

            string[] result = new string[universities.Count+3];
            result[0] = "Количество пользователей бота: " + allUsersCount;
            result[1] = "Пользователи VK: " + vkUsersCount;
            result[2] = "Пользователи Telegram: " + telegramUsersCount;

            for (int i = 3; i <result.Length; i++)
            {
                result[i] = "Пользователи из "+universities[i-3].Name +": "+ _db.SnUsers.Include(n => n.University).Count(n => n.University.Name == universities[i - 3].Name);
            }

         //   string[] result = new[] { ("Количество пользователей бота: " + allUsersCount), ("Пользователи VK: " + vkUsersCount), ("Пользователи Telegram: " + telegramUsersCount), "Пользователи из НИТУ МИСиС: " + misisUsersCount, "Пользователи из РХТУ им.Менделеева: " + rhtuUsersCount };

            return result;
        }
    }
}
