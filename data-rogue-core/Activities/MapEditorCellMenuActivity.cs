using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using data_rogue_core.Components;
using data_rogue_core.Controls;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.IOSystems;
using data_rogue_core.Maps;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.Activities
{
    internal class EntityPickerMenuActivity : BaseActivity
    {
        private ISystemContainer _systemContainer;
        private string _caption;
        private Action<IEntity> _callback;
        private IEntity SelectedCell;

        private IEnumerable<IEntity> Entities;
        private string HoveredCellText => "Cell:" + (SelectedCell?.DescriptionName ?? "(no cell selected)");

        public EntityPickerMenuActivity(IEnumerable<IEntity> entities, ISystemContainer systemContainer, string caption, Action<IEntity> callback)
        {
            _systemContainer = systemContainer;
            _caption = caption;
            _callback = callback;

            Entities = entities;
        }

        public override ActivityType Type => ActivityType.Menu;

        public override bool RendersEntireSpace => false;

        public override bool AcceptsInput => true;

        public override IEnumerable<IDataRogueControl> GetLayout(IUnifiedRenderer renderer, ISystemContainer systemContainer, object rendererHandle, List<IDataRogueControlRenderer> controlRenderers, List<MapCoordinate> playerFov, int width, int height)
        {
            var controls = new List<IDataRogueControl>();
            

            var offsetX = renderer.ActivityPadding.Left * 2;
            var offsetY = renderer.ActivityPadding.Top * 2;

            var maxWidth = width - renderer.ActivityPadding.Right - 150;
            var maxHeight = height - renderer.ActivityPadding.Bottom;

            var x = 0;
            var maxX = 0;
            var y = 0;


            var textControl = new TextControl { Position = new Rectangle(offsetX + x, offsetY + y, 0, 0), Parameters = _caption };
            var textSize = renderer.GetRendererFor(textControl).GetSize(rendererHandle, textControl, systemContainer, playerFov);
            textControl.Position = new Rectangle(offsetX + x, offsetY + y, textSize.Width, textSize.Height);
            controls.Add(textControl);

            y += textSize.Height + 1;
            y += textSize.Height + 1;
            
            var hoveredCellName = new TextControl { Position = new Rectangle(offsetX + x, offsetY + y, 0, 0), Parameters = HoveredCellText };
            textSize = renderer.GetRendererFor(hoveredCellName).GetSize(rendererHandle, hoveredCellName, systemContainer, playerFov);
            hoveredCellName.Position = new Rectangle(offsetX + x, offsetY + y, textSize.Width, textSize.Height);
            if (SelectedCell == null) hoveredCellName.Color = Color.Gray;
            controls.Add(hoveredCellName);
            y += textSize.Height + 1;

            var exampleCell = new MenuEntityControl();

            var cellSize = renderer.GetRendererFor(exampleCell).GetSize(rendererHandle, exampleCell, systemContainer, playerFov);

            foreach(var cell in Entities)
            {
                controls.Add(new MenuEntityControl { Position = new Rectangle(offsetX + x, offsetY + y, cellSize.Width, cellSize.Height), Entity = cell });

                x += cellSize.Width + 1;

                if (maxX < x) maxX = x;

                if (offsetX + x + cellSize.Width >= maxWidth)
                {
                    x = 0;
                    y += cellSize.Height + 1;
                }
            }

            y += cellSize.Height * 2 + 4;


            var finalWidth = maxX + cellSize.Width + 8;
            var buttonControl = new ButtonControl { Position = new Rectangle(offsetX, y, 0, 0), Text = "Cancel" };
            buttonControl.OnClick += buttonControl_OnClick;

            var buttonSize = renderer.GetRendererFor(buttonControl).GetSize(rendererHandle, buttonControl, systemContainer, playerFov);

            var buttonLeft = finalWidth - buttonSize.Width - renderer.ActivityPadding.Right;
            buttonControl.Position = new Rectangle(buttonLeft, buttonControl.Position.Top, buttonSize.Width, buttonSize.Height);
            y += buttonControl.Position.Height + 1;
            controls.Add(buttonControl);

            var finalHeight = y + renderer.ActivityPadding.Bottom;

            var backgroundControl = new BackgroundControl { Position = new Rectangle(renderer.ActivityPadding.Left, renderer.ActivityPadding.Left, finalWidth, finalHeight ) };
            controls.Add(backgroundControl);


            CenterControls(controls, width, height, finalWidth, finalHeight);

            return controls;
        }

        private void CenterControls(List<IDataRogueControl> controls, int width, int height, int finalWidth, int finalHeight)
        {
            var x = width / 2 - finalWidth / 2;
            var y = height / 2 - finalHeight / 2;


            foreach(var control in controls)
            {
                control.Position = new Rectangle(control.Position.Left + x, control.Position.Top + y, control.Position.Width, control.Position.Height);
            }
        }

        private void buttonControl_OnClick(object sender, PositionEventHandlerArgs args)
        {
            CloseActivity();
        }

        public override void HandleAction(ISystemContainer systemContainer, ActionEventData action)
        {
            if (action != null)
            {
                if (action.Action == EventSystem.EventData.ActionType.Select)
                {
                    Select();
                }
            }
        }

        private void Select()
        {
            _callback(SelectedCell);
            CloseActivity();
        }

        private void CloseActivity()
        {
            _systemContainer.ActivitySystem.RemoveActivity(this);
        }

        public override void HandleMouse(ISystemContainer systemContainer, MouseData mouse)
        {
            base.HandleMouse(systemContainer, mouse);

            if (mouse.MouseActive)
            {
                SelectedCell = GetHoveredCell(systemContainer, mouse);
            }

            if (SelectedCell != null && mouse.IsLeftClick)
            {
                Select();
            }
        }

        private IEntity GetHoveredCell(ISystemContainer systemContainer, MouseData mouse)
        {
            var mouseOverControl = GetMouseOverControl(systemContainer, mouse);

            var cellDisplay = mouseOverControl as MenuEntityControl;

            return cellDisplay?.Entity;
        }

        public override void HandleKeyboard(ISystemContainer systemContainer, KeyCombination keyboard)
        {

        }
    }
}