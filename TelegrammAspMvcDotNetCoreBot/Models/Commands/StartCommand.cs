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
        private FileToSend _uniSticker = new FileToSend("CAADAgADCAADi6p7DL84SgdFjxAg");

        public override string Name => @"/start";

        public override bool Contains(Message message)
        {
            if (message.Type != Telegram.Bot.Types.Enums.MessageType.TextMessage)
                return false;

            return message.Text.Contains(this.Name);
        }

        public override async Task Execute(Message message, TelegramBotClient botClient)
		{
            List<string> un = new ScheduleController().GetUniversities();

            string[][] unn = new string[un.ToList().Count][];

            int count = 0;
            foreach (string item in un)
            {
                unn[count] = new string[] { item };
                count++;
            }

            var chatId = message.Chat.Id;

            UserDb userDb = new UserDb();

            if (!userDb.CheckUser(chatId))
                userDb.CreateUser(chatId);
            else
                userDb.RecreateUser(chatId);

            //await botClient.SendTextMessageAsync(chatId, "Hallo I'm ASP.NET Core Bot and I made by Mr.Robot", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
            await botClient.SendTextMessageAsync(chatId, "Привет, выбери свой университет", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: (Telegram.Bot.Types.ReplyMarkups.IReplyMarkup)KeybordController.GetKeyboard(unn, count));
		   // await botClient.SendStickerAsync(message.Chat.Id, _uniSticker, replyMarkup: KeybordController.GetKeyboard(unn, count));
		    //var chatId = message.Chat.Id;
		    //var messageId = message.MessageId;

		    //await botClient.SendTextMessageAsync(chatId, "Hello!", replyToMessageId: messageId);
		}
	}
}
