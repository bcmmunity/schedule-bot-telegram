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
            ResponseBulder response = new ResponseBulder("Telegram");

            var chatId = message.Chat.Id;
            var universities = response.UniversitiesList(chatId);

		    //await botClient.SendTextMessageAsync(chatId, "Привет, выбери свой университет", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: (Telegram.Bot.Types.ReplyMarkups.IReplyMarkup)KeybordController.GetKeyboard(unn, count));

		    await botClient.SendStickerAsync(chatId, _uniSticker, replyMarkup: new TelegramKeyboard().GetKeyboard(universities));

        }
	}
}
