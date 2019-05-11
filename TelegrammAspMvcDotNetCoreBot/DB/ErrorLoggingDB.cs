using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
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

        public List<long> GettingProblemUsers()
        {
            List<SnUser> users = _db.SnUsers.Include(n => n.Group).Where(n => n.Group == null && n.SocialNetwork == "Vk").ToList();

            List<long> problemUsersList = new List<long>();

            foreach (var user in users)
            {
                    problemUsersList.Add(user.SocialNetworkId);
            }

            return problemUsersList;
        }

    }
}
