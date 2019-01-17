using System;
using data_rogue_core.Activities;
using data_rogue_core.Forms;

namespace data_rogue_core.Menus.StaticMenus
{
    public static class CharacterCreationForm
    {
        public static FormActivity GetCharacterCreation()
        {
            var form = new Form(
                "Character Creation",
                FormButton.Ok | FormButton.Cancel,
                HandleCharacterCreationFormSelection,
                new FormData {FormDataType = FormDataType.Text, Name = "Name", Value = "Rodney"}
            );

            return new FormActivity(form, Game.RendererFactory);
        }

        public static void HandleCharacterCreationFormSelection(FormButton button, Form form)
        {
            switch(button)
            {
                case FormButton.Ok:
                    Game.ActivityStack.Pop();
                    Game.StartNewGame(form);
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