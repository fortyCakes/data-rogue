
using data_rogue_core.Renderers;
using data_rogue_core.Forms;
using data_rogue_core.Renderers.ConsoleRenderers;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.IOSystems;
using data_rogue_core.Systems;
using System.Collections.Generic;
using data_rogue_core.Maps;
using data_rogue_core.Components;
using data_rogue_core.Controls;
using System.Drawing;
using System.Linq;
using data_rogue_core.Utils;
using System;
using data_rogue_core.Forms.StaticForms;
using OpenTK.Input;
using System.Windows.Forms;
using Form = data_rogue_core.Forms.Form;

namespace data_rogue_core.Activities
{
    public class FormActivity : BaseActivity
    {
        public override ActivityType Type => ActivityType.Form;
        public override bool RendersEntireSpace => true;
        public override bool AcceptsInput => true;

        public bool Running => true;

        public Form Form { get; set; }

        public FormActivity(Rectangle position, Padding padding, Form form) : base(position, padding)
        {
            Form = form;
            form.Activity = this;
        }
        
        public override void InitialiseControls()
        {
            var paddedPosition = Position.Pad(Padding);

            var backgroundControl = new BackgroundControl { Position = Position };

            var topFlow = new FlowContainerControl { Position = Position };

            var titleText = new LargeTextControl { Parameters = Form.Title };
            topFlow.Controls.Add(titleText);
            var lineControl = new LineControl();
            topFlow.Controls.Add(lineControl);

            var buttonFlowContainer = new FlowContainerControl { Position = Position, FlowDirection = FlowDirection.BottomUp }; 
            var buttonFlow = new FlowContainerControl { Position = Position, FlowDirection = FlowDirection.LeftToRight };
            buttonFlowContainer.Controls.Add(buttonFlow);

            foreach (var button in Form.Buttons.GetFlags())
            {
                var buttonControl = new ButtonControl { Text = button.ToString() };
                buttonControl.OnClick += FormButtonControl_OnClick;
                buttonFlow.Controls.Add(buttonControl);
            }

            foreach(var formField in Form.Fields)
            {
                var nameText = new TextControl { Parameters = formField.Key + ": " };
                var formFieldControl = formField.Value;

                formFieldControl.OnClick += FormFieldControl_OnClick;

                var subFlow = new FlowContainerControl { FlowDirection = FlowDirection.LeftToRight };
                subFlow.Controls.Add(nameText);
                subFlow.Controls.Add(formFieldControl);
                topFlow.Controls.Add(subFlow);
            }
            
            Controls.Add(backgroundControl);
            Controls.Add(topFlow);
            Controls.Add(buttonFlow);
        }

        private void FormFieldControl_OnClick(object sender, PositionEventHandlerArgs args)
        {
            var data = sender as FormData;
            Form.FormSelection = new FormSelection { SelectedItem = data.Name };

            if (data is SubSelectableFormData)
            {
                var subData = data as SubSelectableFormData;

                Form.FormSelection.SubItem = subData.GetSubItems().First();
            }
        }

        private void FormButtonControl_OnClick(object sender, PositionEventHandlerArgs args)
        {
            Form.FormSelection = new FormSelection { SelectedItem = (sender as ButtonControl).Text };
            Form.Select();
        }

        public override void HandleMouse(ISystemContainer systemContainer, MouseData mouse)
        {

        }

        public override void HandleKeyboard(ISystemContainer systemContainer, KeyCombination keyboard)
        {
            Form.HandleKeyboard(keyboard);
        }

        public override void HandleAction(ISystemContainer systemContainer, ActionEventData action)
        {
            Form.HandleAction(action);
        }
    }
}
