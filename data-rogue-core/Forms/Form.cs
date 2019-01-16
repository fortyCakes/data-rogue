using System.Collections.Generic;
using System.Linq;
using data_rogue_core.Menus.StaticMenus;

namespace data_rogue_core.Renderers.ConsoleRenderers
{
    public class Form
    {
        public FormButton Buttons { get; }
        public IEnumerable<FormData> FormData { get; set; }

        public Form(FormButton buttons, params FormData[] formData)
        {
            Buttons = buttons;
            FormData = formData.ToList();
        }
    }
}