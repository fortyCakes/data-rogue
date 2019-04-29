using System;
using System.Collections.Generic;
using System.Linq;
using data_rogue_core.Activities;
using data_rogue_core.Controls;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Utils;

namespace data_rogue_core.IOSystems.BLTTiles
{
    internal class BLTStatInterpolationDisplayer : BLTBaseTextDisplayer
    {
        public override Type DisplayType => typeof(StatInterpolationControl);

        protected override string GetText(IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            var display = control as IDataRogueInfoControl;
            var entity = display.Entity;

            var interpolationSplits = display.Parameters.Split(',');
            var format = interpolationSplits[0];

            var statValues = interpolationSplits.Skip(1).Select(s => systemContainer.EventSystem.GetStat(entity, s).ToString()).ToArray();

            return string.Format(format, statValues);
        }
    }
}