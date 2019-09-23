using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Telegram.Bot.Types.ReplyMarkups;
using TelegrammAspMvcDotNetCoreBot.DB;
using VkNet.Model.Keyboard;

namespace TelegrammAspMvcDotNetCoreBot.Logic
{
    public class ResponseBuilder
    {
        private readonly string _socialNetwork;
        private readonly SnUserDb userDb;
        private readonly ScheduleDB scheduleDb = new ScheduleDB();
        private readonly Schedule schedule = new Schedule();

        public ReplyKeyboardMarkup TelegramMainKeyboard { get; }
        public MessageKeyboard VkMainKeyboard { get; }

        public InlineKeyboardMarkup InlineScheduleKeyboard { get; }
        public InlineKeyboardMarkup InlineTeacherScheduleKeyboard { get; }
        public InlineKeyboardMarkup InlineWatchingHomeworkKeyboard { get; }
        public InlineKeyboardMarkup InlineAddingHomeworkKeyboard { get; }
        public InlineKeyboardMarkup InlineCancelKeyboard { get; }

        public MessageKeyboard PayloadScheduleKeyboard { get; }
        public MessageKeyboard PayloadTeacherScheduleKeyboard { get; }
        public MessageKeyboard PayloadWatchingHomeworkKeyboard { get; }
        public MessageKeyboard PayloadAddingHomeworkKeyboard { get; }
        public MessageKeyboard PayloadCancelKeyboard { get; }

        public string MainVariants { get; }

        public ResponseBuilder(string socialNetwork)
        {
            string[][] mainKeyboardButtons =
            {
                new[] {"Сегодня", "Завтра"},
                new[] {"Расписание"},
                new[] {"Добавить ДЗ", "Что задали?"},
                new[] {"Расписание преподавателя","О пользователе"}
            };
            TelegramKeyboard telegramKeyboard = new TelegramKeyboard();
            VkKeyboard vkKeyboard = new VkKeyboard();

            TelegramMainKeyboard = telegramKeyboard.GetKeyboard(mainKeyboardButtons);
            VkMainKeyboard = vkKeyboard.GetKeyboard(mainKeyboardButtons);

            string[][] cancelButton =
{
                    new[] {"Отменить"}
                };

            string[][] scheduleText =
            {
                    new[] {"Пн", "Вт", "Ср", "Чт", "Пт", "Сб"},
                    new[] {"Пн", "Вт", "Ср", "Чт", "Пт", "Сб"}
                };


            string[][] scheduleTextVk =
            {
                new[] { "Пн", "Пн"},  //слева верхняя
                new[] { "Вт", "Вт" },
                new[] { "Ср", "Ср" },
                new[] { "Чт", "Чт" },
                new[] { "Пт", "Пт" },
                new[] { "Сб", "Сб" },
                new[] {"В главное меню"}
            };

            DateTime now = DateTime.Now.Date;

            string[][] homeworkText =
            {
                        new[] {DateConverter(now.Subtract(new TimeSpan(6, 0, 0, 0))),
                            DateConverter(now.Subtract(new TimeSpan(5, 0, 0, 0))),
                            DateConverter(now.Subtract(new TimeSpan(4, 0, 0, 0)))},
                        new[]{DateConverter(now.Subtract(new TimeSpan(3, 0, 0, 0))),
                            DateConverter(now.Subtract(new TimeSpan(2, 0, 0, 0))),
                            DateConverter(now.Subtract(new TimeSpan(1, 0, 0, 0)))},
                        new[]{DateConverter(now),
                            DateConverter(now.AddDays(1)),
                            DateConverter(now.AddDays(2))},
                        new[] {DateConverter(now.AddDays(3)),
                            DateConverter(now.AddDays(4)),
                            DateConverter(now.AddDays(5))},
                        new[]{DateConverter(now.AddDays(6)),
                            DateConverter(now.AddDays(7))}

                };
            string[][] homeworkTextVk =
            {
                new[] {DateConverter(now.Subtract(new TimeSpan(6, 0, 0, 0))),
                    DateConverter(now.Subtract(new TimeSpan(5, 0, 0, 0))),
                    DateConverter(now.Subtract(new TimeSpan(4, 0, 0, 0)))},
                new[]{DateConverter(now.Subtract(new TimeSpan(3, 0, 0, 0))),
                    DateConverter(now.Subtract(new TimeSpan(2, 0, 0, 0))),
                    DateConverter(now.Subtract(new TimeSpan(1, 0, 0, 0)))},
                new[]{DateConverter(now),
                    DateConverter(now.AddDays(1)),
                    DateConverter(now.AddDays(2))},
                new[] {DateConverter(now.AddDays(3)),
                    DateConverter(now.AddDays(4)),
                    DateConverter(now.AddDays(5))},
                new[]{DateConverter(now.AddDays(6)),
                    DateConverter(now.AddDays(7))},
                new[]{"В главное меню"}
            };

            /* CallbackQuery.Data представляет из себя трехзначное число abc
    a = [0,6], где 0 - отмена, 1 - просмотр расписания верхней недели, 2 - просмотр расписания нижней недели, 3 - добавление ДЗ, 
                                4 - просмотр ДЗ, 5,6 - выбор дня недели препода
    b1, b2, b5, b6 = [1,7], где 1 - понедельник, 2 - вторник, .. , 7 - воскресенье
    b3, b4 = [0,7], где 0 - ноль дней от текущей даты, 1 - один день от текущей даты, ..
    c1, c2,c5, c6 = 0
    c3, c4 = 0 - плюс день, 1 - минус день
    b0, с0 = 0
    */

            string[][] cancelCallbackData =
            {
                    new[] {"000"}
                };

            string[][] scheduleCallbackData =
            {
                    new[] {"110", "120", "130", "140", "150", "160"},
                    new[] {"210", "220", "230", "240", "250", "260"}
                };
            string[][] scheduleCallbackDataVk =
            {
                new[] {"110", "210"},
                new[] { "120", "220" },
                new[] { "130", "230" },
                new[] { "140", "240" },
                new[] { "150", "250" },
                new[] { "160", "260" },
                new [] {"000"}
            };

            string[][] scheduleTeacherCallbackData =
            {
                new[] {"510", "520", "530", "540", "550", "560"},
                new[] {"610", "620", "630", "640", "650", "660"}
            };
            string[][] scheduleTeacherCallbackDataVk =
            {
                new[] {"510", "610"},
                new[] { "520", "620" },
                new[] { "530", "630" },
                new[] { "540", "640" },
                new[] { "550", "650" },
                new[] { "560", "660" },
                new [] {"000"}
            };

            string[][] addingHomeworkCallbackData =
            {
                new[] { "361","351", "341"},
                new[] {"331", "321", "311"},
                new[] {"300","310", "320"},
                new[] {"330", "340", "350"},
                new[] { "360", "370" }

            };


            string[][] addingHomeworkCallbackDataVk =
            {
                new[] { "361","351", "341"},
                new[] {"331", "321", "311"},
                new[] {"300","310", "320"},
                new[] {"330", "340", "350"},
                new[] { "360", "370" },
                new[] {"000"}
            };

            string[][] watchingHomeworkCallbackData =
            {
                    new[] { "461","451", "441"},
                    new[] {"431", "421", "411"},
                    new[] {"400","410", "420"},
                    new[] {"430", "440", "450"},
                    new[] { "460", "470" }
                };
            string[][] watchingHomeworkCallbackDataVk =
            {
                new[] { "461","451", "441"},
                new[] {"431", "421", "411"},
                new[] {"400","410", "420"},
                new[] {"430", "440", "450"},
                new[] { "460", "470" },
                new[] {"000"}
            };


            //Определение сегодняшнего дня

            int day;
            int weekNum = ((CultureInfo.CurrentCulture).Calendar.GetWeekOfYear(DateTime.Now,
                               CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday)) % 2+1;
            if (DateTime.Now.DayOfWeek == 0)
            {
                day = 7;
            }
            else
            {
                day = (int)DateTime.Now.DayOfWeek;
            }

            string today = weekNum.ToString() + day.ToString() + "0";

            InlineScheduleKeyboard = telegramKeyboard.GetInlineKeyboard(scheduleText, scheduleCallbackData);
            InlineTeacherScheduleKeyboard = telegramKeyboard.GetInlineKeyboard(scheduleText, scheduleTeacherCallbackData);
            InlineWatchingHomeworkKeyboard = telegramKeyboard.GetInlineKeyboard(homeworkText, watchingHomeworkCallbackData);
            InlineAddingHomeworkKeyboard = telegramKeyboard.GetInlineKeyboard(homeworkText, addingHomeworkCallbackData);
            InlineCancelKeyboard = telegramKeyboard.GetInlineKeyboard(cancelButton, cancelCallbackData);

            PayloadScheduleKeyboard = vkKeyboard.GetPayloadKeyboard(scheduleTextVk, scheduleCallbackDataVk, today);
            PayloadTeacherScheduleKeyboard = vkKeyboard.GetPayloadKeyboard(scheduleTextVk, scheduleTeacherCallbackDataVk, today);
            PayloadWatchingHomeworkKeyboard = vkKeyboard.GetPayloadKeyboard(homeworkTextVk, watchingHomeworkCallbackDataVk);
            PayloadAddingHomeworkKeyboard = vkKeyboard.GetPayloadKeyboard(homeworkTextVk, addingHomeworkCallbackDataVk);
            PayloadCancelKeyboard = vkKeyboard.GetPayloadKeyboard(cancelButton, cancelCallbackData);

            userDb = new SnUserDb(socialNetwork);
            _socialNetwork = socialNetwork;
            string[][] mainKeyboardButtonsNoKeyboard =
            {
                new[] {"Сегодня", "Завтра"},
                new[] {"О пользователе","Сбросить"}
            };
            MainVariants = AnswerVariants(mainKeyboardButtonsNoKeyboard);
        }

        public string[][] UniversitiesArray(long id)
        {
            if (!userDb.CheckUser(id))
            {
                userDb.CreateUser(id);
            }
            else
            {
                userDb.RecreateUser(id);
            }

            List<string> un = new ScheduleDB().GetUniversities();
            return ButtonsFromList(un);
        }

        public string[][] FacilitiesArray(long id, string university)
        {
            userDb.EditUser(id, "university", university);

            List<string> un = scheduleDb.GetFacilities(userDb.CheckUserElements(id, "university"));
            return ButtonsFromList(un);
        }

        public string[][] CoursesArray(long id, string facility)
        {
            userDb.EditUser(id, "facility", facility);

            List<string> un = scheduleDb.GetCourses(userDb.CheckUserElements(id, "university"),
                userDb.CheckUserElements(id, "facility"));
            return ButtonsFromList(un);
        }

        public string[][] GroupsArray(long id, string course, int page = 1)
        {
            userDb.EditUser(id, "course", course);

            List<string> un = scheduleDb.GetGroups(userDb.CheckUserElements(id, "university"),
                userDb.CheckUserElements(id, "facility"), userDb.CheckUserElements(id, "course"));
            return ButtonsFromList(un, page);
        }

        public void LetsWork(long id, string group)
        {
            userDb.EditUser(id, "group", group);
        }

        public string Today(long id)
        {
            int day;
            int weekNum = ((CultureInfo.CurrentCulture).Calendar.GetWeekOfYear(DateTime.Now,
                               CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday)) % 2+1;
            if (DateTime.Now.DayOfWeek == 0)
            {
                day = 7;
            }
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
                               CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday)) % 2+1;
            if (DateTime.Now.DayOfWeek == 0)
            {
                day = 1;
                weekNum = weekNum == 1 ? 2 : 1;
            }
            else
            {
                if ((int)DateTime.Now.DayOfWeek == 6)
                {
                    day = 7;
                }
                else
                {
                    day = ((int)DateTime.Now.DayOfWeek + 1) % 7;
                }
            }

            return schedule.ScheduleOnTheDay(id, weekNum, day, _socialNetwork);
        }

        public string UserInfo(long id)
        {
            string result = "Информация о пользователе\n \n";
            result += "Id: " + id + "\n";
            result += "Институт: " + userDb.CheckUserElements(id, "university") + "\n";
            result += "Факультет: " + userDb.CheckUserElements(id, "facility") + "\n";
            result += "Курс: " + userDb.CheckUserElements(id, "course") + "\n";
            result += "Группа: " + userDb.CheckUserElements(id, "group") + "\n";
            return result;
        }

        private string[][] ButtonsFromList(List<string> values, int page = 1)
        {
            int valuesLength = values.Count;
            int count = 0;

            if (valuesLength < 10)
            {
                string[][] buttons = new string[values.ToList().Count][];

                foreach (string item in values)
                {
                    buttons[count] = new[] { item };
                    count++;
                }

                return buttons;
            }
            else if ((valuesLength >= 10 && valuesLength <= 18 && _socialNetwork == "Vk") || (valuesLength >= 10 && _socialNetwork == "Telegram"))
            {
                string[][] buttons = valuesLength % 2 == 0 ? new string[valuesLength / 2][] : new string[valuesLength / 2 + 1][];

                for (int i = 0; i < values.Count; i += 2)
                {
                    if (i == values.Count - 1)
                    {
                        buttons[count] = new[] { values[i] }; //элементов нечетное кол-во
                    }
                    else
                    {
                        buttons[count] = new[] { values[i], values[i + 1] };
                    }

                    count++;
                }

                return buttons;
            }
            else if (valuesLength > 18 && _socialNetwork == "Vk")
            {
                List<string> pageList = new List<string>();

                //pages counting
                const int itemsPerPage = 10;
                int pagesCount = 0;
                int elementsCount = valuesLength;

                for (int i = 0; i < 100; i++)
                {
                    elementsCount -= itemsPerPage;
                    pagesCount++;
                    if (elementsCount <= 0)
                    {
                        break;
                    }
                }

                //chosen page forming



                for (int i = 0; i < valuesLength; i++)
                {
                    if (i >= (page - 1) * itemsPerPage && i < page * itemsPerPage)
                    {
                        pageList.Add(values[i]);
                    }
                }

                //forming buttons

                string[][] buttons = new string[pageList.Count / 2 + 1][];

                for (int i = 0; i < pageList.Count; i += 2)
                {
                    buttons[count] = new[] { pageList[i], pageList[i + 1] };
                    count++;
                }

                //forming arrows

                if (page == 1)
                {
                    buttons[count] = new[] { (page + 1) + " ->" };
                }
                else if (page == pagesCount)
                {
                    buttons[count] = new[] { "<- " + (page - 1) };
                }
                else
                {
                    buttons[count] = new[] { "<- " + (page - 1), (page + 1) + " ->" };
                }

                return buttons;


            }
            //else if (valuesLength > 20 && valuesLength <= 30)
            //{
            //    string[][] buttons = valuesLength % 3 == 0 ? new string[valuesLength / 3][] : new string[valuesLength / 3 + 1][];

            //    for (int i = 0; i < values.Count; i += 3)
            //    {
            //        if (values.Count - i == 1)
            //            buttons[count] = new[] { values[i] }; //остаток 1
            //        else if (values.Count - i == 2)
            //            buttons[count] = new[] { values[i], values[i + 1] }; //остаток 2
            //        else
            //            buttons[count] = new[] { values[i], values[i + 1], values[i + 2] };
            //        count++;
            //    }

            //    return buttons;
            //}
            //else if (valuesLength > 30 && valuesLength <= 40)
            //    {
            //        string[][] buttons = valuesLength % 4 == 0 ? new string[valuesLength / 4][] : new string[valuesLength / 4 + 1][];

            //        for (int i = 0; i < values.Count; i += 4)
            //        {
            //            if (values.Count - i == 1)
            //                buttons[count] = new[] { values[i] }; //остаток 1
            //            else if (values.Count - i == 2)
            //                buttons[count] = new[] { values[i], values[i + 1] }; //остаток 2
            //            else if (values.Count - i == 3)
            //                buttons[count] = new[] { values[i], values[i + 1], values[i + 2] }; //остаток 3
            //            else
            //                buttons[count] = new[] { values[i], values[i + 1], values[i + 2], values[i + 3] };
            //            count++;
            //        }

            //        return buttons;
            //    }


            return null;
        }

        public string DateConverter(DateTime date)
        {
            string shortdate = date.ToShortDateString();
            string month = shortdate.Split(".")[1];
            string day = shortdate.Split(".")[0];

            return day + "." + month;
        }

        public string AnswerVariants(string[][] buttons)
        {
            string result = "";
            for (int i = 0; i < buttons.Length; i++)
            {
                for (int j = 0; j < buttons[i].Length; j++)
                {
                    result += buttons[i][j] + ", ";
                }
            }

            return result;
        }
    }
}
