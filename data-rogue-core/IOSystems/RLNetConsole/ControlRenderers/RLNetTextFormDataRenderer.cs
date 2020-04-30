using System;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Utils;
using RLNET;
using System.Collections.Generic;
using System.Drawing;
using data_rogue_core.Activities;
using data_rogue_core.Forms.StaticForms;
using data_rogue_core.Forms;

namespace data_rogue_core.IOSystems
{
    public class RLNetTextFormDataRenderer : RLNetControlRenderer
    {
        public override Type DisplayType => typeof(TextFormData);

        protected override void DisplayInternal(RLConsole console, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            var formData = control as TextFormData;
            console.Print(control.Position.X, control.Position.Y, ((string)formData.Value).PadRight(30, '_'), formData.IsFocused ? RLColor.Cyan : RLColor.White);
        }

        protected override Size GetSizeInternal(RLConsole console, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            return new Size(30, 1);
        }
    }
}
