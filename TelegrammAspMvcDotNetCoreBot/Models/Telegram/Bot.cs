using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using TelegrammAspMvcDotNetCoreBot.Models.Commands;

namespace TelegrammAspMvcDotNetCoreBot.Models.Telegramm
{
    public class Bot
    {
        private static TelegramBotClient _botClient;
        private static List<Command> _commandsList;

        public static IReadOnlyList<Command> Commands => _commandsList.AsReadOnly();

        public static async Task<TelegramBotClient> GetBotClientAsync()
        {
            if (_botClient != null)
            {
                return _botClient;
            }

            _commandsList = new List<Command>();
            _commandsList.Add(new StartCommand());
            //TODO: Add more commands

            _botClient = new TelegramBotClient(Startup.Configuration["ConfigTelegram:Key"]);

			string hook = string.Format(Startup.Configuration["ConfigTelegram:Url"], "api/message/update");
            await _botClient.SetWebhookAsync(hook);
            return _botClient;
        }
    }
}
