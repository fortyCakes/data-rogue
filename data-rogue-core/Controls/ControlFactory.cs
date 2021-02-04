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
        public static IEnumerable<IDataRogueControl> GetControls(
            IEnumerable<IRenderingConfiguration> configurations, 
            IUnifiedRenderer renderer, 
            ISystemContainer systemContainer, 
            object rendererHandle, 
            List<IDataRogueControlRenderer> controlRenderers, 
            List<MapCoordinate> playerFov)
        {
            var controls = new List<IDataRogueControl>();

            foreach (IRenderingConfiguration statsConfiguration in configurations)
            {
                controls.AddRange(CreateControls(renderer, systemContainer, rendererHandle, controlRenderers, playerFov, statsConfiguration));
            }

            return controls;
        }

        private static IEnumerable<IDataRogueControl> CreateControls(IUnifiedRenderer renderer, ISystemContainer systemContainer, object rendererHandle, List<IDataRogueControlRenderer> controlRenderers, List<MapCoordinate> playerFov, IRenderingConfiguration renderingConfiguration)
        {
            var x = renderingConfiguration.Position.X + renderer.ActivityPadding.Left;
            var y = renderingConfiguration.Position.Y + renderer.ActivityPadding.Top;

            foreach (var display in renderingConfiguration.Displays)
            {
                var controlType = display.ControlType;

                var instance = Activator.CreateInstance(controlType);
                var control = (IDataRogueControl)instance;

                if (control is IDataRogueInfoControl)
                {
                    (control as IDataRogueInfoControl).SetData(systemContainer.PlayerSystem.Player, display);
                }

                if (control.FillsContainer)
                {
                    control.Position = renderingConfiguration.Position;
                }
                else
                {
                    control.Position = new Rectangle(control.Position.X, control.Position.Y, renderingConfiguration.Position.Width, 0);

                    var controlRenderer = controlRenderers.Single(s => s.DisplayType == control.GetType());
                    var size = controlRenderer.GetSize(rendererHandle, control, systemContainer, playerFov);

                    control.Position = new Rectangle(x, y, size.Width, size.Height);

                    y += size.Height;
                }                

                yield return control;
            }
        }
    }
}