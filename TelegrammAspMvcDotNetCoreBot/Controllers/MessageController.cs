using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegrammAspMvcDotNetCoreBot.Logic;
using TelegrammAspMvcDotNetCoreBot.Models;
using TelegrammAspMvcDotNetCoreBot.Models.Telegramm;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

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

        FileToSend facSticker = new FileToSend("CAADAgADBwADi6p7D7JUJy3u1Q22Ag");
		FileToSend courseSticker = new FileToSend("CAADAgADBgADi6p7DxEJvhyK0iHFAg");
		FileToSend groupSticker = new FileToSend("CAADAgADBAADi6p7DzzxU-ilYtP6Ag");
		FileToSend workSticker = new FileToSend("CAADAgADBQADi6p7D849HV-BVKxIAg");
		FileToSend relaxSticker = new FileToSend("CAADAgADAgADi6p7D_SOcGo7bWCIAg");

            if (update == null) return Ok();

			var commands = Bot.Commands;
            var message = update.Message;
            var botClient = await Bot.GetBotClientAsync();

		    var chatId = message.Chat.Id;

		    UserDb userDb = new UserDb();

		    ScheduleDB schedule = new ScheduleDB();

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
		                unn[count] = new[] { item };
		                count++;
		            }

                //await botClient.SendTextMessageAsync(chatId, "Теперь выбери факультет", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: (Telegram.Bot.Types.ReplyMarkups.IReplyMarkup)KeybordController.GetKeyboard(unn, count));
		         await botClient.SendStickerAsync(chatId, facSticker, replyMarkup: TelegramKeybord.GetKeyboard(unn, count));

                return Ok();

		        }

		    if (userDb.CheckUserElements(chatId, "facility") == "" && schedule.IsFacilityExist(userDb.CheckUserElements(chatId, "university"), message.Text))
		    {
		        userDb.EditUser(chatId, "facility", message.Text);

		        List<string> un = schedule.GetCourses(userDb.CheckUserElements(chatId, "university"), userDb.CheckUserElements(chatId, "facility"));

		        string[][] unn = new string[un.ToList().Count][];

		        int count = 0;
		        foreach (string item in un)
		        {
		            unn[count] = new[] { item };
		            count++;
		        }

		        //await botClient.SendTextMessageAsync(chatId, "Теперь выбери курс", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: (Telegram.Bot.Types.ReplyMarkups.IReplyMarkup)KeybordController.GetKeyboard(unn, count));
		        await botClient.SendStickerAsync(chatId, courseSticker, replyMarkup: TelegramKeybord.GetKeyboard(unn, count));
		        return Ok();

		    }

		    if (userDb.CheckUserElements(chatId, "course") == "" && schedule.IsCourseExist(userDb.CheckUserElements(chatId, "university"), userDb.CheckUserElements(chatId, "facility"), message.Text))
		    {
		        userDb.EditUser(chatId, "course", message.Text);

		        List<string> un = schedule.GetGroups(userDb.CheckUserElements(chatId, "university"), userDb.CheckUserElements(chatId, "facility"), userDb.CheckUserElements(chatId, "course"));

		        string[][] unn = new string[un.ToList().Count][];

		        int count = 0;
		        foreach (string item in un)
		        {
		            unn[count] = new[] { item };
		            count++;
		        }

		        //await botClient.SendTextMessageAsync(chatId, "Теперь выбери группу", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: (Telegram.Bot.Types.ReplyMarkups.IReplyMarkup)KeybordController.GetKeyboard(unn, count));
		        await botClient.SendStickerAsync(chatId, groupSticker, replyMarkup: TelegramKeybord.GetKeyboard(unn, count));
		        return Ok();

		    }

		    if (userDb.CheckUserElements(chatId, "group") == "" && schedule.IsGroupExist(userDb.CheckUserElements(chatId, "university"), userDb.CheckUserElements(chatId, "facility"), userDb.CheckUserElements(chatId, "course"), message.Text))
		    {
		        userDb.EditUser(chatId, "group", message.Text);

		        string[][] unn = {
		            new[] {"Сегодня", "Завтра"},
		            new[] { "Сбросить" }
		        };


		        // await botClient.SendTextMessageAsync(chatId, "Отлично, можем работать!", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: (Telegram.Bot.Types.ReplyMarkups.IReplyMarkup)KeybordController.GetKeyboard(unn, 2));
		        await botClient.SendStickerAsync(chatId, workSticker, replyMarkup: TelegramKeybord.GetKeyboard(unn, 2));
		        return Ok();

		    }

		    if (message.Text == "Сегодня" && userDb.CheckUserElements(chatId, "group") != "")
		    {
		        int day;
		        int weekNum = ((CultureInfo.CurrentCulture).Calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday) + 1) % 2 + 1;
		        if ((int)DateTime.Now.DayOfWeek == 0)
		            day = 7;
		        else
		        {
		            day = (int)DateTime.Now.DayOfWeek;
		        }

		        ScheduleDay scheduleDay = schedule.GetSchedule(userDb.CheckUserElements(chatId, "university"), userDb.CheckUserElements(chatId, "facility"), userDb.CheckUserElements(chatId, "course"), userDb.CheckUserElements(chatId, "group"), weekNum, day);

		        List<Lesson> listPar = scheduleDay.Lesson.ToList();

		        string result = "";
       
		        foreach (Lesson item in listPar)
		        {
		            result += item.Number + " пара: " + ConvertToCorrectTimeFormat(item.Time) + "\n" + item.Name + "\n" + item.Room + "\n\n";
		        }
		        if (result != "")
		            await botClient.SendTextMessageAsync(chatId, result, parseMode: ParseMode.Markdown);
		        else

		            //await botClient.SendTextMessageAsync(chatId, "Учебы нет, отдыхай", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
		            await botClient.SendStickerAsync(chatId, relaxSticker);

		        return Ok();

		    }

		    if (message.Text == "Завтра" && userDb.CheckUserElements(chatId, "group") != "")
		    {
		        int day;
		        int weekNum = ((CultureInfo.CurrentCulture).Calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday) + 1) % 2 + 1;
		        if ((int)DateTime.Now.DayOfWeek == 0)
		            day = 1;
		        else
		        {
		            if ((int)DateTime.Now.DayOfWeek == 6)
		                day = 7;
		            else
		                day = ((int)DateTime.Now.DayOfWeek + 1) % 7;
		        }

		        ScheduleDay scheduleDay = schedule.GetSchedule(userDb.CheckUserElements(chatId, "university"), userDb.CheckUserElements(chatId, "facility"), userDb.CheckUserElements(chatId, "course"), userDb.CheckUserElements(chatId, "group"), weekNum, day);

		        List<Lesson> listPar = scheduleDay.Lesson.ToList();

		        string result = "";
		        foreach (Lesson item in listPar)
		        {
		            result += item.Number + " пара: " + ConvertToCorrectTimeFormat(item.Time)+ "\n" + item.Name + "\n" + item.Room + "\n\n";
		        }
		        if (result != "")
		            await botClient.SendTextMessageAsync(chatId, result, parseMode: ParseMode.Markdown);
		        else
		            //await botClient.SendTextMessageAsync(chatId, "Учебы нет, отдыхай", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
		            await botClient.SendStickerAsync(chatId, relaxSticker);

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

		    return Ok();
		}

        private string ConvertToCorrectTimeFormat(string time)
        {
            var firstTime = time.Split(" - ").First();
            var secondTime = time.Split(" - ").Last();

            return firstTime.Substring(0, firstTime.LastIndexOf(':')) + " - "                       
                + secondTime.Substring(0, secondTime.LastIndexOf(':'));
        }



	}
}
