namespace TelegrammAspMvcDotNetCoreBot.Logic
{
    public static class TelegramKeybord
    {
		public static Telegram.Bot.Types.ReplyMarkups.ReplyKeyboardMarkup GetKeyboard(string[][] buttons, int rows)
		{
			Telegram.Bot.Types.KeyboardButton[][] keyboardButtons = new Telegram.Bot.Types.KeyboardButton[rows][];

			for (int row = 0; row < rows; row++)
			{
				keyboardButtons[row] = new Telegram.Bot.Types.KeyboardButton[buttons[row].Length];

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
	}
}
