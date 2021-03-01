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
        private readonly ISystemContainer _systemContainer;

        public CharacterCreationForm(ISystemContainer systemContainer) : base(systemContainer.ActivitySystem, "Character Creation", FormButton.Ok | FormButton.Cancel,
                null)
        {
            _systemContainer = systemContainer;

            OnSelectCallback += HandleCharacterCreationFormSelection;

            Fields = CharacterCreationFields;
        }

        public Dictionary<string, FormData> CharacterCreationFields
        {
            get
            {
                var classNames = _systemContainer.EntityEngine.EntitiesWith<Class>(true).Select(e => (object)e.DescriptionName).ToList();

                var creationFields = new Dictionary<string, FormData>
                {
                    {"Name", new TextFormData("Name", FormDataType.Text, "Rowan", 1)},
                    {"Class", new MultipleChoiceFormData("Class", classNames.First(), 2, classNames) },
                    {
                        "Stats", new StatsFormData("Stats", 3, 50, new List<FormStatInformation>
                        {
                            new FormStatInformation("Muscle", 5, 15, 20),
                            new FormStatInformation("Agility", 5, 15, 20),
                            new FormStatInformation("Intellect", 5, 5, 20),
                            new FormStatInformation("Willpower", 5, 15, 20),
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

        public static FormActivity GetCharacterCreationActivity(ISystemContainer systemContainer)
        {
            return new FormActivity(
                systemContainer.ActivitySystem.DefaultPosition,
                systemContainer.ActivitySystem.DefaultPadding,
                new CharacterCreationForm(systemContainer));
        }

        public void HandleCharacterCreationFormSelection(FormButton button, Form form)
        {
            var characterCreationForm = (CharacterCreationForm)form;

            switch(button)
            {
                case FormButton.Ok:
                    CloseActivity();
                    _systemContainer.SaveSystem.Create(characterCreationForm);
                    break;
                case FormButton.Cancel:
                    _activitySystem.Pop();
                    _activitySystem.Push(new MenuActivity(
                        _systemContainer.ActivitySystem.DefaultPosition,
                        _systemContainer.ActivitySystem.DefaultPadding,
                        new MainMenu(_systemContainer)));
                    break;
                default:
                    throw new ApplicationException("Unknown form button");

            }
        }
    }
}