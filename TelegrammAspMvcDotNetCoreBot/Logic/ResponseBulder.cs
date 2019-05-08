using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Telegram.Bot.Types.ReplyMarkups;
using TelegrammAspMvcDotNetCoreBot.DB;
using VkNet.Model.Keyboard;

namespace TelegrammAspMvcDotNetCoreBot.Logic
{
    public class ResponseBulder
    {
        private string _socialNetwork;
        private SnUserDb userDb;
        private ScheduleDB scheduleDb = new ScheduleDB();
        private Schedule schedule = new Schedule();

        public ReplyKeyboardMarkup TelegramMainKeyboard { get; set; }
        public MessageKeyboard VkMainKeyboard { get; set; }

        public ResponseBulder(string socialNetwork)
        {
            string[][] mainKeyboardButtons =
            {
                new[] {"Сегодня", "Завтра"},
                new[] {"Расписание"},
                new[] {"Добавить ДЗ", "Что задали?"},
                new[] {"О пользователе","Сбросить"}
            };
            TelegramKeyboard telegramKeyboard = new TelegramKeyboard();
            TelegramMainKeyboard = telegramKeyboard.GetKeyboard(mainKeyboardButtons);

            VkKeyboard vkKeyboard = new VkKeyboard();
            VkMainKeyboard = vkKeyboard.GetKeyboard(mainKeyboardButtons);

            userDb = new SnUserDb(socialNetwork);
            _socialNetwork = socialNetwork;
        }

        public string[][] UniversitiesList(long id)
        {
            if (!userDb.CheckUser(id))
                userDb.CreateUser(id);
            else
                userDb.RecreateUser(id);

            List<string> un = new ScheduleDB().GetUniversities();
            return ButtonsFromList(un);
        }

        public string[][] FacilitiesList(long id, string university)
        {
            userDb.EditUser(id, "university", university);

            List<string> un = scheduleDb.GetFacilities(userDb.CheckUserElements(id, "university"));
            return ButtonsFromList(un);
        }

        public string[][] CoursesList(long id, string facility)
        {
            userDb.EditUser(id, "facility", facility);

            List<string> un = scheduleDb.GetCourses(userDb.CheckUserElements(id, "university"),
                userDb.CheckUserElements(id, "facility"));
            return ButtonsFromList(un);
        }

        public string[][] GroupsList(long id, string course)
        {
            userDb.EditUser(id, "course", course);

            List<string> un = scheduleDb.GetGroups(userDb.CheckUserElements(id, "university"),
                userDb.CheckUserElements(id, "facility"), userDb.CheckUserElements(id, "course"));
            return ButtonsFromList(un);
        }

        public void LetsWork(long id, string group)
        {
            userDb.EditUser(id, "group", group);
        }

        public string Today(long id)
        {
            int day;
            int weekNum = ((CultureInfo.CurrentCulture).Calendar.GetWeekOfYear(DateTime.Now,
                               CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday) + 1) % 2 + 1;
            if ((int)DateTime.Now.DayOfWeek == 0)
                day = 7;
            else
            {
                day = (int)DateTime.Now.DayOfWeek;
            }

            return schedule.ScheduleOnTheDay(id, weekNum, day, _socialNetwork);
        }

        public string Tommorrow(long id)
        {
            int day;
            int weekNum = ((CultureInfo.CurrentCulture).Calendar.GetWeekOfYear(DateTime.Now,
                               CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday) + 1) % 2 + 1;
            if ((int)DateTime.Now.DayOfWeek == 0)
            {
                day = 1;
                weekNum++;
            }
            else
            {
                if ((int)DateTime.Now.DayOfWeek == 6)
                    day = 7;
                else
                    day = ((int)DateTime.Now.DayOfWeek + 1) % 7;
            }

            return schedule.ScheduleOnTheDay(id, weekNum, day, _socialNetwork);
        }

        private string[][] ButtonsFromList(List<string> values)
        {
            int valuesLength = values.ToList().Count;
            int count = 0;

            if (valuesLength <= 10)
            {
                string[][] buttons = new string[values.ToList().Count][];
 
                foreach (string item in values)
                {
                    buttons[count] = new[] { item };
                    count++;
                }

                return buttons;
            }
            else if (valuesLength > 10 && valuesLength <=20)
            {
                string[][] buttons = valuesLength % 2 == 0 ? new string[valuesLength / 2][] : new string[valuesLength / 2 + 1][];

                for (int i = 0; i < values.Count; i += 2)
                {
                    if (i == values.Count - 1)
                        buttons[count] = new[] { values[i] }; //элементов нечетное кол-во
                    else
                        buttons[count] = new[] { values[i], values[i + 1] };
                    count++;
                }

                return buttons;
            }
            else if (valuesLength > 20 && valuesLength <= 30)
            {
                string[][] buttons = valuesLength % 3 == 0 ? new string[valuesLength / 3][] : new string[valuesLength / 3 + 1][];

                for (int i = 0; i < values.Count; i += 3)
                {
                    if (values.Count - i == 1)
                        buttons[count] = new[] { values[i] }; //остаток 1
                    else if (values.Count - i == 2)
                        buttons[count] = new[] { values[i], values[i + 1] }; //остаток 2
                    else
                        buttons[count] = new[] { values[i], values[i + 1], values[i + 2] };
                    count++;
                }

                return buttons;
            }
            else
            {
                if (valuesLength > 30 && valuesLength <= 40)
                {
                    string[][] buttons = valuesLength % 4 == 0 ? new string[valuesLength / 4][] : new string[valuesLength / 4 + 1][];

                    for (int i = 0; i < values.Count; i += 4)
                    {
                        if (values.Count - i == 1)
                            buttons[count] = new[] { values[i] }; //остаток 1
                        else if (values.Count - i == 2)
                            buttons[count] = new[] { values[i], values[i + 1] }; //остаток 2
                        else if (values.Count - i == 3)
                            buttons[count] = new[] { values[i], values[i + 1], values[i + 2] }; //остаток 3
                        else
                            buttons[count] = new[] { values[i], values[i + 1], values[i + 2], values[i + 3] };
                        count++;
                    }

                    return buttons;
                }
            }

            return null;
        }
    }
}
