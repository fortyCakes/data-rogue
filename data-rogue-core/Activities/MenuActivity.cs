using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using BLTWrapper;
using data_rogue_core.Controls;
using data_rogue_core.IOSystems;
using data_rogue_core.Maps;
using data_rogue_core.Menus;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;
using Menu = data_rogue_core.Menus.Menu;
using MenuItem = data_rogue_core.Menus.MenuItem;

namespace data_rogue_core.Activities
{
    public class MenuActivity : BaseActivity
    {
        private TextControl PageCountText;
        private MenuSelectorControl LeftSelector;
        private MenuSelectorControl RightSelector;

        public override ActivityType Type => ActivityType.Menu;
        public override bool RendersEntireSpace => true;
        public override bool AcceptsInput => true;
        public Menu Menu { get; }

        public bool Running => true;

        public MenuActivity(Rectangle position, Padding padding, Menu menu) : base(position, padding)
        {
            Menu = menu;
            Menu.Activity = this;
        }

        public override void InitialiseControls()
        {
            var background = new BackgroundControl { Position = Position };
            Controls.Add(background);

            var menuActions = new MenuActionsControl { AvailableActions = Menu.AvailableActions, SelectedAction = Menu.SelectedAction, SelectedColor = Color.Blue, VerticalAlignment = VerticalAlignment.Top };
            Controls.Add(menuActions);

            var horizontalAlignment = Menu.Centred ? HorizontalAlignment.Center : HorizontalAlignment.Left;
            var verticalAlignment = Menu.Centred ? VerticalAlignment.Center : VerticalAlignment.Top;
            var topFlow = new FlowContainerControl { Position = Position, HorizontalAlignment = horizontalAlignment };

            var titleText = new LargeTextControl { Parameters = Menu.MenuName };
            topFlow.Controls.Add(titleText);
            var lineControl = new LineControl();
            topFlow.Controls.Add(lineControl);

            foreach(var item in Menu.MenuItems)
            {
                topFlow.Controls.Add(item);
            }

            PageCountText = new TextControl { Position = Position, Parameters = $"(page 1 of {Menu.PageCount})", HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Bottom };
            LeftSelector = new MenuSelectorControl { Direction = TileDirections.Left };
            RightSelector = new MenuSelectorControl { Direction = TileDirections.Right };

            Controls.Add(PageCountText);
            Controls.Add(LeftSelector);
            Controls.Add(RightSelector);
        }

        public override void HandleKeyboard(ISystemContainer systemContainer, KeyCombination keyboard)
        {
        }

        public override void HandleMouse(ISystemContainer systemContainer, MouseData mouse)
        {
            if (mouse.MouseActive)
            {
                var controlsUnderMouse = MouseControlHelper.GetControlsUnderMouse(mouse, Controls);
                controlsUnderMouse = controlsUnderMouse.Where(c => c.GetType() != typeof(BackgroundControl));

                if (controlsUnderMouse.Count() == 1)
                {
                    var control = controlsUnderMouse.Single();

                    if (control is MenuItem)
                    {
                        Menu.SelectedItem = control as MenuItem;
                    }


                    if (mouse.IsLeftClick)
                    {
                        if (control is MenuItem)
                        {
                            Menu.Select();
                        }

                        if (control is MenuActionsControl)
                        {
                            Menu.NextAction();
                        }
                    }
                }
            }
        }

        public override void HandleAction(ISystemContainer systemContainer, ActionEventData action)
        {
            Menu.HandleAction(action);
        }

        private IEnumerable<IDataRogueControl> RenderMenuSelectors(int x, int y, Size size, ISystemContainer systemContainer, object rendererHandle, List<IDataRogueControlRenderer> controlRenderers, List<MapCoordinate> playerFov)
        {
            var leftSelector = new 
            var selectorSize = GetSizeOf(systemContainer, rendererHandle, controlRenderers, playerFov, leftSelector);
            leftSelector.Position = new Rectangle(x - selectorSize.Width * 2, y, selectorSize.Width, selectorSize.Height);

            var 

            yield return leftSelector;
            yield return rightSelector;
        }
    }
}
