using data_rogue_core.Extensions;
using data_rogue_core.Menus;
using RLNET;

namespace data_rogue_core.Renderers.ConsoleRenderers
{
    public class ConsoleMenuRenderer : BaseConsoleRenderer, IMenuRenderer
    {
        public ConsoleMenuRenderer(RLConsole console) : base(console)
        {
        }

        public void Render(Menu menu)
        {
            Console.Clear();

            //TODO fix for available height, for now we'll assume we have room.

            Console.Print(1, 1, menu.MenuName, RLColor.White);
            Console.Print(1, 2, new string('-', menu.MenuName.Length), RLColor.White);

            int i = 3;
            foreach(MenuItem item in menu.MenuItems)
            {
                if (item == menu.SelectedItem)
                {
                    Console.Print(1, i, ">", RLColor.White);
                }

                Console.Print(2, i, item.Text, item.Color.ToRLColor());

                i++;
            }
        }
    }
}