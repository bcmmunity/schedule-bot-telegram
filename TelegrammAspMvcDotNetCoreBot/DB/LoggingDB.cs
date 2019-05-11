using Microsoft.EntityFrameworkCore;
using System;
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
            StatisticLog statisticLog = new StatisticLog()
            {
                ChatId = chatId,
                MessageText = messageText,
                MessageDateTime = recordDateTime
            };
            _db.StatisticLogs.Add(statisticLog);
            _db.SaveChanges();
        }

        public string[] GetStatistic()
        {


            int allUsersCount = _db.SnUsers.Count();
            int vkUsersCount = _db.SnUsers.Count(n => n.SocialNetwork == "Vk");
            int telegramUsersCount = _db.SnUsers.Count(n => n.SocialNetwork == "Telegram");
            int misisUsersCount = _db.SnUsers.Include(n => n.University).Count(n => n.University.Name == "НИТУ МИСиС");
            int rhtuUsersCount = _db.SnUsers.Include(n => n.University).Count(n => n.University.Name == "РХТУ им.Менделеева");

            string[] result = new[] { ("Количество пользователей бота: " + allUsersCount), ("Пользователи VK: " + vkUsersCount), ("Пользователи Telegram: " + telegramUsersCount), "Пользователи из НИТУ МИСиС: " + misisUsersCount, "Пользователи из РХТУ им.Менделеева: " + rhtuUsersCount };

            return result;
        }
    }
}
