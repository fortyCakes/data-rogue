using data_rogue_core.Controls;
using System.Collections.Generic;
using System.Drawing;

namespace data_rogue_core.IOSystems
{
    public class MapEditorTargetingConfiguration : IRenderingConfiguration
    {
        public Rectangle Position { get; set; }

        public virtual IEnumerable<InfoDisplay> Displays => new List<InfoDisplay> { new InfoDisplay { ControlType = typeof(MapEditorHighlightControl) } };
    }
}