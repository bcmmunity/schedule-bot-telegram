using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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
                SnUser = _db.SnUsers.FirstOrDefault(u=> u.SocialNetworkId == chatId),
                UpdateType = updateType,
                MessageText = messageText,
                ErrorMessage = errorMessage,
                ErrorDateTime = errorDateTime
            };
            _db.ErrorLogs.Add(errorLog);
            _db.SaveChanges();
        }

        public List<long> GettingProblemUsers()
        {
            List<SnUser> users = _db.SnUsers.Where(n => n.SocialNetwork == "Telegram").ToList();

            List<long> problemUsersList = new List<long>();

            foreach (SnUser user in users)
            {
                problemUsersList.Add(user.SocialNetworkId);
            }

            return problemUsersList;
        }

    }
}
