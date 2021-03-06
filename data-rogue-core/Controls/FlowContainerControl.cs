﻿using data_rogue_core.Activities;
using data_rogue_core.IOSystems;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace data_rogue_core.Controls
{
    public class FlowContainerControl : BaseContainerControl
    {
        public FlowDirection FlowDirection { get; set; } = FlowDirection.TopDown;

        public bool WrapContents { get; set; } = true;

        public override bool Layout(List<IDataRogueControlRenderer> controlRenderers, ISystemContainer systemContainer, object handle, List<MapCoordinate> playerFov, Rectangle boundingBox)
        {
            ApplyFlowLayout(controlRenderers, systemContainer, handle, playerFov, boundingBox);
            
            if (ShrinkToContents)
            {
                CalculateOwnPosition(boundingBox);
            }

            return false;
        }

        private void ApplyFlowLayout(List<IDataRogueControlRenderer> controlRenderers, ISystemContainer systemContainer, object handle, List<MapCoordinate> playerFov, Rectangle boundingBox)
        {
            var paddedBoundingBox = boundingBox.PadIn(Margin).PadIn(Padding);
            var availableSpace = new Rectangle(paddedBoundingBox.Location, paddedBoundingBox.Size);
            var largestMinorAxis = 0;
            var minorPosition = 0;

            foreach (var control in Controls)
            {
                var placed = false;
                while (!placed)
                {
                    control.Layout(controlRenderers, systemContainer, handle, playerFov, availableSpace);
                    var neededSize = GetMajorAxisSize(control.LayoutPosition);
                    var availableSize = GetMajorAxisSize(availableSpace);

                    if (availableSize >= neededSize)
                    {
                        placed = true;

                        if (FlowDirection == FlowDirection.BottomUp)
                        {
                            MoveToBottom(control, availableSpace);
                        }

                        if (FlowDirection == FlowDirection.RightToLeft)
                        {
                            MoveToRight(control, availableSpace);
                        }

                        availableSpace = ShrinkMajorAxis(availableSpace, neededSize);
                        var minorSize = GetMinorAxisSize(control.LayoutPosition);
                        if (minorSize > largestMinorAxis)
                        {
                            largestMinorAxis = minorSize;
                        }
                    }
                    else
                    {
                        if (neededSize > GetMajorAxisSize(paddedBoundingBox)) throw new ApplicationException("Can't place a larger control in a FlowContainerControl.");

                        minorPosition += largestMinorAxis;
                        availableSpace = ResetToNextRow(minorPosition, paddedBoundingBox);
                    }
                }
            }

            ApplyAlignmentToContents(paddedBoundingBox);

            if (minorPosition == 0)
            {
                if ((FlowDirection == FlowDirection.LeftToRight || FlowDirection == FlowDirection.RightToLeft) && VerticalAlignment == VerticalAlignment.Center)
                {
                    VerticallyCenterChildren();
                }
                if ((FlowDirection == FlowDirection.BottomUp || FlowDirection == FlowDirection.TopDown) && HorizontalAlignment == HorizontalAlignment.Center)
                {
                    HorizontallyCenterChildren();
                }
            }
        }

        private void MoveToRight(IDataRogueControl control, Rectangle availableSpace)
        {
            control.MovePosition(availableSpace.Right - control.Position.Width - control.Position.X, 0);
        }

        private void MoveToBottom(IDataRogueControl control, Rectangle availableSpace)
        {
            control.MovePosition(0, availableSpace.Bottom - control.Position.Height - control.Position.Y);
        }

        private void HorizontallyCenterChildren()
        {
            var bbox = GetControlBoundingBox();

            var centerX = bbox.X + bbox.Width / 2;

            foreach(var control in Controls)
            {
                var dx = centerX - (control.LayoutPosition.X + control.LayoutPosition.Width / 2);
                control.MovePosition(dx, 0);
            }
        }

        private void VerticallyCenterChildren()
        {
            var bbox = GetControlBoundingBox();

            var centerY = bbox.Y + bbox.Height / 2;

            foreach (var control in Controls)
            {
                var dy = centerY - (control.LayoutPosition.Y + control.LayoutPosition.Height / 2);
                control.MovePosition(0, dy);
            }
        }

        private Rectangle ResetToNextRow(int minorPosition, Rectangle paddedBoundingBox)
        {
            switch (FlowDirection)
            {
                case FlowDirection.LeftToRight:
                case FlowDirection.RightToLeft:
                    return new Rectangle(paddedBoundingBox.Location.X, paddedBoundingBox.Location.Y + minorPosition, paddedBoundingBox.Width, paddedBoundingBox.Height - minorPosition);
                case FlowDirection.TopDown:
                case FlowDirection.BottomUp:
                    return new Rectangle(paddedBoundingBox.Location.X + minorPosition, paddedBoundingBox.Location.Y, paddedBoundingBox.Width - minorPosition, paddedBoundingBox.Height);
                default:
                    throw new ApplicationException("Unknown FlowDirection");
            }
        }

        private Rectangle ShrinkMajorAxis(Rectangle availableSpace, int neededSize)
        {
            switch (FlowDirection)
            {
                case FlowDirection.LeftToRight:
                    return new Rectangle(availableSpace.Location.X + neededSize, availableSpace.Location.Y, availableSpace.Width - neededSize, availableSpace.Height);
                case FlowDirection.RightToLeft:
                    return new Rectangle(availableSpace.Location.X, availableSpace.Location.Y, availableSpace.Width - neededSize, availableSpace.Height);
                case FlowDirection.TopDown:
                    return new Rectangle(availableSpace.Location.X, availableSpace.Location.Y + neededSize, availableSpace.Width, availableSpace.Height - neededSize);
                case FlowDirection.BottomUp:
                    return new Rectangle(availableSpace.Location.X, availableSpace.Location.Y, availableSpace.Width, availableSpace.Height - neededSize);
                default:
                    throw new ApplicationException("Unknown FlowDirection");
            }
        }

        private int GetMajorAxisSize(Rectangle rectangle)
        {
            switch(FlowDirection)
            {
                case FlowDirection.LeftToRight:
                case FlowDirection.RightToLeft:
                    return rectangle.Width;
                case FlowDirection.TopDown:
                case FlowDirection.BottomUp:
                    return rectangle.Height;
                default:
                    throw new ApplicationException("Unknown FlowDirection");
            }
        }

        private int GetMinorAxisSize(Rectangle rectangle)
        {
            switch (FlowDirection)
            {
                case FlowDirection.LeftToRight:
                case FlowDirection.RightToLeft:
                    return rectangle.Height;
                case FlowDirection.TopDown:
                case FlowDirection.BottomUp:
                    return rectangle.Width;
                default:
                    throw new ApplicationException("Unknown FlowDirection");
            }
        }
    }
}
