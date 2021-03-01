using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using BLTWrapper;
using data_rogue_core.Activities;

namespace data_rogue_core.IOSystems
{
    public class IOSystemConfiguration
    {
        public int InitialWidth { get; set; }
        public int InitialHeight { get; set; }
        public int TileWidth { get; set; }
        public int TileHeight { get; set; }
        public string WindowTitle { get; set; }
        public List<IDataRogueControl> GameplayWindowControls { get; set; } = new List<IDataRogueControl>();
        public IEnumerable<IDataRogueControlRenderer> AdditionalControlRenderers { get; set; } = new List<IDataRogueControlRenderer>();
        public Rectangle DefaultPosition { get; set; }
        public Padding DefaultPadding { get; set; }
    }
}