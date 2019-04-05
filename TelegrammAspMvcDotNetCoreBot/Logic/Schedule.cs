﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TelegrammAspMvcDotNetCoreBot.DB;
using TelegrammAspMvcDotNetCoreBot.Logic.Parsers;
using TelegrammAspMvcDotNetCoreBot.Models;

namespace TelegrammAspMvcDotNetCoreBot.Logic
{
	public class Schedule
	{
		public void ScheduleUpdate()
		{
            //MisisParser parserToDB = new MisisParser();
            //parserToDB.ReadXls("ИТАСУ");
            //parserToDB.ReadXls("ИНМИН");
            //parserToDB.ReadXlsx("МГИ");
            //parserToDB.ReadXls("ЭУПП");
            //parserToDB.ReadXls("ЭкоТех");

            MendleevParser mendleevParser = new MendleevParser();
                mendleevParser.ReadXlsx("1 course");

        }

        public string ScheduleOnTheDay(long chatId, int weekNum, int day)
        {
            UserDb userDb = new UserDb();

            string result = "Расписание на " + ConvertWeekDayToRussian(day);
            if (weekNum == 1)
                result += " верхней недели\n \n";
            else
                result += " нижней недели\n \n";


            ScheduleDB schedule = new ScheduleDB();

            ScheduleDay scheduleDay = schedule.GetSchedule(userDb.CheckUserElements(chatId, "university"),
                userDb.CheckUserElements(chatId, "facility"), userDb.CheckUserElements(chatId, "course"),
                userDb.CheckUserElements(chatId, "group"), weekNum, day);

            List<Lesson> listPar = scheduleDay.Lesson.ToList();

            string lessons = "";
            foreach (Lesson item in listPar)
            {
                lessons += item.Number + " пара: " + ConvertToCorrectTimeFormat(item.Time) + "\n" + item.Name +
                          "\n" + item.Room + "\n\n";
            }

            if (lessons != "")
            {
                result += lessons;
                int weekNumNow = ((CultureInfo.CurrentCulture).Calendar.GetWeekOfYear(DateTime.Now,
                                      CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday) + 1) % 2 + 1;
                if (weekNumNow == 1)
                    result += "\nСейчас идет верхняя неделя";
                else
                    result += "\nСейчас идет нижняя неделя";
                return result;
            }

            return "Учебы нет";
        }

	    private string ConvertToCorrectTimeFormat(string time)
	    {
	        var firstTime = time.Split(" - ").First();
	        var secondTime = time.Split(" - ").Last();

	        return firstTime.Substring(0, firstTime.LastIndexOf(':')) + " - "
	                                                                  + secondTime.Substring(0, secondTime.LastIndexOf(':'));
	    }

        private string ConvertWeekDayToRussian(int weekDay)
	    {
	        switch (weekDay)
	        {
	            case 1:
	                return "понедельник";
	            case 2:
	                return "вторник";
	            case 3:
	                return "среду";
	            case 4:
	                return "четверг";
	            case 5:
	                return "пятницу";
	            case 6:
	                return "субботу";

	        }

	        return "";
	    }
        //static string GetResponse(string uri)
        //{
        //	StringBuilder sb = new StringBuilder();
        //	byte[] buf = new byte[8192];
        //	HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
        //	HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        //	Stream resStream = response.GetResponseStream();
        //	int count = 0;
        //	do
        //	{
        //		count = resStream.Read(buf, 0, buf.Length);
        //		if (count != 0)
        //		{
        //			sb.Append(Encoding.Default.GetString(buf, 0, count));
        //		}
        //	}
        //	while (count > 0);
        //	return sb.ToString();
        //}

        //static void Download(string url, string name)
        //{

        //	WebClient wc = new WebClient();

        //	if (url[url.Length - 1] == 'x')
        //	{
        //		wc.DownloadFile(url, name + ".xlsx");
        //		ExcelParserController.ReadXlsx(name);
        //	}
        //	else
        //	{
        //		wc.DownloadFile(url, name + ".xls");
        //		ExcelParserController.ReadXls(name);
        //	}

        //}
    }
}