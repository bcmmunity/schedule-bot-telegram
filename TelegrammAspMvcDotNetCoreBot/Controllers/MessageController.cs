using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;
using TelegrammAspMvcDotNetCoreBot.DB;
using TelegrammAspMvcDotNetCoreBot.Logic;
using TelegrammAspMvcDotNetCoreBot.Models.Telegramm;

namespace TelegrammAspMvcDotNetCoreBot.Controllers
{
    [Route("api/message/update")]
    public class MessageController : Controller
    {
        private static bool Dz { get; set; } = false;
        private static string Date { get; set; } = String.Empty;
        private UserDb userDb = new UserDb();

        // GET api/values
        [HttpGet]
        public string Get()
        {
            return "Method GET unuvalable";
        }

        // POST api/values
        [HttpPost]
        public async Task<OkResult> Post([FromBody]Update update)
        {
            try
            {
                if (update == null) return Ok();

                var botClient = await Bot.GetBotClientAsync();

                if (update.Type == UpdateType.Message)
                {
                    var message = update.Message;
                    var chatId = message.Chat.Id;
                    var commands = Bot.Commands;

                    //await botClient.SendTextMessageAsync(chatId, "Бот на профилактике", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
                    //return Ok();

                    foreach (var command in commands)
                    {
                        if (command.Contains(message) || (!userDb.CheckUser(chatId)))
                        {
                            await command.Execute(message, botClient);
                            return Ok();
                        }
                    }

                    if (!userDb.CheckUser(chatId))
                        return Ok();

                    InputOnlineFile facSticker = new InputOnlineFile("CAADAgADBwADi6p7D7JUJy3u1Q22Ag");
                    InputOnlineFile courseSticker = new InputOnlineFile("CAADAgADBgADi6p7DxEJvhyK0iHFAg");
                    InputOnlineFile groupSticker = new InputOnlineFile("CAADAgADBAADi6p7DzzxU-ilYtP6Ag");
                    InputOnlineFile workSticker = new InputOnlineFile("CAADAgADBQADi6p7D849HV-BVKxIAg");
                    InputOnlineFile relaxSticker = new InputOnlineFile("CAADAgADAgADi6p7D_SOcGo7bWCIAg");

                    ScheduleDB scheduleDb = new ScheduleDB();
                    HomeWorkDB homeWorkDb = new HomeWorkDB();
                    Schedule schedule = new Schedule();

                    TelegramKeybord keybord = new TelegramKeybord();

                    string[][] mainKeyboardButtons =
                    {
                    new[] {"Сегодня", "Завтра"},
                    new[] {"Расписание"},
                    new[] {"Добавить ДЗ", "Что задали?"},
                    new[] {"О пользователе","Сбросить"}
                };

                    ReplyKeyboardMarkup mainKeyboard = keybord.GetKeyboard(mainKeyboardButtons);

                    //Режим добавления ДЗ
                    if (Dz)
                    {
                        homeWorkDb.AddHomeWork(userDb.CheckUserElements(chatId, "university"),
                            userDb.CheckUserElements(chatId, "facility"), userDb.CheckUserElements(chatId, "course"),
                            userDb.CheckUserElements(chatId, "group"), Date, message.Text);
                        Dz = false;
                        Date = String.Empty;
                        await botClient.SendTextMessageAsync(chatId, "Задание было успешно добавлено", ParseMode.Markdown);
                        return Ok();
                    }

                    //Основной режим 
                    if (userDb.CheckUserElements(chatId, "university") == "" && scheduleDb.IsUniversityExist(message.Text))
                    {
                        userDb.EditUser(chatId, "university", message.Text);

                        List<string> un = scheduleDb.GetFacilities(userDb.CheckUserElements(chatId, "university"));

                        string[][] unn = new string[un.ToList().Count][];

                        int count = 0;
                        foreach (string item in un)
                        {
                            unn[count] = new[] { item };
                            count++;
                        }

                        //await botClient.SendTextMessageAsync(chatId, "Теперь выбери факультет", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: (Telegram.Bot.Types.ReplyMarkups.IReplyMarkup)KeybordController.GetKeyboard(unn, count));
                        await botClient.SendStickerAsync(chatId, facSticker, replyMarkup: keybord.GetKeyboard(unn));

                        return Ok();
                    }

                    if (userDb.CheckUserElements(chatId, "facility") == "" &&
                        scheduleDb.IsFacilityExist(userDb.CheckUserElements(chatId, "university"), message.Text))
                    {
                        userDb.EditUser(chatId, "facility", message.Text);

                        List<string> un = scheduleDb.GetCourses(userDb.CheckUserElements(chatId, "university"),
                            userDb.CheckUserElements(chatId, "facility"));

                        string[][] unn = new string[un.ToList().Count][];

                        int count = 0;
                        foreach (string item in un)
                        {
                            unn[count] = new[] { item };
                            count++;
                        }

                        //await botClient.SendTextMessageAsync(chatId, "Теперь выбери курс", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: (Telegram.Bot.Types.ReplyMarkups.IReplyMarkup)KeybordController.GetKeyboard(unn, count));
                        await botClient.SendStickerAsync(chatId, courseSticker, replyMarkup: keybord.GetKeyboard(unn));
                        return Ok();
                    }

                    if (userDb.CheckUserElements(chatId, "course") == "" && scheduleDb.IsCourseExist(
                            userDb.CheckUserElements(chatId, "university"), userDb.CheckUserElements(chatId, "facility"),
                            message.Text))
                    {
                        userDb.EditUser(chatId, "course", message.Text);

                        List<string> un = scheduleDb.GetGroups(userDb.CheckUserElements(chatId, "university"),
                            userDb.CheckUserElements(chatId, "facility"), userDb.CheckUserElements(chatId, "course"));

                        int unnLength = un.ToList().Count;
                        string[][] unn = unnLength % 2 == 0 ? new string[unnLength / 2][] : new string[unnLength / 2 + 1][];

                        int count = 0;

                        for (int i = 0; i < un.Count; i += 2)
                        {
                            if (i == un.Count - 1)
                                unn[count] = new[] { un[i] }; //элементов нечетное кол-во
                            else
                                unn[count] = new[] { un[i], un[i + 1] };
                            count++;
                        }

                        //await botClient.SendTextMessageAsync(chatId, "Теперь выбери группу", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: (Telegram.Bot.Types.ReplyMarkups.IReplyMarkup)KeybordController.GetKeyboard(unn, count));
                        await botClient.SendStickerAsync(chatId, groupSticker, replyMarkup: keybord.GetKeyboard(unn));
                        return Ok();
                    }

                    if (userDb.CheckUserElements(chatId, "group") == "" && scheduleDb.IsGroupExist(
                            userDb.CheckUserElements(chatId, "university"), userDb.CheckUserElements(chatId, "facility"),
                            userDb.CheckUserElements(chatId, "course"), message.Text))
                    {
                        userDb.EditUser(chatId, "group", message.Text);

                        // await botClient.SendTextMessageAsync(chatId, "Отлично, можем работать!", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: (Telegram.Bot.Types.ReplyMarkups.IReplyMarkup)KeybordController.GetKeyboard(unn, 2));
                        await botClient.SendStickerAsync(chatId, workSticker, replyMarkup: mainKeyboard);
                        return Ok();
                    }

                    if (message.Text == "Сегодня" && userDb.CheckUserElements(chatId, "group") != "")
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

                        string result = schedule.ScheduleOnTheDay(chatId, weekNum, day);

                        if (!result.Equals("Учебы нет"))
                            await botClient.SendTextMessageAsync(chatId, result, parseMode: ParseMode.Markdown, replyMarkup: mainKeyboard);
                        else

                            //await botClient.SendTextMessageAsync(chatId, "Учебы нет, отдыхай", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
                            await botClient.SendStickerAsync(chatId, relaxSticker, replyMarkup: mainKeyboard);

                        return Ok();
                    }

                    if (message.Text == "Завтра" && userDb.CheckUserElements(chatId, "group") != "")
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

                        string result = schedule.ScheduleOnTheDay(chatId, weekNum, day);

                        if (!result.Equals("Учебы нет"))
                            await botClient.SendTextMessageAsync(chatId, result, ParseMode.Markdown, replyMarkup: mainKeyboard);
                        else
                            //await botClient.SendTextMessageAsync(chatId, "Учебы нет, отдыхай", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
                            await botClient.SendStickerAsync(chatId, relaxSticker, replyMarkup: mainKeyboard);

                        return Ok();
                    }

                    if (message.Text == "Расписание")
                    {
                        string[][] unn =
                        {
                        new[] {"Пн", "Вт", "Ср", "Чт", "Пт", "Сб"},
                        new[] {"Пн", "Вт", "Ср", "Чт", "Пт", "Сб"}
                    };

                        string[][] callbackData =
                        {
                        new[] {"110", "120", "130", "140", "150", "160"},
                        new[] {"210", "220", "230", "240", "250", "260"}
                    };

                        await botClient.SendTextMessageAsync(chatId, "Выбери неделю и день", ParseMode.Markdown,
                            replyMarkup: keybord.GetInlineKeyboard(unn, callbackData));
                        return Ok();
                    }

                    if (message.Text == "Добавить ДЗ" && userDb.CheckUserElements(chatId, "group") != "")
                    {
                        DateTime now = DateTime.Now.Date;

                        string[][] unn =
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

                        string[][] callbackData =
                        {
                        new[] { "361","351", "341"},
                        new[] {"331", "321", "311"},
                        new[] {"300","310", "320"},
                        new[] {"330", "340", "350"},
                        new[] { "360", "370" }

                    };

                        await botClient.SendTextMessageAsync(chatId, "Выбери дату\n \nСегодня " + DateConverter(now), ParseMode.Markdown,
                            replyMarkup: keybord.GetInlineKeyboard(unn, callbackData));
                        return Ok();
                    }
                    if (message.Text == "Что задали?" && userDb.CheckUserElements(chatId, "group") != "")
                    {
                        DateTime now = DateTime.Now.Date;


                        string[][] unn =
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

                        string[][] callbackData =
                        {
                        new[] { "461","451", "441"},
                        new[] {"431", "421", "411"},
                        new[] {"400","410", "420"},
                        new[] {"430", "440", "450"},
                        new[] { "460", "470" }
                    };

                        await botClient.SendTextMessageAsync(chatId, "Выбери дату\n \nСегодня " + DateConverter(now), ParseMode.Markdown,
                            replyMarkup: keybord.GetInlineKeyboard(unn, callbackData));
                        return Ok();
                    }
                    if (message.Text == "О пользователе")
                    {
                        string result = "Информация о пользователе\n \n";
                        result += "Id: " + chatId + "\n";
                        result += "Институт: " + userDb.CheckUserElements(chatId, "university") + "\n";
                        result += "Факультет: " + userDb.CheckUserElements(chatId, "facility") + "\n";
                        result += "Курс: " + userDb.CheckUserElements(chatId, "course") + "\n";
                        result += "Группа: " + userDb.CheckUserElements(chatId, "group") + "\n";

                        await botClient.SendTextMessageAsync(chatId, result, parseMode: ParseMode.Markdown, replyMarkup: mainKeyboard);
                        return Ok();
                    }

                    if (message.Text == "Сбросить")
                    {
                        message.Text = @"/start";

                        foreach (var command in commands)
                        {
                            if (command.Contains(message))
                            {
                                await command.Execute(message, botClient);
                                return Ok();
                            }
                        }

                        return Ok();
                    }
                }

                //Callback Query
                else if (update.Type == UpdateType.CallbackQuery)
                {
                    long chatId = update.CallbackQuery.Message.Chat.Id;
                    TelegramKeybord keybord = new TelegramKeybord();
                    DateTime now = DateTime.Now.Date;
                    Schedule schedule = new Schedule();
                    HomeWorkLogic homeWork = new HomeWorkLogic();

                    string[][] homeworkCancelButton =
                    {
                    new[] {"Отменить"}
                };

                    string[][] scheduleText =
                    {
                    new[] {"Пн", "Вт", "Ср", "Чт", "Пт", "Сб"},
                    new[] {"Пн", "Вт", "Ср", "Чт", "Пт", "Сб"}
                };

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

                    string[][] homeworkCancelCallbackData =
                    {
                    new[] {"000"}
                };

                    string[][] scheduleCallbackData =
                    {
                    new[] {"110", "120", "130", "140", "150", "160"},
                    new[] {"210", "220", "230", "240", "250", "260"}
                };

                    string[][] homeworkCallbackData =
                    {
                    new[] { "461","451", "441"},
                    new[] {"431", "421", "411"},
                    new[] {"400","410", "420"},
                    new[] {"430", "440", "450"},
                    new[] { "460", "470" }
                };

                    InlineKeyboardMarkup inlineScheduleKeyboard = keybord.GetInlineKeyboard(scheduleText, scheduleCallbackData);
                    InlineKeyboardMarkup inlineHomeworkKeyboard = keybord.GetInlineKeyboard(homeworkText, homeworkCallbackData);
                    InlineKeyboardMarkup homeworkCancelKeyboard = keybord.GetInlineKeyboard(homeworkCancelButton, homeworkCancelCallbackData);

                    /* CallbackQuery.Data представляет из себя трехзначное число abc
                        a = [0,4], где 0 - отмена, 1 - просмотр расписания верхней недели, 2 - просмотр расписания нижней недели, 3 - добавление ДЗ, 4 - просмотр ДЗ
                        b1, b2 = [1,7], где 1 - понедельник, 2 - вторник, .. , 7 - воскресенье
                        b3, b4 = [0,7], где 0 - ноль дней от текущей даты, 1 - один день от текущей даты, ..
                        c1, c2 = 0
                        c3, c4 = 0 - плюс день, 1 - минус день
                        b0, с0 = 0
                        */

                    int a = Convert.ToInt32(Char.GetNumericValue(update.CallbackQuery.Data[0]));
                    int b = Convert.ToInt32(Char.GetNumericValue(update.CallbackQuery.Data[1]));
                    int c = Convert.ToInt32(Char.GetNumericValue(update.CallbackQuery.Data[2]));

                    if (a == 0)
                    {
                        Dz = false;
                        Date = String.Empty;
                        await botClient.EditMessageTextAsync(chatId,
                            update.CallbackQuery.Message.MessageId, "Ввод задания отменен");
                        await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                    }
                    else if (a == 1 || a == 2)
                    {
                        string result = schedule.ScheduleOnTheDay(chatId, a, b);
                        await botClient.EditMessageTextAsync(chatId,
                            update.CallbackQuery.Message.MessageId, result, replyMarkup: inlineScheduleKeyboard);
                        await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                    }
                    else if (a == 3)
                    {
                        if (c == 0)
                            await botClient.EditMessageTextAsync(chatId,
                                update.CallbackQuery.Message.MessageId, AddHomework(b), replyMarkup: homeworkCancelKeyboard);
                        else if (c == 1)
                            await botClient.EditMessageTextAsync(chatId,
                                update.CallbackQuery.Message.MessageId, AddHomework(-b), replyMarkup: homeworkCancelKeyboard);
                        await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                    }
                    else if (a == 4)
                    {
                        string result = String.Empty;
                        if (c == 0)
                            result = homeWork.SendHomework(chatId, b);
                        else if (c == 1)
                            result = homeWork.SendHomework(chatId, -b);

                        if (result == update.CallbackQuery.Message.Text)
                        {
                            await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                        }
                        else
                        {
                            await botClient.EditMessageTextAsync(chatId,
                                update.CallbackQuery.Message.MessageId, result, replyMarkup: inlineHomeworkKeyboard);
                            await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                        }

                    }
                }

                return Ok();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                if (update != null)
                {
                    var message = update.Message;
                    var chatId = message.Chat.Id;
                    var botClient = await Bot.GetBotClientAsync();

                    if (chatId == 358243561)
                        await botClient.SendTextMessageAsync(chatId, e.Message, parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
                    else
                        await botClient.SendTextMessageAsync(chatId, "Хм, что то пошло не так", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);

                }

                return Ok();
            }
        }

        private string DateConverter(DateTime date)
        {
            string shortdate = date.ToShortDateString();
            string month = shortdate.Split(".")[1];
            string day = shortdate.Split(".")[0];

            return day + "." + month;
        }

        private string AddHomework(int daysfromtoday)
        {
            DateTime now = DateTime.Now.Date;
            Dz = true;
            if (daysfromtoday < 0)
                Date = DateConverter(now.Subtract(new TimeSpan(-daysfromtoday, 0, 0, 0)));
            else if (daysfromtoday == 0)
            {
                Date = DateConverter(now);
            }
            else if (daysfromtoday > 0)
            {
                Date = DateConverter(now.AddDays(daysfromtoday));
            }

            return "Введите текст домашнего задания и отправьте его как обычное сообщение";
        }

       
	}
}
