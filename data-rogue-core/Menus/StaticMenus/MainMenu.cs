using data_rogue_core.Activities;
using data_rogue_core.Systems.Interfaces;
using System;

namespace data_rogue_core.Menus.StaticMenus
{
    public class MainMenu : Menu
    {
        public MainMenu(IActivitySystem activitySystem) : base(
            activitySystem,
            "Main Menu", 
            HandleMainMenuSelection,
            new MenuItem("New Game"),
            new MenuItem("Load Game"),
            new MenuItem("Quit"))
        {

        }

        public static MenuItemSelected CallSelectionHandler(IActivitySystem activitySystem)
        {
            return (MenuItem item, MenuAction menuAction) => HandleMainMenuSelection(activitySystem, item, menuAction);
        }

        public static void HandleMainMenuSelection(IActivitySystem activitySystem, MenuItem item, MenuAction menuAction)
        {
            switch(item.Text)
            {
                case "Quit":
                    activitySystem.Pop();
                    Game.Quit();
                    break;
                case "New Game":
                    activitySystem.Pop();
                    Game.CreateCharacter();
                    break;
                case "Load Game":
                    activitySystem.Pop();
                    Game.LoadGame();
                    break;

            }
        }
    }
}