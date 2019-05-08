using System;
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
    }
}
