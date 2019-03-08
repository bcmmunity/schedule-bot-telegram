using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using TelegrammAspMvcDotNetCoreBot.Models;
using System.IO;
using System.Net;
using TelegrammAspMvcDotNetCoreBot.Controllers;
using System.Globalization;

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
			if (update == null) return Ok();

			var commands = Bot.Commands;
            var message = update.Message;
            var botClient = await Bot.GetBotClientAsync();


			foreach (var command in commands)
            {
                if (command.Contains(message))
                {
                    await command.Execute(message, botClient);
					return Ok();
                }
            }

		    //await botClient.SendTextMessageAsync(message.Chat.Id, message.Sticker.FileId, Telegram.Bot.Types.Enums.ParseMode.Markdown);

            UserDb userDb = new UserDb();

            ScheduleController schedule = new ScheduleController();

		    if (!userDb.CheckUser(message.Chat.Id)) return Ok();

		    
		        if (userDb.CheckUserElements(message.Chat.Id, "university") == "" && schedule.IsUniversityExist(message.Text))
		        {
		            userDb.EditUser(message.Chat.Id, "university", message.Text);

		            List<string> un = schedule.GetFacilities(userDb.CheckUserElements(message.Chat.Id, "university"));

		            string[][] unn = new string[un.ToList().Count][];

		            int count = 0;
		            foreach (string item in un)
		            {
		                unn[count] = new string[] { item };
		                count++;
		            }

		            await botClient.SendTextMessageAsync(message.Chat.Id, "Теперь выбери факультет", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: (Telegram.Bot.Types.ReplyMarkups.IReplyMarkup)KeybordController.GetKeyboard(unn, count));

                    return Ok();

		        }
		        else if (userDb.CheckUserElements(message.Chat.Id, "facility") == "" && schedule.IsFacilityExist(userDb.CheckUserElements(message.Chat.Id, "university"), message.Text))
		        {
		            userDb.EditUser(message.Chat.Id, "facility", message.Text);

		            List<string> un = schedule.GetCourses(userDb.CheckUserElements(message.Chat.Id, "university"), userDb.CheckUserElements(message.Chat.Id, "facility"));

		            string[][] unn = new string[un.ToList().Count][];

		            int count = 0;
		            foreach (string item in un)
		            {
		                unn[count] = new string[] { item };
		                count++;
		            }

		            await botClient.SendTextMessageAsync(message.Chat.Id, "Теперь выбери курс", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: (Telegram.Bot.Types.ReplyMarkups.IReplyMarkup)KeybordController.GetKeyboard(unn, count));
		            return Ok();

		        }
		        else if (userDb.CheckUserElements(message.Chat.Id, "course") == "" && schedule.IsCourseExist(userDb.CheckUserElements(message.Chat.Id, "university"), userDb.CheckUserElements(message.Chat.Id, "facility"), message.Text))
		        {
		            userDb.EditUser(message.Chat.Id, "course", message.Text);

		            List<string> un = schedule.GetGroups(userDb.CheckUserElements(message.Chat.Id, "university"), userDb.CheckUserElements(message.Chat.Id, "facility"), userDb.CheckUserElements(message.Chat.Id, "course"));

		            string[][] unn = new string[un.ToList().Count][];

		            int count = 0;
		            foreach (string item in un)
		            {
		                unn[count] = new string[] { item };
		                count++;
		            }

		            await botClient.SendTextMessageAsync(message.Chat.Id, "Теперь выбери группу", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: (Telegram.Bot.Types.ReplyMarkups.IReplyMarkup)KeybordController.GetKeyboard(unn, count));
		            return Ok();

		        }
		        else if (userDb.CheckUserElements(message.Chat.Id, "group") == "" && schedule.IsGroupExist(userDb.CheckUserElements(message.Chat.Id, "university"), userDb.CheckUserElements(message.Chat.Id, "facility"), userDb.CheckUserElements(message.Chat.Id, "course"), message.Text))
		        {
		            userDb.EditUser(message.Chat.Id, "group", message.Text);

		            string[][] unn = new string[][]
		            {
		                new string[] {"Сегодня", "Завтра"},
		                new string[] { "Сбросить" }
		            };


		            await botClient.SendTextMessageAsync(message.Chat.Id, "Отлично, можем работать!", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: (Telegram.Bot.Types.ReplyMarkups.IReplyMarkup)KeybordController.GetKeyboard(unn, 2));
		            return Ok();

		        }
		        else if (message.Text == "Сегодня" && userDb.CheckUserElements(message.Chat.Id, "group") != "")
		        {
		            int day;
		            int weekNum = ((CultureInfo.CurrentCulture).Calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday) + 1) % 2 + 1;
                if ((int)DateTime.Now.DayOfWeek == 0)
		                day = 7;
		            else
		            {
		                day = (int)DateTime.Now.DayOfWeek;
		            }

		            ScheduleDay scheduleDay = schedule.GetSchedule(userDb.CheckUserElements(message.Chat.Id, "university"), userDb.CheckUserElements(message.Chat.Id, "facility"), userDb.CheckUserElements(message.Chat.Id, "course"), userDb.CheckUserElements(message.Chat.Id, "group"), weekNum, day);

		            List<Lesson> listPar = scheduleDay.Lesson.ToList();

		            string result = "";
		            foreach (Lesson item in listPar)
		            {
		                result += item.Number + " пара: " + item.Time + "\n" + item.Name + "\n" + item.Room + "\n\n";
		            }
		            if (result != "")
		                await botClient.SendTextMessageAsync(message.Chat.Id, result, parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
		            else
		                await botClient.SendTextMessageAsync(message.Chat.Id, "Учебы нет, отдыхай", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);

		            return Ok();

		        }
		        else if (message.Text == "Завтра" && userDb.CheckUserElements(message.Chat.Id, "group") != "")
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

		            ScheduleDay scheduleDay = schedule.GetSchedule(userDb.CheckUserElements(message.Chat.Id, "university"), userDb.CheckUserElements(message.Chat.Id, "facility"), userDb.CheckUserElements(message.Chat.Id, "course"), userDb.CheckUserElements(message.Chat.Id, "group"), weekNum, day);

		            List<Lesson> listPar = scheduleDay.Lesson.ToList();

		            string result = "";
		            foreach (Lesson item in listPar)
		            {
		                result += item.Number + " пара: " + item.Time + "\n" + item.Name + "\n" + item.Room + "\n\n";
		            }
		            if (result != "")
		                await botClient.SendTextMessageAsync(message.Chat.Id, result, parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
		            else
		                await botClient.SendTextMessageAsync(message.Chat.Id, "Учебы нет, отдыхай", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);

		            return Ok();

		        }
		        else if (message.Text == "Сбросить")
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
	}
}
