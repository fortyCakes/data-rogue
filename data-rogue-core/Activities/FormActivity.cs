
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
using System.Windows.Forms.VisualStyles;

namespace data_rogue_core.Activities
{
    public class FormActivity : BaseActivity
    {
        private List<FormData> FormControls;
        private List<ButtonControl> Buttons;

        public override ActivityType Type => ActivityType.Form;
        public override bool RendersEntireSpace => true;
        public override bool AcceptsInput => true;

        public bool Running => true;

        public Form Form { get; set; }

        public FormActivity(Rectangle position, Padding padding, Form form) : base(position, padding)
        {
            Form = form;
            form.Activity = this;
            OnLayout += FormActivity_OnLayout;
        }

        public override void InitialiseControls()
        {
            var backgroundControl = new BackgroundControl { Position = Position, Padding = Padding };

            var topFlow = new FlowContainerControl { Position = Position, ShrinkToContents = true };

            var titleText = new LargeTextControl { Parameters = Form.Title };
            topFlow.Controls.Add(titleText);
            var lineControl = new LineControl();
            topFlow.Controls.Add(lineControl);

            var buttonFlowContainer = new FlowContainerControl { Position = Position, FlowDirection = FlowDirection.BottomUp, VerticalAlignment = VerticalAlignment.Bottom, ShrinkToContents = true }; 
            var buttonFlow = new FlowContainerControl { Position = Position, FlowDirection = FlowDirection.LeftToRight, ShrinkToContents = true, VerticalAlignment = VerticalAlignment.Bottom };
            buttonFlowContainer.Controls.Add(buttonFlow);

            Buttons = new List<ButtonControl>();
            foreach (var button in Form.Buttons.GetFlags())
            {
                var buttonControl = new ButtonControl { Text = button.ToString(), Margin = new Padding(2) };
                buttonControl.OnClick += FormButtonControl_OnClick;
                buttonFlow.Controls.Add(buttonControl);
                Buttons.Add(buttonControl);
            }

            FormControls = new List<FormData>();
            foreach (var formField in Form.Fields)
            {
                var nameText = new TextControl { Parameters = formField.Key + ": " };
                var formFieldControl = formField.Value;

                formFieldControl.OnClick += FormFieldControl_OnClick;
                FormControls.Add(formFieldControl);

                var subFlow = new FlowContainerControl { FlowDirection = FlowDirection.LeftToRight, ShrinkToContents = true, Margin = new Padding { Top = 1 } };
                subFlow.Controls.Add(nameText);
                subFlow.Controls.Add(formFieldControl);
                topFlow.Controls.Add(subFlow);
            }
            
            Controls.Add(backgroundControl);
            backgroundControl.Controls.Add(topFlow);
            backgroundControl.Controls.Add(buttonFlow);
        }

        private void FormActivity_OnLayout(object sender, EventArgs e)
        {
            UpdateFormControls();
        }

        private void UpdateFormControls()
        {
            foreach(FormData control in FormControls)
            {
                if (Form.FormSelection.SelectedItem == control.Name)
                {
                    control.IsFocused = true;
                    if (control is SubSelectableFormData)
                        (control as SubSelectableFormData).SubSelection = Form.FormSelection.SubItem;
                }
                else
                {
                    control.IsFocused = false;
                    if (control is SubSelectableFormData)
                        (control as SubSelectableFormData).SubSelection = null;
                }
            }

            foreach (var button in Buttons)
            {
                button.IsFocused = Form.FormSelection.SelectedItem == button.Text;
            }
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
