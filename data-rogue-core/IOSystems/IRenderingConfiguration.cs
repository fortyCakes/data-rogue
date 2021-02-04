using System.Collections.Generic;
using System.Drawing;

namespace data_rogue_core.IOSystems
{
    public interface IRenderingConfiguration
    {
        Rectangle Position { get; set; }

        IEnumerable<InfoDisplay> Displays { get; }
    }
}