using data_rogue_core.Activities;
using data_rogue_core.Systems.Interfaces;
using System;
using data_rogue_core.Forms.StaticForms;

namespace data_rogue_core.Menus.StaticMenus
{
    public class MainMenu : Menu
    {
        private readonly IPlayerSystem _playerSystem;
        private readonly ISaveSystem _saveSystem;
        private readonly IRendererSystem _rendererSystem;

        public MainMenu(IActivitySystem activitySystem, IPlayerSystem playerSystem, ISaveSystem saveSystem, IRendererSystem rendererSystem) : base(
            activitySystem,
            "Main Menu",
            null,
            new MenuItem("New Game"),
            new MenuItem("Load Game"),
            new MenuItem("Quit"))
        {
            _playerSystem = playerSystem;
            _saveSystem = saveSystem;
            _rendererSystem = rendererSystem;
            OnSelectCallback += HandleMainMenuSelection;
        }



        public void HandleMainMenuSelection(MenuItem item, MenuAction menuAction)
        {
            switch(item.Text)
            {
                case "Quit":
                    _activitySystem.Pop();
                    _rendererSystem.QuitAction();
                    break;
                case "New Game":
                    _activitySystem.Pop();
                    StartCharacterCreation();
                    break;
                case "Load Game":
                    _activitySystem.Pop();
                    _saveSystem.Load();
                    break;

            }
        }

        private void StartCharacterCreation()
        {
            _activitySystem.Push(CharacterCreationForm.GetCharacterCreationActivity(_activitySystem, _rendererSystem, _saveSystem, _playerSystem));
        }
    }
}