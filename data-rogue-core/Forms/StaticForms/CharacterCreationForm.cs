using System;
using System.Collections.Generic;
using data_rogue_core.Activities;
using data_rogue_core.Forms;
using data_rogue_core.Renderers.ConsoleRenderers;

namespace data_rogue_core.Menus.StaticMenus
{
    public static class CharacterCreationForm
    {
        public static FormActivity GetCharacterCreation()
        {
            var form = new Form(
                FormButton.Ok | FormButton.Cancel,
                new FormData {FormDataType = FormDataType.Text, Name = "Name", Value = "PLAYERNAME"}
            );

            return new FormActivity(form, Game.RendererFactory);
        }

        public static void HandleCharacterCreationFormSelection(FormButton button, Form form)
        {
            switch(button)
            {
                case FormButton.Ok:
                    throw new NotImplementedException();
                    break;
                case FormButton.Cancel:
                    throw new NotImplementedException();
                    break;
                default:
                    throw new ApplicationException("Unknown form button");

            }
        }
    }
}