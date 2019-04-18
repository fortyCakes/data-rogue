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

namespace data_rogue_core.Activities
{
    public class InformationActivity : IActivity
    {
        public ActivityType Type => ActivityType.Information;
        public object Data => StatsConfigs;
        public bool RendersEntireSpace { get; set; }
        public IEntity Entity { get; }
        public List<StatsConfiguration> StatsConfigs { get; set; }
        public IUnifiedRenderer Renderer { get; private set; }
        public bool CloseOnKeyPress { get; }

        private StatsDisplayTypeMapping _dictionary;

        public bool Running => true;

        private readonly IActivitySystem _activitySystem;

        public InformationActivity(IActivitySystem activitySystem, List<StatsConfiguration> statsConfigs, IEntity entity, bool closeOnKeyPress = true, bool rendersEntireSpace = false)
        {
            Entity = entity;
            StatsConfigs = statsConfigs;
            CloseOnKeyPress = closeOnKeyPress;
            RendersEntireSpace = rendersEntireSpace;
            _activitySystem = activitySystem;
        }

        public void Render(ISystemContainer systemContainer)
        {
            Renderer.Render(systemContainer, this);
        }

        public IEnumerable<IDataRogueControl> GetLayout(ISystemContainer systemContainer, object rendererHandle, List<IDataRogueControlRenderer> controlRenderers, List<MapCoordinate> playerFov, int width, int height)
        {
            yield return new BackgroundControl { Position = new Rectangle(0, 0, width, height) };

            foreach(var config in StatsConfigs)
            {
                var y = config.Position.Y;

                foreach(var display in config.Displays)
                {
                    var controlType = display.ControlType;

                    var control = (IDataRogueInfoControl)Activator.CreateInstance(controlType);
                    control.SetData(Entity, display);

                    var renderer = controlRenderers.Single(s => s.DisplayType == control.GetType());
                    var size = renderer.GetSize(rendererHandle, control, systemContainer, playerFov);

                    var position = new Rectangle(config.Position.X, y, size.Width, size.Height);
                    control.Position = position;

                    y += size.Height;

                    yield return control;
                }
            }
        }

        public void Initialise(IRenderer renderer)
        {
            Renderer = (IUnifiedRenderer)renderer;
        }
        
        public void HandleKeyboard(ISystemContainer systemContainer, KeyCombination keyboard)
        {
            if (keyboard != null && keyboard.Key != Key.Unknown && CloseOnKeyPress)
            {
                Close();
            }
        }

        public void HandleMouse(ISystemContainer systemContainer, MouseData mouse)
        {
            if (mouse.IsLeftClick)
            {
                Close();
            }
        }

        public void HandleAction(ISystemContainer systemContainer, ActionEventData action)
        {
            //throw new System.NotImplementedException();
        }

        private void Close()
        {
            _activitySystem.RemoveActivity(this);
        }
    }
}
