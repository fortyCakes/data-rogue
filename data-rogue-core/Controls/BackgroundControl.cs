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
        public bool ShrinkToContents = false;

        public override bool Layout(List<IDataRogueControlRenderer> controlRenderers, ISystemContainer systemContainer, object handle, List<MapCoordinate> playerFov, Rectangle boundingBox)
        {
            var internalBoundingBox = boundingBox.Pad(Margin).Pad(Padding);

            var ok = base.Layout(controlRenderers, systemContainer, handle, playerFov, internalBoundingBox);

            if (ShrinkToContents)
            {
                var left = int.MaxValue;
                var right = int.MinValue;
                var top = int.MaxValue;
                var bottom = int.MinValue;

                foreach (var control in Controls)
                {
                    if (control.Position.Left < left) left = control.Position.Left;
                    if (control.Position.Right < right) right = control.Position.Right;
                    if (control.Position.Top < top) top = control.Position.Top;
                    if (control.Position.Bottom < bottom) bottom = control.Position.Bottom;
                }

                Position = new Rectangle(left, top, right - left, bottom - top);
            }

            return ok;
        }


        public override void Paint(List<IDataRogueControlRenderer> controlRenderers, object handle, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            GetCachedRenderer(controlRenderers).Paint(handle, this, systemContainer, playerFov);

            base.Paint(controlRenderers, handle, systemContainer, playerFov);
        }
    }
}
