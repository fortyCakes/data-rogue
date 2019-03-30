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
        public Rectangle MapPosition { get; set; }
        public Rectangle StatsPosition { get; set; }
        public Rectangle MessagePosition { get; set; }
    }
}