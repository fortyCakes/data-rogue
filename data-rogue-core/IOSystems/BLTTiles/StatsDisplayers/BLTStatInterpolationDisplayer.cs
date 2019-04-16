using System.Collections.Generic;
using System.Linq;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Utils;

namespace data_rogue_core.IOSystems.BLTTiles
{
    internal class BLTStatInterpolationDisplayer : BLTStatsRendererHelper
    {
        public override string DisplayType => "StatInterpolation";

        protected override void DisplayInternal(int x, ISpriteManager spriteManager, StatsDisplay display, ISystemContainer systemContainer, IEntity entity, List<MapCoordinate> playerFov, ref int y)
        {
            var interpolationSplits = display.Parameters.Split(',');
            var format = interpolationSplits[0];

            var statValues = interpolationSplits.Skip(1).Select(s => systemContainer.EventSystem.GetStat(entity, s).ToString()).ToArray();

            var interpolated = string.Format(format, statValues);

            RenderText(x, ref y, interpolated, display.Color);
        }
    }
}