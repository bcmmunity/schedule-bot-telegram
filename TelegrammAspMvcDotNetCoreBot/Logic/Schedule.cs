using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using TelegrammAspMvcDotNetCoreBot.DB;
using TelegrammAspMvcDotNetCoreBot.Models;

namespace TelegrammAspMvcDotNetCoreBot.Logic
{
    public class Schedule
    {

        public string ScheduleOnTheDay(long chatId, int weekNum, int day, string socialNetwork,bool buttons = false)
        {
          //  int realWeekNum = GetWeekNum(chatId, weekNum, socialNetwork);
            if (day == 7)
                return "Учебы нет";

            SnUserDb userDb = new SnUserDb(socialNetwork);


            ScheduleDB schedule = new ScheduleDB();
            byte scheduleType = userDb.GetUserScheduleType(chatId);
            var weekNumUse = 0;
            int septemberTheFirstWeek = 35; //35 - неделя на которой было 2 сентября
            if (buttons == false)
            {
                

                if (scheduleType == 1)
                    weekNumUse = 1;
                else if (scheduleType != 0 && scheduleType != 2)
                {
                    int weekNumNow = ((CultureInfo.CurrentCulture).Calendar.GetWeekOfYear(DateTime.Now,
                                         CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday)) % scheduleType + 1;
                    weekNumUse = weekNum == 1 ? weekNumNow : (weekNumNow == scheduleType ? 1 : weekNumNow + 1);

                }
                else if (scheduleType == 2)
                {
                    weekNumUse = weekNum;
                }
                else
                {
                    int weekNumNow = ((CultureInfo.CurrentCulture).Calendar.GetWeekOfYear(DateTime.Now,
                                         CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday)) - septemberTheFirstWeek;

                    weekNumUse = weekNum == 1 ? weekNumNow : weekNumNow + 1;
                }
            }
            else
                weekNumUse = weekNum;


            string result = "Расписание на " + ConvertWeekDayToRussian(day);
            result += ", " + GetWeekName(chatId, weekNumUse, socialNetwork) + "\n \n";

            List<Lesson> listPar = schedule.GetSchedule(userDb.CheckUserElements(chatId, "university"),
                userDb.CheckUserElements(chatId, "facility"), userDb.CheckUserElements(chatId, "course"),
                userDb.CheckUserElements(chatId, "group"), weekNumUse, day);
            LessonIComparer<Lesson> comparer = new LessonIComparer<Lesson>();
            listPar.Sort(comparer);

            string lessons = "";
            foreach (Lesson item in listPar)
            {
                string teacher = item.TeachersNames;
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
                int weekNumNow = 0;
                if (scheduleType != 0)
                    weekNumNow = GetWeekNum(chatId, ((CultureInfo.CurrentCulture).Calendar.GetWeekOfYear(DateTime.Now,
                                                        CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday)) % scheduleType + 1, socialNetwork);
                else
                    weekNumNow = GetWeekNum(chatId, ((CultureInfo.CurrentCulture).Calendar.GetWeekOfYear(DateTime.Now,
                                                        CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday)) - septemberTheFirstWeek, socialNetwork);

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

 
            ScheduleDB schedule = new ScheduleDB();

            byte scheduleType = userDb.GetUserScheduleType(chatId);
            int septemberTheFirstWeek = 35; //35 - неделя на которой было 2 сентября

            if (scheduleType == 1)
                weekNum = 1;
            else if (scheduleType != 0 && scheduleType != 2)
            {
                int weekNumNow = ((CultureInfo.CurrentCulture).Calendar.GetWeekOfYear(DateTime.Now,
                                     CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday)) % scheduleType + 1;
                weekNum = weekNum == 1 ? weekNumNow : (weekNumNow == scheduleType ? 1 : weekNumNow + 1);

            }
            else if (scheduleType != 2)
            {
                int weekNumNow = ((CultureInfo.CurrentCulture).Calendar.GetWeekOfYear(DateTime.Now,
                                     CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday)) - septemberTheFirstWeek;

                weekNum = weekNum == 1 ? weekNumNow : weekNumNow + 1;
            }

            string result = "Расписание на " + ConvertWeekDayToRussian(day);
            result += ", " + GetWeekName(chatId, weekNum, socialNetwork) + "\n \n";


            List<Lesson> listPar = schedule.GetTeacherSchedule(teacherName, weekNum, day);
            LessonIComparer<Lesson> comparer = new LessonIComparer<Lesson>();
            listPar.Sort(comparer);

            string lessons = "";
            foreach (Lesson item in listPar)
            {
                string teacher = item.TeachersNames;
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
                int weekNumNow = 0;
                if (scheduleType != 0)
                    weekNumNow = GetWeekNum(chatId, ((CultureInfo.CurrentCulture).Calendar.GetWeekOfYear(DateTime.Now,
                                  CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday)) % scheduleType + 1, socialNetwork);
                else
                    weekNumNow = GetWeekNum(chatId, ((CultureInfo.CurrentCulture).Calendar.GetWeekOfYear(DateTime.Now,
                                  CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday)) - septemberTheFirstWeek, socialNetwork);
                result += "\nСейчас идет " + GetWeekName(chatId, weekNumNow, socialNetwork);
                return result;
            }

            return "Пар нет";
        }

        public string SimpleScheduleOnTheDay(long chatId, int weekNum, int day, string socialNetwork, bool buttons = false)
        {
            //  int realWeekNum = GetWeekNum(chatId, weekNum, socialNetwork);
            if (day == 7)
                return "Пар нет";

            SnUserDb userDb = new SnUserDb(socialNetwork);


            ScheduleDB schedule = new ScheduleDB();
            byte scheduleType = userDb.GetUserScheduleType(chatId);
            var weekNumUse = 0;
            int septemberTheFirstWeek = 35; //35 - неделя на которой было 2 сентября

            if (scheduleType == 1)
                    weekNumUse = 1;
                else if (scheduleType != 0 && scheduleType != 2)
                {
                    int weekNumNow = ((CultureInfo.CurrentCulture).Calendar.GetWeekOfYear(DateTime.Now,
                                         CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday)) % scheduleType + 1;
                    weekNumUse = weekNum == 1 ? weekNumNow : (weekNumNow == scheduleType ? 1 : weekNumNow + 1);

                }
                else if (scheduleType == 2)
                {
                    weekNumUse = weekNum;
                }
                else
                {
                    int weekNumNow = ((CultureInfo.CurrentCulture).Calendar.GetWeekOfYear(DateTime.Now,
                                         CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday)) - septemberTheFirstWeek;

                    weekNumUse = weekNum == 1 ? weekNumNow : weekNumNow + 1;
                }
            


            

            List<Lesson> listPar = schedule.GetSchedule(userDb.CheckUserElements(chatId, "university"),
                userDb.CheckUserElements(chatId, "facility"), userDb.CheckUserElements(chatId, "course"),
                userDb.CheckUserElements(chatId, "group"), weekNumUse, day);
            LessonIComparer<Lesson> comparer = new LessonIComparer<Lesson>();
            listPar.Sort(comparer);

            string result;

            if (day == (int)DateTime.Now.DayOfWeek)
                result = "Сегодня будет "+ listPar.Count+ " "+ GetLessonSpelling(listPar.Count)+"\n";
            else if (((day == ((int)DateTime.Now.DayOfWeek + 1) % 7) && DateTime.Now.DayOfWeek != 0) || (day == 1 && DateTime.Now.DayOfWeek == 0))
                result = "Завтра будет " + listPar.Count + " " + GetLessonSpelling(listPar.Count) + "\n";
            else
                result = "В "+ ConvertWeekDayToRussian(day)+" будет " + listPar.Count + " " + GetLessonSpelling(listPar.Count) + "\n";


            string lessons = "";
            foreach (Lesson item in listPar)
            {
                string teacher = item.TeachersNames;
                lessons += GetCountableNumber(item.Number) + " пара в " + ConvertToCorrectTimeFormat(item.Time).Split(" - ")[0] + "\n" + item.Name.Replace('.',' ');
                if (!string.IsNullOrEmpty(item.Type))
                    lessons += "\n" + item.Type.Replace('(',' ').Replace(')',' ').Replace('.',' ');
                if (!string.IsNullOrEmpty(item.Room))
                    lessons += "\nв " + item.Room.Replace('.', ' ');
                if (!string.IsNullOrEmpty(teacher))
                    lessons += "\n Ведёт " + teacher.Split(' ')[0];
                lessons += "\n\n";
            }

            if (lessons != "")
            {
                result += lessons;
                int weekNumNow = 0;
                if (scheduleType != 0)
                    weekNumNow = GetWeekNum(chatId, ((CultureInfo.CurrentCulture).Calendar.GetWeekOfYear(DateTime.Now,
                                                        CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday)) % scheduleType + 1, socialNetwork);
                else
                    weekNumNow = GetWeekNum(chatId, ((CultureInfo.CurrentCulture).Calendar.GetWeekOfYear(DateTime.Now,
                                                        CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday)) - septemberTheFirstWeek, socialNetwork);
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
                    return weekNum+" неделя";
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
                    return weekNum;
                }
            }
        }

        private string GetLessonSpelling(int lessonsCount)
        {
            int lastNumber = lessonsCount % 10;
            if (lastNumber == 1)
                return "пара";
            else if (lastNumber > 1 && lastNumber< 5)
                return "пары";
            else
                return "пар";
        }

        private string GetCountableNumber(string number)
        {
            switch (number)
            {
                case "1":
                    return "Первая";
                case "2":
                    return "Вторая";
                case "3":
                    return "Третья";
                case "4":
                    return "Четвёртая";
                case "5":
                    return "Пятая";
                case "6":
                    return "Шестая";
                case "7":
                    return "Седьмая";
                case "8":
                    return "Восьмая";
                case "9":
                    return "Девятая";
                case "10":
                    return "Десятая";
                default:
                    return "";
            }
        }
    }
}
