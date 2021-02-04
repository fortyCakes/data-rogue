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
                }
            }
        }

        public override IEnumerable<IDataRogueControl> GetLayout(IUnifiedRenderer renderer, ISystemContainer systemContainer, object rendererHandle, List<IDataRogueControlRenderer> controlRenderers, List<MapCoordinate> playerFov, int width, int height)
        {
            var controls = new List<IDataRogueControl>();

            var player = systemContainer.PlayerSystem.Player;

            controls.Add( new LinesControl { Position = new Rectangle(0, 0, width, height), Configuration = _ioSystemConfiguration });

            controls.AddRange(ControlFactory.GetMapControls(_ioSystemConfiguration.MapConfigurations));

            controls.AddRange(ControlFactory.GetStatsControls(_ioSystemConfiguration.StatsConfigurations, renderer, systemContainer, rendererHandle, controlRenderers, playerFov, player));

            controls.AddRange(ControlFactory.GetMessageControls(_ioSystemConfiguration.MessageConfigurations));

            return controls;
        }
    }
}