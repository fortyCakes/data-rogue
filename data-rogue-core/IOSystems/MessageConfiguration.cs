using data_rogue_core.Controls;
using System.Collections.Generic;
using System.Drawing;

namespace data_rogue_core.IOSystems
{
    public class MessageConfiguration : IRenderingConfiguration
    {
        public Rectangle Position { get; set; }

        public int NumberOfMessages { get; set; }

        public IEnumerable<InfoDisplay> Displays => new List<InfoDisplay> { new InfoDisplay { ControlType = typeof(MessageLogControl), Parameters = NumberOfMessages.ToString() } };
    }
}