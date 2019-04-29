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
    public class RLNetStatArrayRenderer : RLNetControlRenderer
    {
        public override Type DisplayType => typeof(StatsFormData);

        protected override void DisplayInternal(RLConsole console, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            var statsFormData = control as StatsFormData;
            var stats = statsFormData.Stats;
            var x = control.Position.X;
            var y = control.Position.Y;
            var foreColor = control.Color.ToRLColor();

            console.Print(x + 5, y, "[     /     ]", foreColor);
            console.Print(x + 7, y, statsFormData.CurrentTotalStat.ToString().PadLeft(4), foreColor);
            console.Print(x + 12, y, statsFormData.MaxTotalStat.ToString().PadRight(4), foreColor);
            y += 1;
            var longestStat = stats.Max(s => s.statName.Length);

            foreach (var stat in stats)
            {
                var statSelected = statsFormData.SubSelection == stat.statName;
                var statForeColor = statSelected ? RLColor.Cyan : RLColor.White;

                console.Print(2, y, (stat.statName + ": ").PadRight(longestStat + 2), statForeColor);
                console.Print(2 + longestStat + 2, y, "-", statSelected ? RLColor.Red : RLColor.White);
                console.Print(2 + longestStat + 4, y, stat.statValue.ToString().PadBoth(4), statForeColor);
                console.Print(2 + longestStat + 10, y, "+", statSelected ? RLColor.Green : RLColor.White);

                y += 1;
            }
        }

        protected override Size GetSizeInternal(RLConsole console, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            var statsFormData = control as StatsFormData;
            return new Size(35, statsFormData.Stats.Count + 1);
        }
    }
}
