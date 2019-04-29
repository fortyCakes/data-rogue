using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using BLTWrapper;
using data_rogue_core.Controls;
using data_rogue_core.IOSystems;
using data_rogue_core.Maps;
using data_rogue_core.Menus;
using data_rogue_core.Renderers;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.Activities
{
    public class MenuActivity : IActivity
    {
        public ActivityType Type => ActivityType.Menu;
        public object Data => Menu;
        public bool RendersEntireSpace => true;
        public Menu Menu { get; }

        public bool Running => true;

        public MenuActivity(Menu menu)
        {
            Menu = menu;
            Menu.Activity = this;
        }

        public void HandleKeyboard(ISystemContainer systemContainer, KeyCombination keyboard)
        {
            //throw new System.NotImplementedException();
        }

        public void HandleMouse(ISystemContainer systemContainer, MouseData mouse)
        {
            //throw new System.NotImplementedException();
        }

        public void HandleAction(ISystemContainer systemContainer, ActionEventData action)
        {
            Menu.HandleAction(action);
        }

        public IEnumerable<IDataRogueControl> GetLayout(IUnifiedRenderer renderer, ISystemContainer systemContainer, object rendererHandle, List<IDataRogueControlRenderer> controlRenderers, List<MapCoordinate> playerFov, int width, int height)
        {
            yield return new BackgroundControl { Position = new Rectangle(0, 0, width, height) };

            var y = renderer.ActivityPadding.Top;

            var titleText = new LargeTextControl { Position = new Rectangle(renderer.ActivityPadding.Left, y, 0, 0), Parameters = Menu.MenuName };

            var titleSize = GetSizeOf(systemContainer, rendererHandle, controlRenderers, playerFov, titleText);

            if (Menu.Centred)
            {
                y = height / 2 - titleSize.Height;
                titleText.Position = new Rectangle(width / 2 - titleSize.Width / 2, y, 0, 0);

                
            }
            y += titleSize.Height;

            Rectangle linePosition;
            if (Menu.Centred)
            {
                linePosition = new Rectangle(titleText.Position.X, y, titleSize.Width, 0);
            }
            else
            {
                linePosition = new Rectangle(0, y, width, 0);
            }

            var lineControl = new LineControl { Position = linePosition };
            Size lineSize = GetSizeOf(systemContainer, rendererHandle, controlRenderers, playerFov, lineControl);
            y += lineSize.Height;

            yield return titleText;
            yield return lineControl;

            var menuActionsControl = new MenuActionsControl {AvailableActions = Menu.AvailableActions, SelectedAction = Menu.SelectedAction, SelectedColor = Color.Blue};
            var actionsSize = GetSizeOf(systemContainer, rendererHandle, controlRenderers, playerFov, menuActionsControl);
            menuActionsControl.Position = new Rectangle(width - renderer.ActivityPadding.Right - actionsSize.Width, renderer.ActivityPadding.Top, actionsSize.Width, actionsSize.Height);
            yield return menuActionsControl;

            var availableHeight = height - y - renderer.ActivityPadding.Bottom;

            var testSize = GetSizeOf(systemContainer, rendererHandle, controlRenderers, playerFov, Menu.MenuItems.First());
            var itemHeight = testSize.Height;

            int selectedIndex = Menu.SelectedIndex;
            int itemCount = Menu.MenuItems.Count;
            int itemsPerPage = (availableHeight / itemHeight) - 2;
            int pageCount = (itemCount - 1) / itemsPerPage + 1;
            int page = selectedIndex / itemsPerPage;

            var leftSelector = new MenuSelectorControl { Direction = TileDirections.Left };
            var selectorSize = GetSizeOf(systemContainer, rendererHandle, controlRenderers, playerFov, leftSelector);

            for (int i = 0; i < itemsPerPage; i++)
            {
                var displayIndex = page * itemsPerPage + i;
                if (displayIndex < itemCount)
                {
                    MenuItem item = Menu.MenuItems[displayIndex];
                    int itemY = y + i * itemHeight;

                    int itemX = renderer.ActivityPadding.Left + selectorSize.Width * 2;
                    var size = GetSizeOf(systemContainer, rendererHandle, controlRenderers, playerFov, item);

                    if (Menu.Centred)
                    {
                        itemX = width / 2 - size.Width / 2;
                    }

                    item.Position = new Rectangle(itemX, itemY, size.Width, size.Height);

                    if (displayIndex == selectedIndex)
                    {
                        foreach(var menuSelector in RenderMenuSelectors(itemX, itemY, size, systemContainer, rendererHandle, controlRenderers, playerFov))
                        {
                            yield return menuSelector;
                        }
                    }

                    yield return item;
                }

                y += itemHeight;
            }

            if (pageCount > 1)
            {
                y += itemHeight;
                yield return new TextControl { Position = new Rectangle(renderer.ActivityPadding.Left, y, 0, 0), Parameters = $"(page {page} of {pageCount})" };
            }
        }

        private IEnumerable<IDataRogueControl> RenderMenuSelectors(int x, int y, Size size, ISystemContainer systemContainer, object rendererHandle, List<IDataRogueControlRenderer> controlRenderers, List<MapCoordinate> playerFov)
        {
            var leftSelector = new MenuSelectorControl { Direction = TileDirections.Left };
            var selectorSize = GetSizeOf(systemContainer, rendererHandle, controlRenderers, playerFov, leftSelector);
            leftSelector.Position = new Rectangle(x - selectorSize.Width * 2, y, selectorSize.Width, selectorSize.Height);

            var rightSelector = new MenuSelectorControl { Direction = TileDirections.Right, Position = new Rectangle(x + size.Width + selectorSize.Width, y, selectorSize.Width, selectorSize.Height) };

            yield return leftSelector;
            yield return rightSelector;
        }

        private static Size GetSizeOf(ISystemContainer systemContainer, object rendererHandle, List<IDataRogueControlRenderer> controlRenderers, List<MapCoordinate> playerFov, IDataRogueControl control)
        {
            return controlRenderers.Single(c => c.DisplayType == control.GetType()).GetSize(rendererHandle, control, systemContainer, playerFov);
        }

        public void Initialise()
        {
        }
    }
}
