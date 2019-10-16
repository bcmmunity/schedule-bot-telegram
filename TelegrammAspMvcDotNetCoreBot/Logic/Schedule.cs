using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TelegrammAspMvcDotNetCoreBot.DB;
using TelegrammAspMvcDotNetCoreBot.Models;

namespace TelegrammAspMvcDotNetCoreBot.Logic
{
    public class Schedule
    {

        public string ScheduleOnTheDay(long chatId, int weekNum, int day, string socialNetwork)
        {
          //  int realWeekNum = GetWeekNum(chatId, weekNum, socialNetwork);
            if (day == 7)
                return "Учебы нет";

            SnUserDb userDb = new SnUserDb(socialNetwork);

            string result = "Расписание на " + ConvertWeekDayToRussian(day);
            result += ", "+ GetWeekName(chatId, weekNum, socialNetwork) + "\n \n";



            ScheduleDB schedule = new ScheduleDB();

            ScheduleDay scheduleDay = schedule.GetSchedule(userDb.CheckUserElements(chatId, "university"),
                userDb.CheckUserElements(chatId, "facility"), userDb.CheckUserElements(chatId, "course"),
                userDb.CheckUserElements(chatId, "group"), weekNum, day);

            List<Lesson> listPar = scheduleDay.Lesson.ToList();
            LessonIComparer<Lesson> comparer = new LessonIComparer<Lesson>();
            listPar.Sort(comparer);

            string lessons = "";
            foreach (Lesson item in listPar)
            {
                string teacher = schedule.GeTeacher(item);
                lessons += item.Number + " пара: " + ConvertToCorrectTimeFormat(item.Time) + "\n" + item.Name;
                if (!string.IsNullOrEmpty(item.Type))
                    lessons += "\n" + item.Type;
                if (!string.IsNullOrEmpty(item.Room))
                    lessons += "\n" + item.Room;
                if (!string.IsNullOrEmpty(teacher))
                    lessons += "\n" + teacher;
                lessons += "\n\n";
            }

            if (lessons != "")
            {
                result += lessons;
                int weekNumNow = GetWeekNum(chatId,(((CultureInfo.CurrentCulture).Calendar.GetWeekOfYear(DateTime.Now,
                                                 CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday)) % 2 + 1),socialNetwork);
                if (weekNumNow == 1)
                    result += "\nСейчас идет " + GetWeekName(chatId, weekNumNow, socialNetwork);
                else
                    result += "\nСейчас идет " + GetWeekName(chatId, weekNumNow, socialNetwork);
                return result;
            }

            return "Учебы нет";
        }

        public string TeacherScheduleOnTheDay(long chatId, string teacherName, int weekNum, int day, string socialNetwork)
        {
          //  int realWeekNum = GetWeekNum(chatId, weekNum, socialNetwork);
            if (day == 7)
                return "Пар нет";

            SnUserDb userDb = new SnUserDb(socialNetwork);

            string result = "Расписание на " + ConvertWeekDayToRussian(day);
            result += ", "+ GetWeekName(chatId, weekNum, socialNetwork) + "\n \n";



            ScheduleDB schedule = new ScheduleDB();

            ScheduleDay scheduleDay = schedule.GetTeacherSchedule(teacherName, weekNum, day);

            List<Lesson> listPar = scheduleDay.Lesson.ToList();
            LessonIComparer<Lesson> comparer = new LessonIComparer<Lesson>();
            listPar.Sort(comparer);

            string lessons = "";
            foreach (Lesson item in listPar)
            {
                string teacher = schedule.GeTeacher(item);
                 lessons += item.Number + " пара: " + ConvertToCorrectTimeFormat(item.Time) + "\n" + item.Name;
                if (!string.IsNullOrEmpty(item.Type))
                    lessons += "\n" + item.Type;
                if (!string.IsNullOrEmpty(item.Room))
                    lessons += "\n" + item.Room;
                if (!string.IsNullOrEmpty(teacher))
                    lessons += "\n" + teacher;
                lessons += "\n\n";
            }

            if (lessons != "")
            {
                result += lessons;
                int weekNumNow = GetWeekNum(chatId,(((CultureInfo.CurrentCulture).Calendar.GetWeekOfYear(DateTime.Now,
                                                 CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday)) % 2 + 1),socialNetwork);
                if (weekNumNow == 1)
                    result += "\nСейчас идет " + GetWeekName(chatId, weekNumNow, socialNetwork);
                else
                    result += "\nСейчас идет " + GetWeekName(chatId, weekNumNow, socialNetwork);
                return result;
            }

            return "Пар нет";
        }

        private string ConvertToCorrectTimeFormat(string time)
        {
            string firstTime = time.Split(" - ").First();
            string secondTime = time.Split(" - ").Last();

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

        public string GetWeekName(long id, int weekNum, string socialNetwork)
        {
            SnUserDb userDb = new SnUserDb(socialNetwork);
            string university = userDb.CheckUserElements(id, "university");

            switch (university)
            {
                case "НИТУ МИСиС":
                {
                    if (weekNum == 1)
                        return "верхняя неделя";
                    else
                        return "нижняя неделя";

                }

                case "РХТУ им.Менделеева":
                {
                    if (weekNum == 1)
                        return "1 неделя";
                    else
                        return "2 неделя";
                }
                case "МГТУ Им. Н.Э.Баумана":
                {
                    if (weekNum == 1)
                        return "числитель";
                    else
                        return "знаменатель";
                }

                default:
                {
                    if (weekNum == 1)
                        return "1 неделя";
                    else
                        return "2 неделя";
                }
            }
            
        }

        private int GetWeekNum(long id, int weekNum, string socialNetwork)
        {
            SnUserDb userDb = new SnUserDb(socialNetwork);
            string university = userDb.CheckUserElements(id, "university");

            switch (university)
            {
                case "НИТУ МИСиС":
                {
                    if (weekNum == 1)
                        return 1;
                    else
                        return 2;

                }

                case "РХТУ им.Менделеева":
                {
                    if (weekNum == 1)
                        return 1;
                    else
                        return 2;
                }

                default:
                {
                    if (weekNum == 1)
                        return 1;
                    else
                        return 2;
                }
            }
        }
    }
}
