using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Forms.StaticForms;
using data_rogue_core.Activities;
using System.Linq;
using data_rogue_core.Maps;
using System.Windows.Forms;

namespace data_rogue_core.Menus.StaticMenus
{
    public class MainMenu : Menu
    {
        public override bool Centred => true;

        private readonly ISystemContainer _systemContainer;

        public MainMenu(ISystemContainer systemContainer) : base(
            systemContainer.ActivitySystem,
            "Main Menu",
            null,
            new MenuItem("New Game") { Margin = new Padding(1) },
            new MenuItem("Load Game") { Margin = new Padding(1) },
            new MenuItem("High Scores") { Margin = new Padding(1) },
            new MenuItem("Map Editor") { Margin = new Padding(1) },
            new MenuItem("Quit") { Margin = new Padding(1) })
        {
            _systemContainer = systemContainer;
            OnSelectCallback += HandleMainMenuSelection;
        }



        public void HandleMainMenuSelection(MenuItem item, MenuAction menuAction)
        {
            switch(item.Text)
            {
                case "Quit":
                    CloseActivity();
                    _activitySystem.QuitAction();
                    break;
                case "New Game":
                    CloseActivity();
                    StartCharacterCreation();
                    break;
                case "Load Game":
                    CloseActivity();
                    _systemContainer.SaveSystem.Load();
                    _activitySystem.GameplayActivity.Running = true;
                    break;
                case "High Scores":
                    _activitySystem.Push(new HighScoresActivity(_activitySystem, _systemContainer.SaveSystem));
                    break;
                case "Map Editor":
                    _activitySystem.Push(new MapEditorActivity(_systemContainer.ActivitySystem.DefaultPosition, _systemContainer.ActivitySystem.DefaultPadding, _systemContainer));
                    break;

            }
        }

        private void StartCharacterCreation()
        {
            _activitySystem.Push(CharacterCreationForm.GetCharacterCreationActivity(_systemContainer));
        }
    }
}