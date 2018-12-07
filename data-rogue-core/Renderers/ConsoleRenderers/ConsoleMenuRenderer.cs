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

            Console.Print(1, 1, menu.MenuName, RLColor.White);
            Console.Print(1, 2, new string('-', menu.MenuName.Length), RLColor.White);

            int availableHeight = Console.Height - 6; // Two for top/bottom border, two for title and underline, one for pagination
            int selectedIndex = menu.SelectedIndex;
            int itemCount = menu.MenuItems.Count;
            int pageCount = (itemCount - 1) / availableHeight + 1;
            int page = selectedIndex / availableHeight;

            int menuOffset = 3;
            for (int i = 0; i < availableHeight; i++)
            {
                var displayIndex = page * availableHeight + i;
                if (displayIndex < itemCount)
                {
                    MenuItem item = menu.MenuItems[displayIndex];
                    Console.Print(2, menuOffset + i, item.Text, item.Enabled? RLColor.White : RLColor.Gray);
                }

                if (displayIndex == selectedIndex)
                {
                    Console.Print(1, menuOffset + i, ">", RLColor.White);
                }
            }

            if (pageCount > 1)
            {
                Console.Print(1, 3 + availableHeight, $"(page {page} of {pageCount})", RLColor.White);
            }
        }
    }
}