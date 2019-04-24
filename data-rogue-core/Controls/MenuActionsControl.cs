using System.Collections.Generic;
using System.Drawing;
using data_rogue_core.Menus;

namespace data_rogue_core.Controls
{
    public class MenuActionsControl : BaseControl
    {
        public List<MenuAction> AvailableActions { get; set; }
        public MenuAction SelectedAction { get; set; }
        public Color SelectedColor { get; set; }
    }
}