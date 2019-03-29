using System.Linq;
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

            PrintTitleBar(menu);

            PrintActions(menu);

            PrintItems(menu);
        }

        private void PrintActions(Menu menu)
        {
            var width = Console.Width;

            var textLength = 2 + menu.AvailableActions.Sum(a => a.ToString().Length + 1);

            var x = width - textLength - 1;

            Console.Print(x, 0, "[", RLColor.White);
            

            foreach (var action in menu.AvailableActions)
            {
                x++;

                var foreColor = action == menu.SelectedAction ? RLColor.White : RLColor.Gray;

                string actionName = action.ToString();
                Console.Print(x, 0, actionName, foreColor);
                x += actionName.Length;
                Console.Print(x, 0, "|", RLColor.White);
            }

            Console.Print(x, 0, "]", RLColor.White);
        }

        private void PrintItems(Menu menu)
        {
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
                    Console.Print(2, menuOffset + i, item.Text, item.Enabled ? RLColor.White : RLColor.Gray);
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

        private void PrintTitleBar(Menu menu)
        {
            Console.Print(1, 1, menu.MenuName, RLColor.White);
            Console.Print(1, 2, new string('-', menu.MenuName.Length), RLColor.White);
        }
    }
}