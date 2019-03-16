using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using TelegrammAspMvcDotNetCoreBot.Logic;
using TelegrammAspMvcDotNetCoreBot.Models;
using TelegrammAspMvcDotNetCoreBot.Models.Telegramm;

namespace TelegrammAspMvcDotNetCoreBot.Controllers
{
    [Route("api/message/update")]
    public class MessageController : Controller
    {
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

		    var message = update.Message;
            var botClient = await Bot.GetBotClientAsync();
		    var chatId = message.Chat.Id;

		    UserDb userDb = new UserDb();


            if (update.Type == UpdateType.Message)
		    {
		        await botClient.SendTextMessageAsync(chatId, "We are in messages", ParseMode.Markdown);

                InputOnlineFile facSticker = new InputOnlineFile("CAADAgADBwADi6p7D7JUJy3u1Q22Ag");
		        InputOnlineFile courseSticker = new InputOnlineFile("CAADAgADBgADi6p7DxEJvhyK0iHFAg");
		        InputOnlineFile groupSticker = new InputOnlineFile("CAADAgADBAADi6p7DzzxU-ilYtP6Ag");
		        InputOnlineFile workSticker = new InputOnlineFile("CAADAgADBQADi6p7D849HV-BVKxIAg");
		        InputOnlineFile relaxSticker = new InputOnlineFile("CAADAgADAgADi6p7D_SOcGo7bWCIAg");

		        if (update == null) return Ok();

		        var commands = Bot.Commands;

		        ScheduleDB schedule = new ScheduleDB();

		        TelegramKeybord keybord = new TelegramKeybord();

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

		            string[][] unn =
		            {
		                new[] {"Сегодня", "Завтра"},
		                new[] {"Расписание"},
		                new[] {"Сбросить"}
		            };


		            // await botClient.SendTextMessageAsync(chatId, "Отлично, можем работать!", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: (Telegram.Bot.Types.ReplyMarkups.IReplyMarkup)KeybordController.GetKeyboard(unn, 2));
		            await botClient.SendStickerAsync(chatId, workSticker, replyMarkup: keybord.GetKeyboard(unn, 3));
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
		                await botClient.SendTextMessageAsync(chatId, result, parseMode: ParseMode.Markdown);
		            else

		                //await botClient.SendTextMessageAsync(chatId, "Учебы нет, отдыхай", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
		                await botClient.SendStickerAsync(chatId, relaxSticker);

		            return Ok();
		        }

		        if (message.Text == "Завтра" && userDb.CheckUserElements(chatId, "group") != "")
		        {
		            int day;
		            int weekNum = ((CultureInfo.CurrentCulture).Calendar.GetWeekOfYear(DateTime.Now,
		                               CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday) + 1) % 2 + 1;
		            if ((int) DateTime.Now.DayOfWeek == 0)
		                day = 1;
		            else
		            {
		                if ((int) DateTime.Now.DayOfWeek == 6)
		                    day = 7;
		                else
		                    day = ((int) DateTime.Now.DayOfWeek + 1) % 7;
		            }

		            string result = ScheduleOnTheDay(chatId, userDb, weekNum, day);

		            if (!result.Equals("Учебы нет"))
		                await botClient.SendTextMessageAsync(chatId, result, ParseMode.Markdown);
		            else
		                //await botClient.SendTextMessageAsync(chatId, "Учебы нет, отдыхай", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
		                await botClient.SendStickerAsync(chatId, relaxSticker);

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
		        await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "We are in callback", ParseMode.Markdown);

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

                await botClient.EditMessageTextAsync(update.CallbackQuery.Message.Chat.Id,
		                    update.CallbackQuery.Message.MessageId, "xxx", replyMarkup: new TelegramKeybord().GetInlineKeyboard(unn, callbackData, 2));

                //await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id,
                //"test", parseMode: ParseMode.Html);
                //await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, "done", cacheTime: 1);

                //if (update.CallbackQuery.Data.Equals("Mo1"))
                //{
                //    // Код определения следующего рецепта...
                //    await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id,
                //    "test", parseMode: ParseMode.Html);
                //}

                //switch (update.CallbackQuery.Data)  
                //{
                //    case "Mo1":
                //    {
                //        string result = ScheduleOnTheDay(chatId, userDb, 1, 1);
                //                      await botClient.EditMessageTextAsync(update.CallbackQuery.Message.Chat.Id,
                //            update.CallbackQuery.Message.MessageId, result);
                //              break;
                //    }
                //    case "Tu1":
                //    {
                //        string result = ScheduleOnTheDay(chatId, userDb, 1, 2);
                //        await botClient.EditMessageTextAsync(update.CallbackQuery.Message.Chat.Id,
                //            update.CallbackQuery.Message.MessageId, result);
                //        break;
                //    }
                //    case "We1":
                //    {
                //        string result = ScheduleOnTheDay(chatId, userDb, 1, 3);
                //        await botClient.EditMessageTextAsync(update.CallbackQuery.Message.Chat.Id,
                //            update.CallbackQuery.Message.MessageId, result);
                //        break;
                //    }
                //    case "Th1":
                //    {
                //        string result = ScheduleOnTheDay(chatId, userDb, 1, 4);
                //        await botClient.EditMessageTextAsync(update.CallbackQuery.Message.Chat.Id,
                //            update.CallbackQuery.Message.MessageId, result);
                //        break;
                //    }
                //    case "Fr1":
                //    {
                //        string result = ScheduleOnTheDay(chatId, userDb, 1, 5);
                //        await botClient.EditMessageTextAsync(update.CallbackQuery.Message.Chat.Id,
                //            update.CallbackQuery.Message.MessageId, result);
                //        break;
                //    }
                //    case "Sa1":
                //    {
                //        string result = ScheduleOnTheDay(chatId, userDb, 1, 6);
                //        await botClient.EditMessageTextAsync(update.CallbackQuery.Message.Chat.Id,
                //            update.CallbackQuery.Message.MessageId, result);
                //        break;
                //    }
                //              case "Mo2":
                //                  {
                //                      string result = ScheduleOnTheDay(chatId, userDb, 2, 1);
                //                      await botClient.EditMessageTextAsync(update.CallbackQuery.Message.Chat.Id,
                //                  update.CallbackQuery.Message.MessageId, result);
                //                      break;
                //                  }
                //              case "Tu2":
                //                  {
                //                      string result = ScheduleOnTheDay(chatId, userDb, 2, 2);
                //                      await botClient.EditMessageTextAsync(update.CallbackQuery.Message.Chat.Id,
                //                          update.CallbackQuery.Message.MessageId, result);
                //                      break;
                //                  }
                //              case "We2":
                //                  {
                //                      string result = ScheduleOnTheDay(chatId, userDb, 2, 3);
                //                      await botClient.EditMessageTextAsync(update.CallbackQuery.Message.Chat.Id,
                //                          update.CallbackQuery.Message.MessageId, result);
                //                      break;
                //                  }
                //              case "Th2":
                //                  {
                //                      string result = ScheduleOnTheDay(chatId, userDb, 2, 4);
                //                      await botClient.EditMessageTextAsync(update.CallbackQuery.Message.Chat.Id,
                //                          update.CallbackQuery.Message.MessageId, result);
                //                      break;
                //                  }
                //              case "Fr2":
                //                  {
                //                      string result = ScheduleOnTheDay(chatId, userDb, 2, 5);
                //                      await botClient.EditMessageTextAsync(update.CallbackQuery.Message.Chat.Id,
                //                          update.CallbackQuery.Message.MessageId, result);
                //                      break;
                //                  }
                //              case "Sa2":
                //                  {
                //                      string result = ScheduleOnTheDay(chatId, userDb, 2, 6);
                //                      await botClient.EditMessageTextAsync(update.CallbackQuery.Message.Chat.Id,
                //                          update.CallbackQuery.Message.MessageId, result);
                //                      break;
                //                  }
                //          }
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
            ScheduleDB schedule = new ScheduleDB();

		                ScheduleDay scheduleDay = schedule.GetSchedule(userDb.CheckUserElements(chatId, "university"),
		                    userDb.CheckUserElements(chatId, "facility"), userDb.CheckUserElements(chatId, "course"),
		                    userDb.CheckUserElements(chatId, "group"), weekNum, day);

		                List<Lesson> listPar = scheduleDay.Lesson.ToList();

		                string result = "";
		                foreach (Lesson item in listPar)
		                {
		                    result += item.Number + " пара: " + ConvertToCorrectTimeFormat(item.Time) + "\n" + item.Name +
		                              "\n" + item.Room + "\n\n";
		                }

            if (result != "")
                return result;
           
            return "Учебы нет";
        }
	}
}
