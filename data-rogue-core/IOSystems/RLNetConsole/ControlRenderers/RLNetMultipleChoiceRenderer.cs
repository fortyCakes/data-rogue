using System;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Utils;
using RLNET;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using data_rogue_core.Activities;
using data_rogue_core.Controls;
using data_rogue_core.Forms.StaticForms;
using data_rogue_core.Forms;

namespace data_rogue_core.IOSystems
{
    public class RLNetMultipleChoiceRenderer : RLNetControlRenderer
    {
        public override Type DisplayType => typeof(MultipleChoiceFormData);

        protected override void DisplayInternal(RLConsole console, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            var x = control.Position.X;
            var yCoordinate = control.Position.Y;
            var formData = control as MultipleChoiceFormData;
            var foreColor = control.Color.ToRLColor();
            var selected = control.IsFocused;

            console.Print(x + 2, yCoordinate, ((string)formData.Value).PadRight(28, ' '), RLColor.White);
            console.Print(x + 1, yCoordinate, "[", foreColor);
            console.Set(x, yCoordinate, foreColor, null, selected ? 27 : 0);
            console.Set(x + 29, yCoordinate, foreColor, null, selected ? 26 : 0);
            console.Print(x + 28, yCoordinate, "]", foreColor);
        }

        protected override Size GetSizeInternal(RLConsole console, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            return new Size(30, 1);
        }
    }
}
