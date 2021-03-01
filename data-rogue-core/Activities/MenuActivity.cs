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
            var topFlow = new FlowContainerControl { HorizontalAlignment = horizontalAlignment };

            var titleText = new LargeTextControl { Parameters = Menu.MenuName };
            topFlow.Controls.Add(titleText);
            var lineControl = new LineControl();
            topFlow.Controls.Add(lineControl);
            var pagingContainer = new PagedContainerControl();


            foreach(var item in Menu.MenuItems)
            {
                pagingContainer.Controls.Add(item);
            }

            LeftSelector = new MenuSelectorControl { Direction = TileDirections.Left };
            RightSelector = new MenuSelectorControl { Direction = TileDirections.Right };
            
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
    }
}
