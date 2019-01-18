using System;
using System.Collections.Generic;
using data_rogue_core.Activities;
using data_rogue_core.Forms;
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
            {"Class", new MultipleChoiceFormData("Warrior", 2, new List<object> { "Warrior", "Wizard" }) }
        };

        public string Name => Fields["Name"].Value.ToString();

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
                    Game.ActivityStack.Pop();
                    Game.StartNewGame(characterCreationForm);
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