using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Forms.StaticForms;

namespace data_rogue_core.Menus.StaticMenus
{
    public class MainMenu : Menu
    {
        public override bool Centred => true;

        private readonly IPlayerSystem _playerSystem;
        private readonly ISaveSystem _saveSystem;
        private readonly IEntityEngine _entityEngine;

        public MainMenu(IActivitySystem activitySystem, IPlayerSystem playerSystem, ISaveSystem saveSystem, IEntityEngine entityEngine) : base(
            activitySystem,
            "Main Menu",
            null,
            new MenuItem("New Game"),
            new MenuItem("Load Game"),
            new MenuItem("Quit"))
        {
            _playerSystem = playerSystem;
            _saveSystem = saveSystem;
            _entityEngine = entityEngine;
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
                    _saveSystem.Load();
                    break;

            }
        }

        private void StartCharacterCreation()
        {
            _activitySystem.Push(CharacterCreationForm.GetCharacterCreationActivity(_activitySystem, _saveSystem, _playerSystem, _entityEngine));
        }
    }
}