using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;
using TelegrammAspMvcDotNetCoreBot.Logic;
using TelegrammAspMvcDotNetCoreBot.Models;
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

            var botClient = await Bot.GetBotClientAsync();

		

            if (update.Type == UpdateType.Message)
		    {
		        var message = update.Message;
		        var chatId = message.Chat.Id;

                InputOnlineFile facSticker = new InputOnlineFile("CAADAgADBwADi6p7D7JUJy3u1Q22Ag");
		        InputOnlineFile courseSticker = new InputOnlineFile("CAADAgADBgADi6p7DxEJvhyK0iHFAg");
		        InputOnlineFile groupSticker = new InputOnlineFile("CAADAgADBAADi6p7DzzxU-ilYtP6Ag");
		        InputOnlineFile workSticker = new InputOnlineFile("CAADAgADBQADi6p7D849HV-BVKxIAg");
		        InputOnlineFile relaxSticker = new InputOnlineFile("CAADAgADAgADi6p7D_SOcGo7bWCIAg");

		        if (update == null) return Ok();

		        var commands = Bot.Commands;

		        ScheduleDB schedule = new ScheduleDB();
                HomeWorkDB homeWork = new HomeWorkDB();

		        TelegramKeybord keybord = new TelegramKeybord();

                
		        string[][] mainKeyboardButtons =
		        {
		            new[] {"Сегодня", "Завтра"},
		            new[] {"Расписание"},
		            new[] {"Добавить ДЗ", "Что задали?"},
		            new[] {"О пользователе","Сбросить"}
		        };

		        ReplyKeyboardMarkup mainKeyboard = keybord.GetKeyboard(mainKeyboardButtons, 4);

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

                //Режим добавления ДЗ
		        if (Dz)
		        {
		            homeWork.AddHomeWork(userDb.CheckUserElements(chatId, "university"),
		                userDb.CheckUserElements(chatId, "facility"), userDb.CheckUserElements(chatId, "course"),
		                userDb.CheckUserElements(chatId, "group"), Date, message.Text);
                    Dz = false;
                    Date = String.Empty;
		            await botClient.SendTextMessageAsync(chatId, "Задание было успешно добавлено", ParseMode.Markdown);
                    return Ok();
		        }

		        //Основной режим 
                if (userDb.CheckUserElements(chatId, "university") == "" && schedule.IsUniversityExist(message.Text))
		        {
		            userDb.EditUser(chatId, "university", message.Text);

		            List<string> un = schedule.GetFacilities(userDb.CheckUserElements(chatId, "university"));

		            string[][] unn = new string[un.ToList().Count][];

		            int count = 0;
		            foreach (string item in un)
		            {
		                unn[count] = new[] {item};
		                count++;
		            }

		            //await botClient.SendTextMessageAsync(chatId, "Теперь выбери факультет", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: (Telegram.Bot.Types.ReplyMarkups.IReplyMarkup)KeybordController.GetKeyboard(unn, count));
		            await botClient.SendStickerAsync(chatId, facSticker, replyMarkup: keybord.GetKeyboard(unn, count));

		            return Ok();
		        }

		        if (userDb.CheckUserElements(chatId, "facility") == "" &&
		            schedule.IsFacilityExist(userDb.CheckUserElements(chatId, "university"), message.Text))
		        {
		            userDb.EditUser(chatId, "facility", message.Text);

		            List<string> un = schedule.GetCourses(userDb.CheckUserElements(chatId, "university"),
		                userDb.CheckUserElements(chatId, "facility"));

		            string[][] unn = new string[un.ToList().Count][];

		            int count = 0;
		            foreach (string item in un)
		            {
		                unn[count] = new[] {item};
		                count++;
		            }

		            //await botClient.SendTextMessageAsync(chatId, "Теперь выбери курс", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: (Telegram.Bot.Types.ReplyMarkups.IReplyMarkup)KeybordController.GetKeyboard(unn, count));
		            await botClient.SendStickerAsync(chatId, courseSticker, replyMarkup: keybord.GetKeyboard(unn, count));
		            return Ok();
		        }

		        if (userDb.CheckUserElements(chatId, "course") == "" && schedule.IsCourseExist(
		                userDb.CheckUserElements(chatId, "university"), userDb.CheckUserElements(chatId, "facility"),
		                message.Text))
		        {
		            userDb.EditUser(chatId, "course", message.Text);

		            List<string> un = schedule.GetGroups(userDb.CheckUserElements(chatId, "university"),
		                userDb.CheckUserElements(chatId, "facility"), userDb.CheckUserElements(chatId, "course"));

		            string[][] unn = new string[un.ToList().Count][];

		            int count = 0;
		            foreach (string item in un)
		            {
		                unn[count] = new[] {item};
		                count++;
		            }

		            //await botClient.SendTextMessageAsync(chatId, "Теперь выбери группу", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: (Telegram.Bot.Types.ReplyMarkups.IReplyMarkup)KeybordController.GetKeyboard(unn, count));
		            await botClient.SendStickerAsync(chatId, groupSticker, replyMarkup: keybord.GetKeyboard(unn, count));
		            return Ok();
		        }

		        if (userDb.CheckUserElements(chatId, "group") == "" && schedule.IsGroupExist(
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
		            if ((int) DateTime.Now.DayOfWeek == 0)
		                day = 7;
		            else
		            {
		                day = (int) DateTime.Now.DayOfWeek;
		            }

		            string result = ScheduleOnTheDay(chatId, userDb, weekNum, day);

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
		            if ((int) DateTime.Now.DayOfWeek == 0)
		            {
		                day = 1;
		                weekNum++;
		            }
		            else
		            {
		                if ((int) DateTime.Now.DayOfWeek == 6)
		                    day = 7;
		                else
		                    day = ((int) DateTime.Now.DayOfWeek + 1) % 7;
		            }

		            string result = ScheduleOnTheDay(chatId, userDb, weekNum, day);

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
		                new[] {"Mo1", "Tu1", "We1", "Th1", "Fr1", "Sa1"},
		                new[] {"Mo2", "Tu2", "We2", "Th2", "Fr2", "Sa2"}
		            };

		            await botClient.SendTextMessageAsync(chatId, "Выбери неделю и день", ParseMode.Markdown,
		                replyMarkup: keybord.GetInlineKeyboard(unn, callbackData, 2));
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
		                new[] { "m6","m5", "m4"},
		                new[] {"m3", "m2", "m1"},
		                new[] {"mp","p1", "p2"},
                        new[] {"p3", "p4", "p5"},
		                new[] { "p6", "p7" }

                    };

		            await botClient.SendTextMessageAsync(chatId, "Выбери дату\n \nСегодня "+ DateConverter(now), ParseMode.Markdown,
		                replyMarkup: keybord.GetInlineKeyboard(unn, callbackData, 5));
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
                        new[] { "m6?","m5?", "m4?"},
                        new[] {"m3?", "m2?", "m1?"},
                        new[] {"mp?","p1?", "p2?"},
                        new[] {"p3?", "p4?", "p5?"},
                        new[] { "p6?", "p7?" }
                    };

                    await botClient.SendTextMessageAsync(chatId, "Выбери дату\n \nСегодня " + DateConverter(now), ParseMode.Markdown,
                        replyMarkup: keybord.GetInlineKeyboard(unn, callbackData, 5));
                    return Ok();
                }
		        if (message.Text == "О пользователе")
		        {
		            string result = "Информация о пользователе\n \n";
		            result += "Id: " + chatId + "\n";
                    result += "Институт: " + userDb.CheckUserElements(chatId, "university")+"\n";
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
		    else if (update.Type == UpdateType.CallbackQuery)
		    {
		        long chatId = update.CallbackQuery.Message.Chat.Id;
                TelegramKeybord keybord = new TelegramKeybord();
		        DateTime now = DateTime.Now.Date;

		        string[][] homeworkCancelButton =
		        {
		            new[] {"Отменить"}
		        };

                string[][] scheduleText =
		        {
		            new[] {"Пн", "Вт", "Ср", "Чт", "Пт", "Сб"},
		            new[] {"Пн", "Вт", "Ср", "Чт", "Пт", "Сб"}
		        };

                string [][] homeworkText =
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
		            new[] {"Cancel"}
		        };

                string[][] scheduleCallbackData =
		        {
		            new[] {"Mo1", "Tu1", "We1", "Th1", "Fr1", "Sa1"},
		            new[] {"Mo2", "Tu2", "We2", "Th2", "Fr2", "Sa2"}
		        };

		        string[][] homeworkCallbackData =
		        {
		            new[] { "m6?","m5?", "m4?"},
		            new[] {"m3?", "m2?", "m1?"},
		            new[] {"mp?","p1?", "p2?"},
		            new[] {"p3?", "p4?", "p5?"},
		            new[] { "p6?", "p7?" }
                };

                InlineKeyboardMarkup inlineScheduleKeyboard = keybord.GetInlineKeyboard(scheduleText, scheduleCallbackData, 2);
		        InlineKeyboardMarkup inlineHomeworkKeyboard = keybord.GetInlineKeyboard(homeworkText, homeworkCallbackData, 5);
		        InlineKeyboardMarkup homeworkCancelKeyboard = keybord.GetInlineKeyboard(homeworkCancelButton, homeworkCancelCallbackData, 1);

                switch (update.CallbackQuery.Data)
                {
                       /* Обработчики кнопок расписания*/
                    case "Mo1":
                        {
                            string result = ScheduleOnTheDay(chatId, userDb, 1, 1);
                            await botClient.EditMessageTextAsync(chatId,
                  update.CallbackQuery.Message.MessageId, result, replyMarkup: inlineScheduleKeyboard);
                            await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                            break;
                        }
                    case "Tu1":
                        {
                            string result = ScheduleOnTheDay(chatId, userDb, 1, 2);
                            await botClient.EditMessageTextAsync(chatId,
                                update.CallbackQuery.Message.MessageId, result, replyMarkup: inlineScheduleKeyboard);
                            await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                            break;
                        }
                    case "We1":
                        {
                            string result = ScheduleOnTheDay(chatId, userDb, 1, 3);
                            await botClient.EditMessageTextAsync(chatId,
                                update.CallbackQuery.Message.MessageId, result, replyMarkup: inlineScheduleKeyboard);
                            await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                            break;
                        }
                    case "Th1":
                        {
                            string result = ScheduleOnTheDay(chatId, userDb, 1, 4);
                            await botClient.EditMessageTextAsync(chatId,
                                update.CallbackQuery.Message.MessageId, result, replyMarkup: inlineScheduleKeyboard);
                            await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                            break;
                        }
                    case "Fr1":
                        {
                            string result = ScheduleOnTheDay(chatId, userDb, 1, 5);
                            await botClient.EditMessageTextAsync(chatId,
                                update.CallbackQuery.Message.MessageId, result, replyMarkup: inlineScheduleKeyboard);
                            await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                            break;
                        }
                    case "Sa1":
                        {
                            string result = ScheduleOnTheDay(chatId, userDb, 1, 6);
                            await botClient.EditMessageTextAsync(chatId,
                                update.CallbackQuery.Message.MessageId, result, replyMarkup: inlineScheduleKeyboard);
                            await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                            break;
                        }
                    case "Mo2":
                        {
                            string result = ScheduleOnTheDay(chatId, userDb, 2, 1);
                            await botClient.EditMessageTextAsync(chatId,
                        update.CallbackQuery.Message.MessageId, result, replyMarkup: inlineScheduleKeyboard);
                            await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                            break;
                        }
                    case "Tu2":
                        {
                            string result = ScheduleOnTheDay(chatId, userDb, 2, 2);
                            await botClient.EditMessageTextAsync(chatId,
                                update.CallbackQuery.Message.MessageId, result, replyMarkup: inlineScheduleKeyboard);
                            await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                            break;
                        }
                    case "We2":
                        {
                            string result = ScheduleOnTheDay(chatId, userDb, 2, 3);
                            await botClient.EditMessageTextAsync(chatId,
                                update.CallbackQuery.Message.MessageId, result, replyMarkup: inlineScheduleKeyboard);
                            await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                            break;
                        }
                    case "Th2":
                        {
                            string result = ScheduleOnTheDay(chatId, userDb, 2, 4);
                            await botClient.EditMessageTextAsync(chatId,
                                update.CallbackQuery.Message.MessageId, result, replyMarkup: inlineScheduleKeyboard);
                            await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                            break;
                        }
                    case "Fr2":
                        {
                            string result = ScheduleOnTheDay(chatId, userDb, 2, 5);
                            await botClient.EditMessageTextAsync(chatId,
                                update.CallbackQuery.Message.MessageId, result, replyMarkup: inlineScheduleKeyboard);
                            await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                            break;
                        }
                    case "Sa2":
                        {
                            string result = ScheduleOnTheDay(chatId, userDb, 2, 6);
                            await botClient.EditMessageTextAsync(chatId,
                                update.CallbackQuery.Message.MessageId, result, replyMarkup: inlineScheduleKeyboard);
                            await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                            break;
                        }

                    /* Обработчики кнопок Добавления ДЗ*/

                    case "m6":
                    {
                            await botClient.EditMessageTextAsync(chatId,
                  update.CallbackQuery.Message.MessageId, AddHomework(-6), replyMarkup: homeworkCancelKeyboard);
                            await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                            break;
                        }
                    case "m5":
                        {
                            await botClient.EditMessageTextAsync(chatId,
                                update.CallbackQuery.Message.MessageId, AddHomework(-5), replyMarkup: homeworkCancelKeyboard);
                            await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                            break;
                        }
                    case "m4":
                        {
                            await botClient.EditMessageTextAsync(chatId,
                                update.CallbackQuery.Message.MessageId, AddHomework(-4), replyMarkup: homeworkCancelKeyboard);
                            await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                            break;
                        }
                    case "m3":
                        {
                            await botClient.EditMessageTextAsync(chatId,
                                update.CallbackQuery.Message.MessageId, AddHomework(-3), replyMarkup: homeworkCancelKeyboard);
                            await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                            break;
                        }
                    case "m2":
                        {
                            await botClient.EditMessageTextAsync(chatId,
                                update.CallbackQuery.Message.MessageId, AddHomework(-2), replyMarkup: homeworkCancelKeyboard);
                            await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                            break;
                        }
                    case "m1":
                        {
                            await botClient.EditMessageTextAsync(chatId,
                                update.CallbackQuery.Message.MessageId, AddHomework(-1), replyMarkup: homeworkCancelKeyboard);
                            await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                            break;
                        }
                    case "mp":
                        {
                            await botClient.EditMessageTextAsync(chatId,
                                update.CallbackQuery.Message.MessageId, AddHomework(0), replyMarkup: homeworkCancelKeyboard);
                            await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                            break;
                        }
                    case "p1":
                        {
                            await botClient.EditMessageTextAsync(chatId,
                                update.CallbackQuery.Message.MessageId, AddHomework(1), replyMarkup: homeworkCancelKeyboard);
                            await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                            break;
                        }
                    case "p2":
                        {
                            await botClient.EditMessageTextAsync(chatId,
                                update.CallbackQuery.Message.MessageId, AddHomework(2), replyMarkup: homeworkCancelKeyboard);
                            await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                            break;
                        }
                    case "p3":
                        {
                            await botClient.EditMessageTextAsync(chatId,
                                update.CallbackQuery.Message.MessageId, AddHomework(3), replyMarkup: homeworkCancelKeyboard);
                            await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                            break;
                        }
                    case "p4":
                        {
                            await botClient.EditMessageTextAsync(chatId,
                                update.CallbackQuery.Message.MessageId, AddHomework(4), replyMarkup: homeworkCancelKeyboard);
                            await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                            break;
                        }
                    case "p5":
                        {
                            await botClient.EditMessageTextAsync(chatId,
                                update.CallbackQuery.Message.MessageId, AddHomework(5), replyMarkup: homeworkCancelKeyboard);
                            await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                            break;
                        }
                    case "p6":
                    {
                        await botClient.EditMessageTextAsync(chatId,
                            update.CallbackQuery.Message.MessageId, AddHomework(6), replyMarkup: homeworkCancelKeyboard);
                        await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                        break;
                        }
                    case "p7":
                    {
                        await botClient.EditMessageTextAsync(chatId,
                            update.CallbackQuery.Message.MessageId, AddHomework(7), replyMarkup: homeworkCancelKeyboard);
                        await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                        break;
                        }

                    case "Cancel":
                    {
                        Dz = false;
                        Date = String.Empty;
                            await botClient.EditMessageTextAsync(chatId,
                            update.CallbackQuery.Message.MessageId, "Ввод задания отменен");
                        await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                          break;
                    }
                    /* Обработчики кнопок Просмотра ДЗ*/

                    case "m6?":
                    {
                        string result = SendHomework(chatId, -6);
                            if (result == update.CallbackQuery.Message.Text)
                            {
                                await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                                break;
                            }
                            await botClient.EditMessageTextAsync(chatId,
                                update.CallbackQuery.Message.MessageId, result, replyMarkup: inlineHomeworkKeyboard);
                            await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                            break;
                        }
                    case "m5?":
                        {
                            string result = SendHomework(chatId, -5);
                            if (result == update.CallbackQuery.Message.Text)
                            {
                                await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                                break;
                            }
                            await botClient.EditMessageTextAsync(chatId,
                                update.CallbackQuery.Message.MessageId, result, replyMarkup: inlineHomeworkKeyboard);
                            await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                            break;
                        }
                    case "m4?":
                        {
                            string result = SendHomework(chatId, -4);
                            if (result == update.CallbackQuery.Message.Text)
                            {
                                await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                                break;
                            }
                            await botClient.EditMessageTextAsync(chatId,
                                update.CallbackQuery.Message.MessageId, result, replyMarkup: inlineHomeworkKeyboard);
                            await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                            break;
                        }
                    case "m3?":
                        {
                            string result = SendHomework(chatId, -3);
                            if (result == update.CallbackQuery.Message.Text)
                            {
                                await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                                break;
                            }
                            await botClient.EditMessageTextAsync(chatId,
                                update.CallbackQuery.Message.MessageId, result, replyMarkup: inlineHomeworkKeyboard);
                            await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                            break;
                        }
                    case "m2?":
                        {
                            string result = SendHomework(chatId, -2);
                            if (result == update.CallbackQuery.Message.Text)
                            {
                                await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                                break;
                            }
                            await botClient.EditMessageTextAsync(chatId,
                                update.CallbackQuery.Message.MessageId, result, replyMarkup: inlineHomeworkKeyboard);
                            await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                            break;
                        }
                    case "m1?":
                        {
                            string result = SendHomework(chatId, -1);
                            if (result == update.CallbackQuery.Message.Text)
                            {
                                await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                                break;
                            }
                            await botClient.EditMessageTextAsync(chatId,
                                update.CallbackQuery.Message.MessageId, result, replyMarkup: inlineHomeworkKeyboard);
                            await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                            break;
                        }
                    case "mp?":
                        {
                            string result = SendHomework(chatId, 0);
                            if (result == update.CallbackQuery.Message.Text)
                            {
                                await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                                break;
                            }
                            await botClient.EditMessageTextAsync(chatId,
                                update.CallbackQuery.Message.MessageId, result, replyMarkup: inlineHomeworkKeyboard);
                            await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                            break;
                        }
                    case "p1?":
                        {
                            string result = SendHomework(chatId, 1);
                            if (result == update.CallbackQuery.Message.Text)
                            {
                                await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                                break;
                            }
                            await botClient.EditMessageTextAsync(chatId,
                                update.CallbackQuery.Message.MessageId, result, replyMarkup: inlineHomeworkKeyboard);
                            await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                            break;
                        }
                    case "p2?":
                        {
                            string result = SendHomework(chatId, 2);
                            if (result == update.CallbackQuery.Message.Text)
                            {
                                await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                                break;
                            }
                            await botClient.EditMessageTextAsync(chatId,
                                update.CallbackQuery.Message.MessageId, result, replyMarkup: inlineHomeworkKeyboard);
                            await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                            break;
                        }
                    case "p3?":
                        {
                            string result = SendHomework(chatId, 3);
                            if (result == update.CallbackQuery.Message.Text)
                            {
                                await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                                break;
                            }
                            await botClient.EditMessageTextAsync(chatId,
                                update.CallbackQuery.Message.MessageId, result, replyMarkup: inlineHomeworkKeyboard);
                            await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                            break;
                        }
                    case "p4?":
                        {
                            string result = SendHomework(chatId, 4);
                            if (result == update.CallbackQuery.Message.Text)
                            {
                                await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                                break;
                            }
                            await botClient.EditMessageTextAsync(chatId,
                                update.CallbackQuery.Message.MessageId, result, replyMarkup: inlineHomeworkKeyboard);
                            await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                            break;
                        }
                    case "p5?":
                        {
                            string result = SendHomework(chatId, 5);
                            if (result == update.CallbackQuery.Message.Text)
                            {
                                await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                                break;
                            }
                            await botClient.EditMessageTextAsync(chatId,
                                update.CallbackQuery.Message.MessageId, result, replyMarkup: inlineHomeworkKeyboard);
                            await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                            break;
                        }
                    case "p6?":
                        {
                            string result = SendHomework(chatId, 6);
                            if (result == update.CallbackQuery.Message.Text)
                            {
                                await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                                break;
                            }
                            await botClient.EditMessageTextAsync(chatId,
                                update.CallbackQuery.Message.MessageId, result, replyMarkup: inlineHomeworkKeyboard);
                            await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                            break;
                        }
                    case "p7?":
                        {
                            string result = SendHomework(chatId, 7);
                            if (result == update.CallbackQuery.Message.Text)
                            {
                                await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                                break;
                            }
                            await botClient.EditMessageTextAsync(chatId,
                                update.CallbackQuery.Message.MessageId, result, replyMarkup: inlineHomeworkKeyboard);
                            await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                            break;
                        }
                }
            }


            return Ok();
		}

        private string ConvertToCorrectTimeFormat(string time)
        {
            var firstTime = time.Split(" - ").First();
            var secondTime = time.Split(" - ").Last();

            return firstTime.Substring(0, firstTime.LastIndexOf(':')) + " - "                       
                + secondTime.Substring(0, secondTime.LastIndexOf(':'));
        }


        private string ScheduleOnTheDay(long chatId, UserDb userDb, int weekNum, int day)
        {
            string result = "Расписание на "+ ConvertWeekDayToRussian(day);
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

        public string DateConverter(DateTime date)
        {
            string shortdate = date.ToShortDateString();
            string month = shortdate.Split(".")[1];
            string day = shortdate.Split(".")[0];

            return day + "." + month;
        }

        private string SendHomework(long chatId, int daysfromtoday)
        {
            DateTime now = DateTime.Now.Date;
            HomeWorkDB homeWork = new HomeWorkDB();
            string result = "Домашнее задание на ";
            if (daysfromtoday < 0)
                result += DateConverter(now.Subtract(new TimeSpan(-daysfromtoday, 0, 0, 0)))+ "\n \n" + homeWork.GetHomeWork(userDb.CheckUserElements(chatId, "university"),
                    userDb.CheckUserElements(chatId, "facility"), userDb.CheckUserElements(chatId, "course"),
                    userDb.CheckUserElements(chatId, "group"), DateConverter(now.Subtract(new TimeSpan(-daysfromtoday, 0, 0, 0)))) +
                "\nСегодня " + DateConverter(now);
            else if (daysfromtoday == 0)
            {
                result += DateConverter(now)+ "\n \n" + homeWork.GetHomeWork(userDb.CheckUserElements(chatId, "university"),
                              userDb.CheckUserElements(chatId, "facility"), userDb.CheckUserElements(chatId, "course"),
                              userDb.CheckUserElements(chatId, "group"), DateConverter(now)) +
                          "\nСегодня " + DateConverter(now); 
            }
            else if (daysfromtoday > 0)
            {
                result += DateConverter(now.AddDays(daysfromtoday))+ "\n \n" + homeWork.GetHomeWork(userDb.CheckUserElements(chatId, "university"),
                              userDb.CheckUserElements(chatId, "facility"), userDb.CheckUserElements(chatId, "course"),
                              userDb.CheckUserElements(chatId, "group"), DateConverter(now.AddDays(daysfromtoday))) +
                          "\nСегодня " + DateConverter(now);
            }
            return result;
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
	}
}
