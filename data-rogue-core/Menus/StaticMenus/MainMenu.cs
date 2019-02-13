using data_rogue_core.Activities;

namespace data_rogue_core.Menus.StaticMenus
{
    public class MainMenu : Menu
    {
        public MainMenu() : base(
            "Main Menu", 
            HandleMainMenuSelection, 
            new MenuItem("New Game"),
            new MenuItem("Load Game"),
            new MenuItem("Quit"))
        {

        }

        public static void HandleMainMenuSelection(MenuItem item, MenuAction menuAction)
        {
            switch(item.Text)
            {
                case "Quit":
                    Game.ActivityStack.Pop();
                    Game.Quit();
                    break;
                case "New Game":
                    Game.ActivityStack.Pop();
                    Game.CreateCharacter();
                    break;
                case "Load Game":
                    Game.ActivityStack.Pop();
                    Game.LoadGame();
                    break;

            }
        }
    }
}