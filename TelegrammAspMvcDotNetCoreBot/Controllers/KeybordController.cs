using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TelegrammAspMvcDotNetCoreBot.Controllers
{
    public static class KeybordController
    {
		public static Telegram.Bot.Types.ReplyMarkups.ReplyKeyboardMarkup GetKeyboard(string[][] buttons, int rows)
		{
			Telegram.Bot.Types.KeyboardButton[][] KeyboardButtons = new Telegram.Bot.Types.KeyboardButton[rows][];

			for (int row = 0; row < rows; row++)
			{
				KeyboardButtons[row] = new Telegram.Bot.Types.KeyboardButton[buttons[row].Length];

				for (int column = 0; column < buttons[row].Length; column++)
				{
					KeyboardButtons[row][column] = buttons[row][column];
				}
			}



			var keyboard = new Telegram.Bot.Types.ReplyMarkups.ReplyKeyboardMarkup
			{
				Keyboard = KeyboardButtons,
				ResizeKeyboard = true
			};

			return keyboard;
		}
	}
}
