using System;
using System.Collections.Generic;
using System.Drawing;
using data_rogue_core.Activities;
using data_rogue_core.Components;
using data_rogue_core.Controls;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Utils;
using RLNET;

namespace data_rogue_core.IOSystems
{
    internal class RLNetExperienceDisplayer : RLNetControlRenderer
    {
        public override Type DisplayType => typeof(ExperienceControl);

        protected override void DisplayInternal(RLConsole console, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            var display = control as IDataRogueInfoControl;
            var experience = display.Entity.Get<Experience>();

            var text1 = $"Level: {experience.Level}";
            var text2 = $"   XP: {experience.Amount}";

            console.Print(control.Position.X, control.Position.Y, text1, display.Color.ToRLColor(), display.BackColor.ToRLColor());
            console.Print(control.Position.X, control.Position.Y+1, text2, display.Color.ToRLColor(), display.BackColor.ToRLColor());
        }

        protected override Size GetSizeInternal(RLConsole console, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            return new Size(15, 2);
        }
    }
}