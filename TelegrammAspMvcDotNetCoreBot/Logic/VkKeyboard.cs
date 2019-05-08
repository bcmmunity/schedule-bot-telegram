using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VkNet.Model.Keyboard;

namespace TelegrammAspMvcDotNetCoreBot.Logic
{
    public class VkKeyboard
    {
        public MessageKeyboard GetKeyboard(string[][] buttons)
        {

            KeyboardBuilder keyboard = new KeyboardBuilder();

            int rows = buttons.Length;

         
            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < buttons[row].Length; column++)
                {
                    keyboard.AddButton(buttons[row][column],"");
                }

                keyboard.AddLine();
            }

            return keyboard.Build();
        }
    }
}
