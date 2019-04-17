using System.Collections.Generic;
using System.Drawing;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Utils;
using RLNET;

namespace data_rogue_core.IOSystems
{
    public class RLNetAppearanceNameDisplayer : RLNetStatsRendererHelper
    {
        public override string DisplayType => "AppearanceName";

        protected override void DisplayInternal(RLConsole console, InfoDisplay display, ISystemContainer systemContainer, IEntity player, List<MapCoordinate> playerFov, ref int line)
        {
            var appearance = player.Has<Appearance>() ? player.Get<Appearance>() : new Appearance {Color = Color.White, Glyph = '?'};

            console.Print(1, line, appearance.Glyph.ToString(), appearance.Color.ToRLColor(), display.BackColor.ToRLColor());
            console.Print(3, line, player.Get<Description>().Name, display.Color.ToRLColor(), display.BackColor.ToRLColor());

            line++;
        }
    }
}