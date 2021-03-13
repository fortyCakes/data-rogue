using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
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
        public string HoverPrefix = "Cell:";
        public string NoCellHoverText = "(no cell selected)";
        private ISystemContainer _systemContainer;
        private string _caption;
        private Action<IEntity> _callback;
        private IEntity SelectedCell { get; set; }

        private IEnumerable<IEntity> Entities;
        private TextControl HoveredCellName;

        private string HoveredCellText => HoverPrefix + (SelectedCell?.DescriptionName ?? NoCellHoverText);

        public EntityPickerMenuActivity(Rectangle position, Padding padding, IEnumerable<IEntity> entities, ISystemContainer systemContainer, string caption, Action<IEntity> callback) : base(position, padding)
        {
            _systemContainer = systemContainer;
            _caption = caption;
            _callback = callback;

            Entities = entities;

            OnLayout += EntityPickerMenuActivity_OnLayout;
        }

        private void EntityPickerMenuActivity_OnLayout(object sender, EventArgs e)
        {
            HoveredCellName.Parameters = HoveredCellText;
            HoveredCellName.Color = SelectedCell == null ? Color.Gray : Color.White;
        }

        public override ActivityType Type => ActivityType.Menu;

        public override bool RendersEntireSpace => false;

        public override bool AcceptsInput => true;

        public override void InitialiseControls()
        {
            var flow = new FlowContainerControl
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                ApplyAlignment = true,
                Padding = new Padding(10)
            };

            var backgroundControl = new BackgroundControl
            {
                Position = Position,
                Padding = new Padding(3),
                ShrinkToContents = true,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            var downFlow = new FlowContainerControl { FlowDirection = FlowDirection.TopDown, VerticalAlignment = VerticalAlignment.Top, ShrinkToContents = true };
            var sideFlow = new FlowContainerControl { FlowDirection = FlowDirection.LeftToRight, ShrinkToContents = true };
            var textControl = new TextControl { Parameters = _caption, Margin = new Padding(1) };
            var buttonControl = new ButtonControl { Text = "Cancel" };
            HoveredCellName = new TextControl { Parameters = HoveredCellText };

            buttonControl.OnClick += buttonControl_OnClick;

            Controls.Add(flow);
            flow.Controls.Add(backgroundControl);

            backgroundControl.Controls.Add(downFlow);

            downFlow.Controls.Add(textControl);
            downFlow.Controls.Add(sideFlow);
            downFlow.Controls.Add(HoveredCellName);
            downFlow.Controls.Add(buttonControl);

            foreach (var cell in Entities)
            {
                sideFlow.Controls.Add(new MenuEntityControl { Entity = cell, Margin = new Padding(1) });
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