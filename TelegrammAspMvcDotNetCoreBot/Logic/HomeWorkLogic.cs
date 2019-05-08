using System;
using TelegrammAspMvcDotNetCoreBot.DB;

namespace TelegrammAspMvcDotNetCoreBot.Logic
{
    public class HomeWorkLogic
    {
        public string SendHomework(long chatId, int daysfromtoday, string socialNetwork)
        {
            SnUserDb userDb = new SnUserDb(socialNetwork);

            DateTime now = DateTime.Now.Date;
            HomeWorkDB homeWork = new HomeWorkDB();
            string result = "Домашнее задание на ";
            if (daysfromtoday < 0)
                result += DateConverter(now.Subtract(new TimeSpan(-daysfromtoday, 0, 0, 0))) + "\n \n" + homeWork.GetHomeWork(userDb.CheckUserElements(chatId, "university"),
                              userDb.CheckUserElements(chatId, "facility"), userDb.CheckUserElements(chatId, "course"),
                              userDb.CheckUserElements(chatId, "group"), DateConverter(now.Subtract(new TimeSpan(-daysfromtoday, 0, 0, 0)))) +
                          "\nСегодня " + DateConverter(now);
            else if (daysfromtoday == 0)
            {
                result += DateConverter(now) + "\n \n" + homeWork.GetHomeWork(userDb.CheckUserElements(chatId, "university"),
                              userDb.CheckUserElements(chatId, "facility"), userDb.CheckUserElements(chatId, "course"),
                              userDb.CheckUserElements(chatId, "group"), DateConverter(now)) +
                          "\nСегодня " + DateConverter(now);
            }
            else if (daysfromtoday > 0)
            {
                result += DateConverter(now.AddDays(daysfromtoday)) + "\n \n" + homeWork.GetHomeWork(userDb.CheckUserElements(chatId, "university"),
                              userDb.CheckUserElements(chatId, "facility"), userDb.CheckUserElements(chatId, "course"),
                              userDb.CheckUserElements(chatId, "group"), DateConverter(now.AddDays(daysfromtoday))) +
                          "\nСегодня " + DateConverter(now);
            }
            return result;
        }
        private string DateConverter(DateTime date)
        {
            string shortdate = date.ToShortDateString();
            string month = shortdate.Split(".")[1];
            string day = shortdate.Split(".")[0];

            return day + "." + month;
        }
    }
}