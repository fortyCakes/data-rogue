using data_rogue_core.Activities;

namespace data_rogue_core.Menus.StaticMenus
{
    public static class MainMenu
    {
        public static MenuActivity GetMainMenu()
        {
            var menu = new Menu("Main Menu", HandleMainMenuSelection,
                new MenuItem("New Game"),
                new MenuItem("Load Game", false),
                new MenuItem("Quit")
            );

            return new MenuActivity(menu, Game.RendererFactory);
        }

        public static void HandleMainMenuSelection(MenuItem item)
        {
            switch(item.Text)
            {
                case "Quit":
                    Game.ActivityStack.Pop();
                    Game.Quit();
                    break;
                case "New Game":
                    Game.ActivityStack.Pop();
                    Game.StartNewGame();
                    break;

            }
        }
    }
}