using System.Drawing;

namespace data_rogue_core.IOSystems
{
    public class MessageConfiguration : IRenderingConfiguration
    {
        public Rectangle Position { get; set; }

        public int NumberOfMessages { get; set; }
    }
}