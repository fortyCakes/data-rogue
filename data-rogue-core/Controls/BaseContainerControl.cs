using System.Collections.Generic;
using data_rogue_core.Activities;
using data_rogue_core.IOSystems;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.Controls
{
    public class BaseContainerControl : BaseControl
    {
        public List<IDataRogueControl> Controls { get; set; } = new List<IDataRogueControl>();

        public override void Paint(List<IDataRogueControlRenderer> controlRenderers, object handle, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            foreach(var control in Controls)
            {
                control.Paint(controlRenderers, handle, systemContainer, playerFov);
            }
        }
    }
}