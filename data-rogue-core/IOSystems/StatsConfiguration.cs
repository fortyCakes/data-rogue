using System.Collections.Generic;
using System.Drawing;
using data_rogue_core.Renderers.ConsoleRenderers;

namespace data_rogue_core.IOSystems
{
    public class StatsConfiguration : IRendereringConfiguration
    {
        public Rectangle Position { get; set; }
        public List<InfoDisplay> Displays { get; set; } = new List<InfoDisplay>();
    }
}