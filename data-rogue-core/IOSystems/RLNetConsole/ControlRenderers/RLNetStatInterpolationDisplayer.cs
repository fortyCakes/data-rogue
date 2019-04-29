using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using data_rogue_core.Activities;
using data_rogue_core.Controls;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Utils;
using RLNET;

namespace data_rogue_core.IOSystems
{
    public class RLNetStatInterpolationDisplayer : RLNetControlRenderer
    {
        public override Type DisplayType => typeof(StatInterpolationControl);

        protected override void DisplayInternal(RLConsole console, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            IDataRogueInfoControl display = control as IDataRogueInfoControl;
            var x = control.Position.X;
            string interpolated = DataRogueInfoControl(control, systemContainer, out int y);
            console.Print(x, y, interpolated, display.Color.ToRLColor(), display.BackColor.ToRLColor());
        }

        protected override Size GetSizeInternal(RLConsole console, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            string interpolated = DataRogueInfoControl(control, systemContainer, out int y);
            return new Size(interpolated.Length, 1);
        }

        private static string DataRogueInfoControl(IDataRogueControl control, ISystemContainer systemContainer, out int y)
        {
            IDataRogueInfoControl display = control as IDataRogueInfoControl;
            y = display.Position.Y;
            IEntity entity = display.Entity;

            var interpolationSplits = display.Parameters.Split(',');
            string format = interpolationSplits[0];

            var statValues = interpolationSplits.Skip(1).Select(s => systemContainer.EventSystem.GetStat(entity, s).ToString()).ToArray();

            string interpolated = string.Format(format, statValues);
            return interpolated;
        }
    }
}