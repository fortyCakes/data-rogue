using System;
using System.Collections.Generic;
using System.Drawing;
using data_rogue_core.Activities;
using data_rogue_core.IOSystems;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Utils;

namespace data_rogue_core.Controls
{
    public class BaseContainerControl : BaseControl
    {
        public List<IDataRogueControl> Controls { get; set; } = new List<IDataRogueControl>();
        public bool ShrinkToContents = false;

        public override void Paint(List<IDataRogueControlRenderer> controlRenderers, object handle, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            foreach(var control in Controls)
            {
                control.Paint(controlRenderers, handle, systemContainer, playerFov);
            }
        }

        public override bool Layout(List<IDataRogueControlRenderer> controlRenderers, ISystemContainer systemContainer, object handle, List<MapCoordinate> playerFov, Rectangle boundingBox)
        {
            var externalBoundingBox = boundingBox.PadIn(Margin);

            base.Layout(controlRenderers, systemContainer, handle, playerFov, externalBoundingBox);

            var interalBoundingBox = externalBoundingBox.PadIn(Padding);

            var redoLayout = false;

            do
            {
                foreach (var control in Controls)
                {
                    redoLayout |= control.Layout(controlRenderers, systemContainer, handle, playerFov, interalBoundingBox);
                }
            } while (redoLayout);
            
            ApplyAlignmentToContents(boundingBox);

            if (ShrinkToContents)
            {
                CalculateOwnPosition(boundingBox);
            }

            return false;
        }

        protected virtual void CalculateOwnPosition(Rectangle boundingBox)
        {
            Rectangle controlBounding = GetControlBoundingBox();

            Position = controlBounding.PadOut(Padding).PadOut(Margin);
        }

        protected virtual void ApplyAlignmentToContents(Rectangle boundingBox)
        {
            Rectangle controlBounding = GetControlBoundingBox();

            var dx = GetHorizontalAdjustment(boundingBox, controlBounding);
            var dy = GetVerticalAdjustment(boundingBox, controlBounding);

            foreach (var control in Controls)
            {
                AdjustPosition(control, dx, dy);
            }
        }

        protected Rectangle GetControlBoundingBox()
        {
            var xmin = int.MaxValue;
            var ymin = int.MaxValue;
            var xmax = int.MinValue;
            var ymax = int.MinValue;

            foreach (var control in Controls)
            {
                if (control.Position.Left < xmin)
                {
                    xmin = control.Position.Left;
                }
                if (control.Position.Right > xmax)
                {
                    xmax = control.Position.Right;
                }
                if (control.Position.Top < ymin)
                {
                    ymin = control.Position.Top;
                }
                if (control.Position.Bottom > ymax)
                {
                    ymax = control.Position.Bottom;
                }
            }

            var controlBounding = new Rectangle(xmin, ymin, xmax - xmin, ymax - ymin);
            return controlBounding;
        }

        private void AdjustPosition(IDataRogueControl control, int dx, int dy)
        {
            control.MovePosition(dx, dy);
        }

        protected int GetVerticalAdjustment(Rectangle boundingBox, Rectangle controlBounding)
        {
            var dy = 0;

            switch (VerticalAlignment)
            {
                case System.Windows.Forms.VisualStyles.VerticalAlignment.Top:
                    dy = boundingBox.Top - controlBounding.Top;
                    break;
                case System.Windows.Forms.VisualStyles.VerticalAlignment.Bottom:
                    dy = boundingBox.Bottom - controlBounding.Bottom;
                    break;
                case System.Windows.Forms.VisualStyles.VerticalAlignment.Center:
                    var spareHeight = boundingBox.Height - controlBounding.Height;
                    var targetY = spareHeight / 2;
                    dy = targetY - controlBounding.Top;
                    break;
            }

            return dy;
        }

        protected int GetHorizontalAdjustment(Rectangle boundingBox, Rectangle controlBounding)
        {
            var dx = 0;

            switch (HorizontalAlignment)
            {
                case System.Windows.Forms.HorizontalAlignment.Left:
                    dx = boundingBox.Left - controlBounding.Left;
                    break;
                case System.Windows.Forms.HorizontalAlignment.Right:
                    dx = boundingBox.Right - controlBounding.Right;
                    break;
                case System.Windows.Forms.HorizontalAlignment.Center:
                    var spareWidth = boundingBox.Width - controlBounding.Width;
                    var targetX = spareWidth / 2;
                    dx = targetX - controlBounding.Left;
                    break;
            }

            return dx;
        }

        public override void MovePosition(int dx, int dy)
        {
            base.MovePosition(dx, dy);

            foreach(var control in Controls)
            {
                control.MovePosition(dx, dy);
            }
        }
    }
}