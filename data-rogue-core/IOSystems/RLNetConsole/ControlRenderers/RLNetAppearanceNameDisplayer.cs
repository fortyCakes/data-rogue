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
    public class RLNetAppearanceNameDisplayer : RLNetControlRenderer
    {
        public override Type DisplayType => typeof(AppearanceName);
        protected override void DisplayInternal(RLConsole console, IDataRogueControl display, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            var y = display.Position.Y;
            var entity = (display as IDataRogueInfoControl).Entity;

            var appearance = entity.Has<Appearance>() ? entity.Get<Appearance>() : new Appearance { Color = Color.White, Glyph = '?' };

            console.Print(1, y, appearance.Glyph.ToString(), appearance.Color.ToRLColor(), display.BackColor.ToRLColor());
            console.Print(3, y, entity.Get<Description>().Name, display.Color.ToRLColor(), display.BackColor.ToRLColor());
        }

        protected override Size GetSizeInternal(RLConsole console, IDataRogueControl display, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            return new Size(20, 1);
        }
    }
}