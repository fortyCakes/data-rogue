using System;
using data_rogue_core.Controls;

namespace data_rogue_core.IOSystems.BLTTiles
{
    public class BLTMapEditorRenderer : BLTMapRenderer
    {
        public override Type DisplayType => typeof(MapEditorControl);
    }
}