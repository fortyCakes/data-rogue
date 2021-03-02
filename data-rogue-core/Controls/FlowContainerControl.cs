using data_rogue_core.IOSystems;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace data_rogue_core.Controls
{
    public class FlowContainerControl : BaseContainerControl
    {
        public FlowDirection FlowDirection { get; set; } = FlowDirection.TopDown;

        public bool WrapContents { get; set; } = true;

        public override bool Layout(List<IDataRogueControlRenderer> controlRenderers, ISystemContainer systemContainer, object handle, List<MapCoordinate> playerFov, Rectangle boundingBox)
        {
            ApplyFlowLayout(controlRenderers, systemContainer, handle, playerFov, boundingBox);

            ApplyAlignmentToContents(boundingBox);

            return false;
        }

        private void ApplyFlowLayout(List<IDataRogueControlRenderer> controlRenderers, ISystemContainer systemContainer, object handle, List<MapCoordinate> playerFov, Rectangle boundingBox)
        {
            var paddedBoundingBox = boundingBox.Pad(Margin).Pad(Padding);
            var availableSpace = new Rectangle(paddedBoundingBox.Location, paddedBoundingBox.Size);
            var largestMinorAxis = 0;
            var minorPosition = 0;

            foreach (var control in Controls)
            {
                var placed = false;
                while (!placed)
                {
                    control.Layout(controlRenderers, systemContainer, handle, playerFov, availableSpace);
                    var neededSize = GetMajorAxisSize(control.Position);
                    var availableSize = GetMajorAxisSize(availableSpace);

                    if (availableSize >= neededSize)
                    {
                        placed = true;
                        availableSpace = ShrinkMajorAxis(availableSpace, neededSize);
                        var minorSize = GetMinorAxisSize(control.Position);
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

            ApplyAlignmentToContents(boundingBox);
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
