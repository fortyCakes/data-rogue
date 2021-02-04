using System.Collections.Generic;
using System.Drawing;
using BLTWrapper;

namespace data_rogue_core.IOSystems
{
    public class IOSystemConfiguration
    {
        public int InitialWidth { get; set; }
        public int InitialHeight { get; set; }
        public int TileWidth { get; set; }
        public int TileHeight { get; set; }
        public string WindowTitle { get; set; }
        public List<IRenderingConfiguration> GameplayRenderingConfiguration { get; set; } = new List<IRenderingConfiguration>();
        public IEnumerable<IDataRogueControlRenderer> AdditionalControlRenderers { get; set; } = new List<IDataRogueControlRenderer>();
    }
}