using System.Collections.Generic;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Utils;
using RLNET;

namespace data_rogue_core.IOSystems
{
    internal class RLNetExperienceDisplayer : RLNetStatsRendererHelper
    {
        public override string DisplayType => "Experience";
        protected override void DisplayInternal(RLConsole console, InfoDisplay display, ISystemContainer systemContainer, IEntity player, List<MapCoordinate> playerFov, ref int line)
        {
            var experience = player.Get<Experience>();

            var text1 = $"Level: {experience.Level}";
            var text2 = $"   XP: {experience.Amount}";

            console.Print(1, line++, text1, display.Color.ToRLColor(), display.BackColor.ToRLColor());
            console.Print(1, line++, text2, display.Color.ToRLColor(), display.BackColor.ToRLColor());
        }
    }
}