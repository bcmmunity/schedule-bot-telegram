using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using TelegrammAspMvcDotNetCoreBot.DB;
using TelegrammAspMvcDotNetCoreBot.Logic;

namespace TelegrammAspMvcDotNetCoreBot.Models.Commands
{
    public class StartCommand : Command
    {
        private readonly InputOnlineFile _uniSticker = new InputOnlineFile("CAADAgADCAADi6p7D_L_84SgdFjxAg");

        public override string Name => @"/start";

        public override bool Contains(Message message)
        {
            if (message.Type != Telegram.Bot.Types.Enums.MessageType.Text)
                return false;

            return message.Text.Contains(Name);
        }

        public override async Task Execute(Message message, TelegramBotClient botClient)
		{
            List<string> un = new ScheduleDB().GetUniversities();

            string[][] unn = new string[un.ToList().Count][];

            int count = 0;
            foreach (string item in un)
            {
                unn[count] = new[] { item };
                count++;
            }

            var chatId = message.Chat.Id;

            UserDb userDb = new UserDb();

            if (!userDb.CheckUser(chatId))
                userDb.CreateUser(chatId);
            else
                userDb.RecreateUser(chatId);

		    //await botClient.SendTextMessageAsync(chatId, "Привет, выбери свой университет", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: (Telegram.Bot.Types.ReplyMarkups.IReplyMarkup)KeybordController.GetKeyboard(unn, count));

		    await botClient.SendStickerAsync(chatId, _uniSticker, replyMarkup: new TelegramKeybord().GetKeyboard(unn));

        }
	}
}
