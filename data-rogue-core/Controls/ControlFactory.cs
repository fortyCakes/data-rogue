using data_rogue_core.Activities;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.IOSystems;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace data_rogue_core.Controls
{
    public class ControlFactory
    {
        public static IEnumerable<IDataRogueControl> GetMapControls(IEnumerable<MapConfiguration> mapConfigurations)
        {
            foreach (var mapConfiguration in mapConfigurations)
            {
                if (mapConfiguration.GetType() == typeof(MinimapConfiguration))
                {
                    yield return new MinimapControl
                    {
                        Position = mapConfiguration.Position
                    };
                }

                if (mapConfiguration.GetType() == typeof(MapConfiguration))
                {
                    yield return new MapControl { Position = mapConfiguration.Position };
                }
            }
        }

        public static IEnumerable<IDataRogueControl> GetStatsControls(
            List<StatsConfiguration> statsConfigurations, 
            IUnifiedRenderer renderer, 
            ISystemContainer systemContainer, 
            object rendererHandle, 
            List<IDataRogueControlRenderer> controlRenderers, 
            List<MapCoordinate> playerFov, 
            IEntity player)
        {
            foreach (var statsConfiguration in statsConfigurations)
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
        }

        internal static IEnumerable<IDataRogueControl> GetMessageControls(List<MessageConfiguration> messageConfigurations)
        {
            foreach (var messageConfiguration in messageConfigurations)
            {
                yield return new MessageLogControl { Position = messageConfiguration.Position, NumberOfMessages = messageConfiguration.NumberOfMessages };
            }
        }
    }
}