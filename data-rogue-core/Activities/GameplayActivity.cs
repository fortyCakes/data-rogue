using data_rogue_core.EventSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.IOSystems;
using data_rogue_core.Maps;
using data_rogue_core.Renderers;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Utils;
using System.Linq;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using System.Collections.Generic;
using data_rogue_core.Controls;
using System;
using System.Drawing;

namespace data_rogue_core.Activities
{
    public class GameplayActivity : BaseActivity
    {
        public override ActivityType Type => ActivityType.Gameplay;

        public bool Running { get; set; } = false;

        public override bool RendersEntireSpace => true;
        public override bool AcceptsInput => true;

        private readonly IOSystemConfiguration _ioSystemConfiguration;

        public GameplayActivity(IOSystemConfiguration ioSystemConfiguration)
        {
            _ioSystemConfiguration = ioSystemConfiguration;
        }

        public void Initialise()
        {
        }

        public override void HandleAction(ISystemContainer systemContainer, ActionEventData action)
        {
            if (systemContainer.TimeSystem.WaitingForInput && action != null)
            {
                systemContainer.EventSystem.Try(EventType.Action, systemContainer.PlayerSystem.Player, action);
            }
        }

        public override void HandleKeyboard(ISystemContainer systemContainer, KeyCombination keyboard)
        {

        }

        public override void HandleMouse(ISystemContainer systemContainer, MouseData mouse)
        {
            

            IDataRogueControl mouseOverControl = systemContainer.RendererSystem.Renderer.GetControlFromMousePosition(
                systemContainer, 
                this, 
                systemContainer.RendererSystem.CameraPosition, 
                mouse.X, 
                mouse.Y);

            if (mouseOverControl != null && mouseOverControl.CanHandleMouse)
            {
                var renderer = systemContainer.RendererSystem.Renderer.GetRendererFor(mouseOverControl);

                var action = mouseOverControl.HandleMouse(mouse, renderer, systemContainer);
                if (action != null)
                {
                    systemContainer.EventSystem.Try(EventType.Action, systemContainer.PlayerSystem.Player, action);
                    return; // If the control tried to do an action, it handled the mouse. Stop here.
                }
            }
        }

        public override IEnumerable<IDataRogueControl> GetLayout(IUnifiedRenderer renderer, ISystemContainer systemContainer, object rendererHandle, List<IDataRogueControlRenderer> controlRenderers, List<MapCoordinate> playerFov, int width, int height)
        {
            yield return new LinesControl { Position = new Rectangle(0, 0, width, height), Configuration = _ioSystemConfiguration };

            foreach (var mapConfiguration in _ioSystemConfiguration.MapConfigurations)
            {
                if (mapConfiguration.GetType() == typeof(MinimapConfiguration))
                {
                    yield return new MinimapControl { Position = mapConfiguration.Position };
                }

                if (mapConfiguration.GetType() == typeof(MapConfiguration))
                {
                    yield return new MapControl {Position = mapConfiguration.Position};
                }
            }

            var player = systemContainer.PlayerSystem.Player;

            foreach (var statsConfiguration in _ioSystemConfiguration.StatsConfigurations)
            {
                var x = statsConfiguration.Position.X + renderer.ActivityPadding.Left;
                var y = statsConfiguration.Position.Y + renderer.ActivityPadding.Top;

                foreach (var display in statsConfiguration.Displays)
                {
                    var controlType = display.ControlType;

                    var instance = Activator.CreateInstance(controlType);
                    var control = (IDataRogueInfoControl)instance;
                    control.SetData(player, display);
                    control.Position = new Rectangle(control.Position.X, control.Position.Y, statsConfiguration.Position.Width, 0);

                    var controlRenderer = controlRenderers.Single(s => s.DisplayType == control.GetType());
                    var size = controlRenderer.GetSize(rendererHandle, control, systemContainer, playerFov);

                    control.Position = new Rectangle(x, y, size.Width, size.Height);

                    y += size.Height;

                    yield return control;
                }
            }

            foreach(var messageConfiguration in _ioSystemConfiguration.MessageConfigurations)
            {
                yield return new MessageLogControl { Position = messageConfiguration.Position, NumberOfMessages = messageConfiguration.NumberOfMessages };
            }
        }
    }
}