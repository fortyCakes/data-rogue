using System;
using System.Collections.Generic;
using System.Drawing;
using data_rogue_core.Activities;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.IOSystems;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Utils;

namespace data_rogue_core.Controls
{
    public class BaseContainerControl : BaseControl, IDataRogueParentControl
    {
        public IList<IDataRogueControl> Controls { get; set; } = new List<IDataRogueControl>();
        public bool ShrinkToContents = false;
        public bool ApplyAlignment = false;

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

            if (ApplyAlignment)
            {
                ApplyAlignmentToContents(boundingBox);
            }

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
                if (control.LayoutPosition.Left < xmin)
                {
                    xmin = control.LayoutPosition.Left;
                }
                if (control.LayoutPosition.Right > xmax)
                {
                    xmax = control.LayoutPosition.Right;
                }
                if (control.LayoutPosition.Top < ymin)
                {
                    ymin = control.LayoutPosition.Top;
                }
                if (control.LayoutPosition.Bottom > ymax)
                {
                    ymax = control.LayoutPosition.Bottom;
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

        public void SetUpEntityForInfoControls(IEntity entity)
        {
            foreach (var control in Controls)
            {
                if (control is IDataRogueInfoControl)
                {
                    var infoControl = control as IDataRogueInfoControl;
                    infoControl.Entity = entity;
                }

                if (control is IDataRogueParentControl)
                {
                    var parentControl = control as IDataRogueParentControl;
                    parentControl.SetUpEntityForInfoControls(entity);
                }
            }
        }
    }
}