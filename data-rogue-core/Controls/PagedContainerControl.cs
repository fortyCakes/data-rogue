using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BLTWrapper;
using data_rogue_core.Activities;
using data_rogue_core.IOSystems;
using data_rogue_core.Maps;
using data_rogue_core.Menus;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Utils;
using MenuItem = data_rogue_core.Menus.MenuItem;

namespace data_rogue_core.Controls
{
    public class PagedMenuControl : BaseContainerControl
    {
        public FlowDirection FlowDirection { get; set; } = FlowDirection.TopDown;
        public List<MenuItem> MenuItems { get; set; }

        public MenuItem SelectedItem { get; set; }

        private bool Initialised = false;
        private TextControl PagingText;
        private MenuSelectorControl LeftSelector;
        private MenuSelectorControl RightSelector;

        public override bool Layout(List<IDataRogueControlRenderer> controlRenderers, ISystemContainer systemContainer, object handle, List<MapCoordinate> playerFov, Rectangle boundingBox)
        {
            if (!Initialised) InitialiseControls();

            Controls = new List<IDataRogueControl> { LeftSelector, RightSelector };

            var internalBoundingBox = boundingBox.PadIn(Margin).PadIn(Padding);

            // First assign space for the paging text, if it turns out to be necessary.
            PagingText.Layout(controlRenderers, systemContainer, handle, playerFov, internalBoundingBox);
            var spaceForMenuItems = new Rectangle(internalBoundingBox.Location, new Size(internalBoundingBox.Width, internalBoundingBox.Height - PagingText.LayoutPosition.Height));

            // Find the size of a test menu item. Use this to work out how many pages you need. We assume we're always laying this out vertically.
            var testMenuItem = MenuItems.First();
            testMenuItem.Layout(controlRenderers, systemContainer, handle, playerFov, spaceForMenuItems);
            var itemHeight = testMenuItem.LayoutPosition.Height;
            var itemCount = MenuItems.Count();

            var itemsPerPage = spaceForMenuItems.Height / itemHeight;
            var totalPages = ( + itemsPerPage - 1) / itemsPerPage;

            var selectedItemIndex = MenuItems.IndexOf(SelectedItem);
            var page = selectedItemIndex / itemsPerPage;

            // Find what page the selected item is on, and layout those items.
            for (int i = 0; i < itemsPerPage; i++)
            {
                var displayIndex = page * itemsPerPage + i;
                if (displayIndex < itemCount)
                {
                    MenuItem item = MenuItems[displayIndex];
                    item.Layout(controlRenderers, systemContainer, handle, playerFov, spaceForMenuItems);
                    Controls.Add(item);

                    spaceForMenuItems = spaceForMenuItems.ShrinkFromTop(item.LayoutPosition.Height);
                }
            }

            // If page count > 1, add the page text. Otherwise make sure it's not in Controls.
            if (totalPages > 1)
            {
                var pageTextPosition = new Rectangle(internalBoundingBox.Left, spaceForMenuItems.Bottom, internalBoundingBox.Width, internalBoundingBox.Bottom - spaceForMenuItems.Bottom);

                PagingText.Layout(controlRenderers, systemContainer, handle, playerFov, pageTextPosition);

                Controls.Add(PagingText);
            }

            if (ShrinkToContents)
            {
                CalculateOwnPosition(boundingBox);
            }

            ApplyAlignmentToContents(internalBoundingBox);

            if (ShrinkToContents)
            {
                CalculateOwnPosition(boundingBox);
            }
            // Place the MenuSelectorControls arond the selected item.
            PlaceSelectorControls(controlRenderers, systemContainer, handle, playerFov, boundingBox);

            return false;
        }

        protected override void CalculateOwnPosition(Rectangle boundingBox)
        {
            Rectangle controlBounding = GetCustomControlBoundingBox();

            Position = controlBounding.PadOut(Padding).PadOut(Margin);
        }

        protected override void ApplyAlignmentToContents(Rectangle boundingBox)
        {
            Rectangle controlBounding = GetCustomControlBoundingBox();

            var dx = GetHorizontalAdjustment(boundingBox, controlBounding);
            var dy = GetVerticalAdjustment(boundingBox, controlBounding);

            foreach (var control in Controls)
            {
                var cdx = dx + controlBounding.Width / 2;
                if (HorizontalAlignment == HorizontalAlignment.Center)
                {
                    cdx -= control.Position.Width / 2;
                }
                control.MovePosition(cdx, dy);
            }
        }

        protected Rectangle GetCustomControlBoundingBox()
        {
            var xmin = int.MaxValue;
            var ymin = int.MaxValue;
            var xmax = int.MinValue;
            var ymax = int.MinValue;

            foreach (var control in Controls.OfType<MenuItem>())
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

        private void PlaceSelectorControls(List<IDataRogueControlRenderer> controlRenderers, ISystemContainer systemContainer, object handle, List<MapCoordinate> playerFov, Rectangle boundingBox)
        {
            LeftSelector.Layout(controlRenderers, systemContainer, handle, playerFov, boundingBox);
            RightSelector.Layout(controlRenderers, systemContainer, handle, playerFov, boundingBox);

            var leftPos = new Point(
                SelectedItem.LayoutPosition.Left - LeftSelector.LayoutPosition.Width,
                SelectedItem.LayoutPosition.Top + SelectedItem.LayoutPosition.Height / 2 + 1 - LeftSelector.LayoutPosition.Height / 2);

            LeftSelector.Position = new Rectangle(leftPos, LeftSelector.Position.Size);

            var rightPos = new Point(
                SelectedItem.LayoutPosition.Right + 1,
                SelectedItem.LayoutPosition.Top + SelectedItem.LayoutPosition.Height / 2 + 1 - RightSelector.LayoutPosition.Height / 2);

            RightSelector.Position = new Rectangle(rightPos, RightSelector.Position.Size);
        }

        private void InitialiseControls()
        {
            PagingText = new TextControl { Parameters = "(page xx of yy)" };
            LeftSelector = new MenuSelectorControl { Direction = TileDirections.Left, Margin = new Padding(1, 0, 1, 0) };
            RightSelector = new MenuSelectorControl { Direction = TileDirections.Right, Margin = new Padding(1, 0, 1, 0) };
        }
    }
}
