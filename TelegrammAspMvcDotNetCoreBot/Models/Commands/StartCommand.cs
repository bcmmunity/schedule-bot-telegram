using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegrammAspMvcDotNetCoreBot.Controllers;

namespace TelegrammAspMvcDotNetCoreBot.Models.Commands
{
    public class StartCommand : Command
    {
        public override string Name => @"/start";

        public override bool Contains(Message message)
        {
            if (message.Type != Telegram.Bot.Types.Enums.MessageType.TextMessage)
                return false;

            return message.Text.Contains(this.Name);
        }

        public override async Task Execute(Message message, TelegramBotClient botClient)
		{
			ScheduleController.Unit();
			List<string> un = ScheduleController.GetUniversities();
			
			string[][] unn = new string[un.ToList().Count][];
			
			int count = 0;
			foreach (string item in un)
			{
				unn[count] = new string[] { item };
				count++;
			}

			var chatId = message.Chat.Id;

			if (!UserController.CheckUser(chatId))
				UserController.CreateUser(chatId);
			else
				UserController.RecreateUser(chatId);
			//await botClient.SendTextMessageAsync(chatId, "Hallo I'm ASP.NET Core Bot and I made by Mr.Robot", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
			await botClient.SendTextMessageAsync(chatId, "Привет, выбери свой университет", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: (Telegram.Bot.Types.ReplyMarkups.IReplyMarkup) KeybordController.GetKeyboard(unn, count));
		}
	}
}
