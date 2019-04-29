using System;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Utils;
using RLNET;
using System.Collections.Generic;
using data_rogue_core.Controls;
using data_rogue_core.Activities;
using System.Drawing;
using System.Linq;

namespace data_rogue_core.IOSystems
{
    public class RLNetDescriptionDisplayer : RLNetControlRenderer
    {
        public override Type DisplayType => typeof(DescriptionControl);

        protected override void DisplayInternal(RLConsole console, IDataRogueControl display, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            var entity = (display as IDataRogueInfoControl).Entity;
            console.Print(display.Position.X, display.Position.Y, entity.Get<Description>().Detail, display.Color.ToRLColor(), display.BackColor.ToRLColor());
        }

        protected override Size GetSizeInternal(RLConsole console, IDataRogueControl display, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            var entity = (display as IDataRogueInfoControl).Entity;
            var text = entity.Get<Description>().Detail;
            var lines = text.Count(c => c == '\n') + 1;
            var longestLine = text.Split('\n').Max(l => l.Length);
            return new Size(longestLine, lines);
        }
    }
}
