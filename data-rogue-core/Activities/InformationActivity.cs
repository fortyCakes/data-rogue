using data_rogue_core.IOSystems;
using data_rogue_core.Renderers;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;
using OpenTK.Input;
using System.Collections.Generic;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Controls;
using System.Drawing;
using System;
using System.Linq;
using data_rogue_core.Maps;
using System.Windows.Forms;

namespace data_rogue_core.Activities
{
    public class InformationActivity : BaseActivity
    {
        public override ActivityType Type => ActivityType.Information;
        public override bool RendersEntireSpace { get; }
        public IEntity Entity { get; }
        public List<StatsConfiguration> StatsConfigs { get; set; }
        public bool CloseOnKeyPress { get; }
        public override bool AcceptsInput => true;

        private readonly IActivitySystem _activitySystem;

        public InformationActivity(IActivitySystem activitySystem, List<StatsConfiguration> statsConfigs, IEntity entity, bool closeOnKeyPress = true, bool rendersEntireSpace = false)
        {
            Entity = entity;
            StatsConfigs = statsConfigs;
            CloseOnKeyPress = closeOnKeyPress;
            RendersEntireSpace = rendersEntireSpace;
            _activitySystem = activitySystem;
        }

        public override IEnumerable<IDataRogueControl> GetLayout(IUnifiedRenderer renderer, ISystemContainer systemContainer, object rendererHandle, List<IDataRogueControlRenderer> controlRenderers, List<MapCoordinate> playerFov, int width, int height)
        {
            List<IDataRogueControl> controls = new List<IDataRogueControl>();

            var maxX = 0;
            var maxY = 0;

            foreach(var config in StatsConfigs)
            {
                var x = config.Position.X + renderer.ActivityPadding.Left;
                var y = config.Position.Y + renderer.ActivityPadding.Top;

                foreach(var display in config.Displays)
                {
                    var controlType = display.ControlType;

                    var control = (IDataRogueInfoControl)Activator.CreateInstance(controlType);
                    control.SetData(Entity, display);

                    var controlRenderer = controlRenderers.Single(s => s.DisplayType == control.GetType());
                    var size = controlRenderer.GetSize(rendererHandle, control, systemContainer, playerFov);

                    control.Position = new Rectangle(x, y, size.Width, size.Height);

                    y += size.Height;

                    if (control.Position.Width > maxX) { maxX = control.Position.Width; }
                    maxY = y;

                    controls.Add(control);
                }
            }

            maxX += 8;
            maxY += 8;

            var spareWidth = width - maxX;
            var spareHeight = height - maxY;

            var left = spareWidth / 2;
            var top = spareHeight / 2;

            if (!RendersEntireSpace)
            {

                foreach (var control in controls)
                {
                    control.Position = new Rectangle(control.Position.Left + left, control.Position.Top + top, control.Position.Width, control.Position.Height);
                }

                controls.Add(new BackgroundControl { Position = new Rectangle(spareWidth / 2, spareHeight / 2, maxX, maxY) });

            }
            else
            {
                controls.Add(new BackgroundControl { Position = new Rectangle(0, 0, width, height) });
            }

            return controls;
        }

        public void Initialise()
        {
        }
        
        public override void HandleKeyboard(ISystemContainer systemContainer, KeyCombination keyboard)
        {
            if (keyboard != null && keyboard.Key != Key.Unknown && CloseOnKeyPress)
            {
                Close();
            }
        }

        public override void HandleMouse(ISystemContainer systemContainer, MouseData mouse)
        {
            if (mouse.IsLeftClick)
            {
                Close();
            }
        }

        public override void HandleAction(ISystemContainer systemContainer, ActionEventData action)
        {
        }

        private void Close()
        {
            _activitySystem.RemoveActivity(this);
        }
    }
}
