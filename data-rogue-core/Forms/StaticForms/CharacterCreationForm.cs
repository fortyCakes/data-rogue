using System;
using System.Collections.Generic;
using System.Linq;
using data_rogue_core.Activities;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Menus.StaticMenus;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.Forms.StaticForms
{
    public class CharacterCreationForm : Form
    {
        private readonly ISaveSystem _saveSystem;
        private readonly IPlayerSystem _playerSystem;
        private readonly IEntityEngine _entityEngine;

        public CharacterCreationForm(IActivitySystem activitySystem, ISaveSystem saveSystem, IPlayerSystem playerSystem, IEntityEngine entityEngine) : base(activitySystem, "Character Creation", FormButton.Ok | FormButton.Cancel,
                null)
        {
            _entityEngine = entityEngine;
            _saveSystem = saveSystem;
            _playerSystem = playerSystem;

            OnSelectCallback += HandleCharacterCreationFormSelection;

            Fields = CharacterCreationFields;
        }

        public Dictionary<string, FormData> CharacterCreationFields
        {
            get
            {
                var classNames = _entityEngine.EntitiesWith<Class>(true).Select(e => (object)e.DescriptionName).ToList();

                var creationFields = new Dictionary<string, FormData>
                {
                    {"Name", new TextFormData("Name", FormDataType.Text, "Steve", 1)},
                    {"Class", new MultipleChoiceFormData("Class", classNames.First(), 2, classNames) },
                    {
                        "Stats", new StatsFormData("Stats", 3, 50, new List<FormStatInformation>
                        {
                            new FormStatInformation("Muscle", 5, 10, 20),
                            new FormStatInformation("Agility", 5, 10, 20),
                            new FormStatInformation("Intellect", 5, 10, 20),
                            new FormStatInformation("Willpower", 5, 10, 20),
                        })
                    }
                };

                return creationFields;
            }
        }

        public string Name => Fields["Name"].Value.ToString();

        public int Muscle => (Fields["Stats"] as StatsFormData).GetStat("Muscle");

        public int Agility => (Fields["Stats"] as StatsFormData).GetStat("Agility");

        public int Intellect => (Fields["Stats"] as StatsFormData).GetStat("Intellect");

        public int Willpower => (Fields["Stats"] as StatsFormData).GetStat("Willpower");

        public string Class => Fields["Class"].Value.ToString();

        public static FormActivity GetCharacterCreationActivity(IActivitySystem activitySystem, ISaveSystem saveSystem, IPlayerSystem playerSystem, IEntityEngine entityEngine)
        {
            return new FormActivity(new CharacterCreationForm(activitySystem, saveSystem, playerSystem, entityEngine));
        }

        public void HandleCharacterCreationFormSelection(FormButton button, Form form)
        {
            var characterCreationForm = (CharacterCreationForm)form;

            switch(button)
            {
                case FormButton.Ok:
                    CloseActivity();
                    _saveSystem.Create(characterCreationForm);
                    break;
                case FormButton.Cancel:
                    _activitySystem.Pop();
                    _activitySystem.Push(new MenuActivity(new MainMenu(_activitySystem, _playerSystem, _saveSystem, _entityEngine)));
                    break;
                default:
                    throw new ApplicationException("Unknown form button");

            }
        }
    }
}