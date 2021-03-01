using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using BearLib;
using BLTWrapper;
using data_rogue_core.Activities;
using data_rogue_core.Controls;
using data_rogue_core.Forms;
using data_rogue_core.Forms.StaticForms;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Utils;

namespace data_rogue_core.IOSystems.BLTTiles
{
    public class BLTStatsFormRenderer : BLTControlRenderer
    {
        public override Type DisplayType => typeof(StatsFormData);

        protected override void DisplayInternal(ISpriteManager spriteManager, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            var display = control as StatsFormData;
            var x = display.Position.X;
            var y = display.Position.Y;
            
            var stats = display.Stats;

            var statTotal = $"[[{display.CurrentTotalStat.ToString().PadLeft(4)}/{display.MaxTotalStat.ToString().PadRight(4)}]]";

            BLT.Print(16 + x, y, statTotal);

            var longestStat = stats.Max(s => BLT.Measure(s.statName).Width);
            y += 4;

            foreach (var stat in stats)
            {
                var statSelected = display.SubSelection == stat.statName;

                BLT.Print(12, y, (stat.statName + ": ").PadRight(longestStat + 2));
                BLT.Print(12 + longestStat + 4, y, (statSelected ? "[color=red]" : "") + "-");
                BLT.Print(12 + longestStat + 8, y, stat.statValue.ToString().PadBoth(4));
                BLT.Print(12 + longestStat + 16, y, (statSelected ? "[color=green]" : "") + "+");

                if (statSelected)
                {
                    BLTLayers.Set(BLTLayers.UIElements, control.ActivityIndex);
                    BLT.Font("");
                    BLT.Put(6, y, spriteManager.Tile("selector_left", TileDirections.Left));
                    BLTLayers.Set(BLTLayers.Text, control.ActivityIndex);
                    BLT.Font("text");
                }

                y += 4;
            }
        }

        protected override Size LayoutInternal(ISpriteManager spriteManager, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            var display = control as StatsFormData;

            var stats = display.Stats;

            var statTotal = $"[[{display.CurrentTotalStat.ToString().PadLeft(4)}/{display.MaxTotalStat.ToString().PadRight(4)}]]";

            var statTotalSize = BLT.Measure(statTotal);

            var longestStat = stats.Max(s => BLT.Measure(s.statName).Width);
            var longest = Math.Max(longestStat, statTotalSize.Width);
            return new Size(longest, 4 * (stats.Count + 1));
        }
    }
}