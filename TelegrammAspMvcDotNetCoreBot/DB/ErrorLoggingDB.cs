using System;
using TelegrammAspMvcDotNetCoreBot.Models;

namespace TelegrammAspMvcDotNetCoreBot.DB
{
    public class ErrorLoggingDB
    {
        private readonly MyContext _db;

        public ErrorLoggingDB()
        {
            _db = new DB().Connect();
        }

        public void AddErrorInLog(long chatId, string updateType, string messageText, string errorMessage, DateTime errorDateTime)
        {
            ErrorLog errorLog = new ErrorLog
            {
                ChatId = chatId,
                UpdateType = updateType,
                MessageText = messageText,
                ErrorMessage = errorMessage,
                ErrorDateTime = errorDateTime
            };
            _db.ErrorLogs.Add(errorLog);
            _db.SaveChanges();
        }

    }
}
