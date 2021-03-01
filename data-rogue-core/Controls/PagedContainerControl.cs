using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using data_rogue_core.IOSystems;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.Controls
{
    public class PagedContainerControl : BaseContainerControl
    {
        public FlowDirection FlowDirection { get; set; } = FlowDirection.TopDown;

        public override bool Layout(List<IDataRogueControlRenderer> controlRenderers, ISystemContainer systemContainer, object handle, List<MapCoordinate> playerFov, Rectangle boundingBox)
        {
            throw new NotImplementedException();
            return base.Layout(controlRenderers, systemContainer, handle, playerFov, boundingBox);
        }

        public override void Paint(List<IDataRogueControlRenderer> controlRenderers, object handle, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            throw new NotImplementedException();
            base.Paint(controlRenderers, handle, systemContainer, playerFov);
        }
    }
}
