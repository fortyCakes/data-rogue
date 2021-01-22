
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

namespace data_rogue_core.Activities
{
    public class FormActivity : BaseActivity
    {
        public override ActivityType Type => ActivityType.Form;
        public override bool RendersEntireSpace => true;
        public override bool AcceptsInput => true;

        public bool Running => true;

        public Form Form { get; set; }

        public FormActivity(Form form)
        {
            Form = form;
            form.Activity = this;
        }

        public void Initialise()
        {
        }

        public override void HandleMouse(ISystemContainer systemContainer, MouseData mouse)
        {
            if (mouse.IsLeftClick)
            {
                var controlsUnderMouse = MouseControlHelper.GetControlsUnderMouse(mouse, Controls);
                controlsUnderMouse = controlsUnderMouse.Where(c => c.GetType() != typeof(BackgroundControl));

                if (controlsUnderMouse.Count() == 1)
                {
                    var control = controlsUnderMouse.Single();

                    if (control is ButtonControl)
                    {
                        var button = control as ButtonControl;
                        Form.FormSelection = new FormSelection { SelectedItem = button.Text };
                        Form.Select();
                    }

                    if (control is FormData)
                    {
                        var data = control as FormData;
                        Form.FormSelection = new FormSelection { SelectedItem = data.Name };
                        control.Click(this, new PositionEventHandlerArgs(mouse.X, mouse.Y));

                        if (control is SubSelectableFormData)
                        {
                            var subData = control as SubSelectableFormData;

                            Form.FormSelection.SubItem = subData.GetSubItems().First();
                        }
                    }
                }
            }
        }

        public override void HandleKeyboard(ISystemContainer systemContainer, KeyCombination keyboard)
        {
            // None
        }

        public override void HandleAction(ISystemContainer systemContainer, ActionEventData action)
        {
            Form.HandleAction(action);
        }

        public override IEnumerable<IDataRogueControl> GetLayout(IUnifiedRenderer renderer, ISystemContainer systemContainer, object rendererHandle, List<IDataRogueControlRenderer> controlRenderers, List<MapCoordinate> playerFov, int width, int height)
        {
            yield return new BackgroundControl { Position = new Rectangle(0, 0, width, height) };

            var y = renderer.ActivityPadding.Top;

            var titleText = new LargeTextControl { Position = new Rectangle(renderer.ActivityPadding.Left, y, 0, 0), Parameters = Form.Title };
            var titleSize = GetSizeOf(systemContainer, rendererHandle, controlRenderers, playerFov, titleText);
            y += titleSize.Height;
            
            var lineControl = new LineControl { Position = new Rectangle(0, y, width, 0) };
            Size lineSize = GetSizeOf(systemContainer, rendererHandle, controlRenderers, playerFov, lineControl);
            y += lineSize.Height;

            yield return titleText;
            yield return lineControl;

            var buttonX = renderer.ActivityPadding.Left;
            foreach (var button in Form.Buttons.GetFlags())
            {
                var control = new ButtonControl { Text = button.ToString(), IsFocused = Form.FormSelection.SelectedItem == button.ToString()};
                var size = GetSizeOf(systemContainer, rendererHandle, controlRenderers, playerFov, control);

                control.Position = new Rectangle(buttonX, height - renderer.ActivityPadding.Top - size.Height, size.Width, size.Height);
                yield return control;

                buttonX += size.Width + renderer.ActivityPadding.Left;
            }

            foreach(var kvp in Form.Fields)
            {
                var nameText = new TextControl { Parameters = kvp.Key + ": " };
                var nameSize = GetSizeOf(systemContainer, rendererHandle, controlRenderers, playerFov, nameText);

                var x = renderer.ActivityPadding.Left;

                nameText.Position = new Rectangle(x, y, nameSize.Width, nameSize.Height);
                yield return nameText;

                x += nameSize.Width;
                
                var formData = kvp.Value;
                var size = GetSizeOf(systemContainer, rendererHandle, controlRenderers, playerFov, formData);

                formData.Position = new Rectangle(x, y, size.Width, size.Height);

                formData.IsFocused = Form.FormSelection.SelectedItem == kvp.Key;

                y += size.Height;

                SubSelectableFormData subData;
                if ((subData = formData as SubSelectableFormData) != null)
                {
                    subData.SubSelection = Form.FormSelection.SubItem;
                }

                yield return formData;

                y += renderer.ActivityPadding.Top;
            }
        }

        private static Size GetSizeOf(ISystemContainer systemContainer, object rendererHandle, List<IDataRogueControlRenderer> controlRenderers, List<MapCoordinate> playerFov, IDataRogueControl control)
        {
            return controlRenderers.Single(c => c.DisplayType == control.GetType()).GetSize(rendererHandle, control, systemContainer, playerFov);
        }
    }
}
