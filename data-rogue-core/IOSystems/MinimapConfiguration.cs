using data_rogue_core.Controls;
using System.Collections.Generic;

namespace data_rogue_core.IOSystems
{
    public class MinimapConfiguration : MapConfiguration
    {
        public override IEnumerable<InfoDisplay> Displays => new List<InfoDisplay> { new InfoDisplay { ControlType = typeof(MinimapControl) } };
    }
}