using System;
using data_rogue_core.Extensions;
using RLNET;

namespace data_rogue_core
{
    public class ConsoleMenuRenderer
    {
        public static void Render(RLConsole console, Menu menu)
        {
            console.Clear();

            //TODO fix for available height, for now we'll assume we have room.

            console.Print(1, 1, menu.MenuName, RLColor.White);
            console.Print(1, 2, new string('-', menu.MenuName.Length), RLColor.White);

            int i = 3;
            foreach(MenuItem item in menu.MenuItems)
            {
                if (item == menu.SelectedItem)
                {
                    console.Print(1, i, ">", RLColor.White);
                }

                console.Print(2, i, item.Text, item.Color.ToRLColor());

                i++;
            }
        }
    }
}