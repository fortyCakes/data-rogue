using System.Collections.Generic;
using System.Drawing;

namespace data_rogue_core.IOSystems
{
    public class IOSystemConfiguration
    {
        public int InitialWidth { get; set; }
        public int InitialHeight { get; set; }
        public int TileWidth { get; set; }
        public int TileHeight { get; set; }
        public string WindowTitle { get; set; }
        public List<MapConfiguration> MapConfigurations { get; set; } = new List<MapConfiguration>();
        public List<StatsConfiguration> StatsConfigurations { get; set; } = new List<StatsConfiguration>();
        public List<MessageConfiguration> MessageConfigurations { get; set; } = new List<MessageConfiguration>();
        public IEnumerable<IStatsRendererHelper> AdditionalStatsDisplayers { get; set; } = new List<IStatsRendererHelper>();
    }
}