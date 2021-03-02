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

            var internalBoundingBox = boundingBox.Pad(Margin).Pad(Padding);

            // First assign space for the paging text, if it turns out to be necessary.
            PagingText.Layout(controlRenderers, systemContainer, handle, playerFov, internalBoundingBox);
            var spaceForMenuItems = new Rectangle(internalBoundingBox.Location, new Size(internalBoundingBox.Width, internalBoundingBox.Height - PagingText.Position.Height));

            // Find the size of a test menu item. Use this to work out how many pages you need. We assume we're always laying this out vertically.
            var testMenuItem = MenuItems.First();
            testMenuItem.Layout(controlRenderers, systemContainer, handle, playerFov, spaceForMenuItems);
            var itemHeight = testMenuItem.Position.Height;
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

                    spaceForMenuItems = spaceForMenuItems.ShrinkFromTop(item.Position.Height);
                }
            }

            // Place the MenuSelectorControls arond the selected item.
            PlaceSelectorControls(controlRenderers, systemContainer, handle, playerFov, boundingBox);

                    // If page count > 1, add the page text. Otherwise make sure it's not in Controls.
            if (totalPages > 1)
            {
                var pageTextPosition = new Rectangle(internalBoundingBox.Left, spaceForMenuItems.Bottom, internalBoundingBox.Width, internalBoundingBox.Bottom - spaceForMenuItems.Bottom);

                PagingText.Layout(controlRenderers, systemContainer, handle, playerFov, pageTextPosition);

                Controls.Add(PagingText);
            }

            return false;
        }

        private void PlaceSelectorControls(List<IDataRogueControlRenderer> controlRenderers, ISystemContainer systemContainer, object handle, List<MapCoordinate> playerFov, Rectangle boundingBox)
        {
            LeftSelector.Layout(controlRenderers, systemContainer, handle, playerFov, boundingBox);
            RightSelector.Layout(controlRenderers, systemContainer, handle, playerFov, boundingBox);

            var leftPos = new Point(
                SelectedItem.Position.Left - LeftSelector.Position.Width - LeftSelector.Margin.Right,
                SelectedItem.Position.Top - SelectedItem.Position.Height / 2 + LeftSelector.Position.Height / 2);

            LeftSelector.Position = new Rectangle(leftPos, LeftSelector.Position.Size);

            var rightPos = new Point(
                SelectedItem.Position.Right + RightSelector.Margin.Left,
                SelectedItem.Position.Top - SelectedItem.Position.Height / 2 + RightSelector.Position.Height / 2);

            RightSelector.Position = new Rectangle(rightPos, RightSelector.Position.Size);
        }

        private void InitialiseControls()
        {
            PagingText = new TextControl { Parameters = "(page xx of yy)" };
            LeftSelector = new MenuSelectorControl { Direction = TileDirections.Left, Margin = new Padding(4) };
            RightSelector = new MenuSelectorControl { Direction = TileDirections.Right, Margin = new Padding(4) };
        }
    }
}
