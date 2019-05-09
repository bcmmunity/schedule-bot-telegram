using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VkNet.Enums.SafetyEnums;
using VkNet.Model.Keyboard;

namespace TelegrammAspMvcDotNetCoreBot.Logic
{
    public class VkKeyboard
    {
        public MessageKeyboard GetKeyboard(string[][] buttons,string payload = "") //для создания обычных и постраничных клав
        {

            KeyboardBuilder keyboard = new KeyboardBuilder();

            int rows = buttons.Length;

         
            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < buttons[row].Length; column++)
                {
                    if (buttons[row][column].Contains("<"))
                        keyboard.AddButton(buttons[row][column], buttons[row][column].Split(' ')[1] + ";"+payload,KeyboardButtonColor.Primary);
                    else if (buttons[row][column].Contains(">"))
                        keyboard.AddButton(buttons[row][column], buttons[row][column].Split(' ')[0] + ";" + payload,KeyboardButtonColor.Primary);
                    else
                        keyboard.AddButton(buttons[row][column], "");

                }

                keyboard.AddLine();
            }

            return keyboard.Build();
        }

        public MessageKeyboard GetPayloadKeyboard(string[][] buttons, string[][] payload, string today = "")
        {
            KeyboardBuilder keyboard = new KeyboardBuilder();

            int rows = buttons.Length;

            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < buttons[row].Length; column++)
                {
                    if (payload[row][column] == "000")
                        keyboard.AddButton(buttons[row][column], payload[row][column],KeyboardButtonColor.Negative);
                    else if (payload[row][column] == "300" || payload[row][column] == "400" || payload[row][column] == today)
                        keyboard.AddButton(buttons[row][column], payload[row][column], KeyboardButtonColor.Positive);
                    else
                        keyboard.AddButton(buttons[row][column], payload[row][column]);

                }

                keyboard.AddLine();
            }

            return keyboard.Build();
        }
    }
}
