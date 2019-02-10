using System;
using System.Collections.Generic;
using data_rogue_core.Activities;
using data_rogue_core.Menus.StaticMenus;

namespace data_rogue_core.Forms.StaticForms
{
    public class CharacterCreationForm : Form
    {
        public CharacterCreationForm() : base("Character Creation", FormButton.Ok | FormButton.Cancel,
                HandleCharacterCreationFormSelection, StaticFields)
        {
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

        public static FormActivity GetCharacterCreationActivity()
        {
            return new FormActivity(new CharacterCreationForm(), Game.RendererFactory);
        }

        public static void HandleCharacterCreationFormSelection(FormButton button, Form form)
        {
            var characterCreationForm = (CharacterCreationForm)form;

            switch(button)
            {
                case FormButton.Ok:
                    Game.StartNewGame(characterCreationForm);
                    Game.ActivityStack.Pop();
                    break;
                case FormButton.Cancel:
                    Game.ActivityStack.Pop();
                    Game.ActivityStack.Push(MainMenu.GetMainMenu());
                    break;
                default:
                    throw new ApplicationException("Unknown form button");

            }
        }
    }
}