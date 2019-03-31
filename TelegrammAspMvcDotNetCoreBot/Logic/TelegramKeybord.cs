namespace TelegrammAspMvcDotNetCoreBot.Logic
{
    public class TelegramKeybord
    {
		public Telegram.Bot.Types.ReplyMarkups.ReplyKeyboardMarkup GetKeyboard(string[][] buttons)
		{
		    int rows = buttons.Length;

            Telegram.Bot.Types.ReplyMarkups.KeyboardButton[][] keyboardButtons = new Telegram.Bot.Types.ReplyMarkups.KeyboardButton[rows][];

			for (int row = 0; row < rows; row++)
			{
				keyboardButtons[row] = new Telegram.Bot.Types.ReplyMarkups.KeyboardButton[buttons[row].Length];

				for (int column = 0; column < buttons[row].Length; column++)
				{
					keyboardButtons[row][column] = buttons[row][column];
				}
			}



			var keyboard = new Telegram.Bot.Types.ReplyMarkups.ReplyKeyboardMarkup
			{
				Keyboard = keyboardButtons,
				ResizeKeyboard = true
			};

			return keyboard;
		}

        public Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup GetInlineKeyboard(string[][] buttons, string[][] callback_data)
        {
            int rows = buttons.Length;
            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton[][] keyboardButtons = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton[rows][];
            
            for (int row = 0; row < rows; row++)
            {
                keyboardButtons[row] = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton[buttons[row].Length];

                for (int column = 0; column < buttons[row].Length; column++)
                {
                    keyboardButtons[row][column] = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton {Text = buttons[row][column], CallbackData = callback_data[row][column] };
                } 
            }


            var keyboard = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(keyboardButtons);

            return keyboard;
        }
    }
}
