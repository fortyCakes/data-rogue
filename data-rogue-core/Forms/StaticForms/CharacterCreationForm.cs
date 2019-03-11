using System;
using System.Collections.Generic;
using data_rogue_core.Activities;
using data_rogue_core.Menus.StaticMenus;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.Forms.StaticForms
{
    public class CharacterCreationForm : Form
    {
        private readonly IRendererSystem _rendererSystem;
        private readonly ISaveSystem _saveSystem;
        private readonly IPlayerSystem _playerSystem;

        public CharacterCreationForm(IActivitySystem activitySystem, IRendererSystem rendererSystem, ISaveSystem saveSystem, IPlayerSystem playerSystem) : base(activitySystem, "Character Creation", FormButton.Ok | FormButton.Cancel,
                null, StaticFields)
        {
            _rendererSystem = rendererSystem;
            _saveSystem = saveSystem;
            _playerSystem = playerSystem;
            OnSelectCallback += HandleCharacterCreationFormSelection;
        }

        public static Dictionary<string, FormData> StaticFields => new Dictionary<string, FormData>
        {
            {"Name", new FormData (FormDataType.Text, "Rodney",1) },
            {"Class", new MultipleChoiceFormData("Warrior", 2, new List<object> { "Warrior", "Wizard" }) },
            {"Stats", new StatsFormData(3, 50, new List<FormStatInformation>
            {
                new FormStatInformation("Muscle", 5, 10, 20),
                new FormStatInformation("Agility", 5, 10, 20),
                new FormStatInformation("Intellect", 5, 10, 20),
                new FormStatInformation("Willpower", 5, 10, 20),
            } ) }
        };

        public string Name => Fields["Name"].Value.ToString();

        public int Muscle => (Fields["Stats"] as StatsFormData).GetStat("Muscle");

        public int Agility => (Fields["Stats"] as StatsFormData).GetStat("Agility");

        public int Intellect => (Fields["Stats"] as StatsFormData).GetStat("Intellect");

        public int Willpower => (Fields["Stats"] as StatsFormData).GetStat("Willpower");

        public static FormActivity GetCharacterCreationActivity(IActivitySystem activitySystem, IRendererSystem rendererSystem, ISaveSystem saveSystem, IPlayerSystem playerSystem)
        {
            return new FormActivity(new CharacterCreationForm(activitySystem, rendererSystem, saveSystem, playerSystem), rendererSystem.RendererFactory);
        }

        public void HandleCharacterCreationFormSelection(FormButton button, Form form)
        {
            var characterCreationForm = (CharacterCreationForm)form;

            switch(button)
            {
                case FormButton.Ok:
                    _saveSystem.Create(characterCreationForm);
                    _activitySystem.Pop();
                    break;
                case FormButton.Cancel:
                    _activitySystem.Pop();
                    _activitySystem.Push(new MenuActivity(new MainMenu(_activitySystem, _playerSystem, _saveSystem, _rendererSystem), _rendererSystem.RendererFactory));
                    break;
                default:
                    throw new ApplicationException("Unknown form button");

            }
        }
    }
}