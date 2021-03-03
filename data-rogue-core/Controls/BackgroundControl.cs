using data_rogue_core.Activities;
using data_rogue_core.IOSystems;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_rogue_core.Controls
{
    public class BackgroundControl : BaseContainerControl
    {
        

        public override bool Layout(List<IDataRogueControlRenderer> controlRenderers, ISystemContainer systemContainer, object handle, List<MapCoordinate> playerFov, Rectangle boundingBox)
        {
            base.Layout(controlRenderers, systemContainer, handle, playerFov, boundingBox);

            return false;
        }


        public override void Paint(List<IDataRogueControlRenderer> controlRenderers, object handle, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            GetCachedRenderer(controlRenderers).Paint(handle, this, systemContainer, playerFov);

            base.Paint(controlRenderers, handle, systemContainer, playerFov);
        }
    }
}
