using data_rogue_core.EventSystem;
using data_rogue_core.IOSystems;
using data_rogue_core.Maps;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace data_rogue_core.Activities
{
    public abstract class BaseActivity : IActivity
    {

        public IEnumerable<IDataRogueControl> Controls { get; set; } = new List<IDataRogueControl>();
        public abstract ActivityType Type { get; }
        public abstract bool RendersEntireSpace { get; }
        public abstract bool AcceptsInput { get; }

        public void Layout(IUnifiedRenderer renderer, ISystemContainer systemContainer, object rendererHandle, List<IDataRogueControlRenderer> controlRenderers, List<MapCoordinate> playerFov, int width, int height)
        {
            Controls = GetLayout(renderer, systemContainer, rendererHandle, controlRenderers, playerFov, width, height).ToList();
        }

        public abstract IEnumerable<IDataRogueControl> GetLayout(IUnifiedRenderer renderer, ISystemContainer systemContainer, object rendererHandle, List<IDataRogueControlRenderer> controlRenderers, List<MapCoordinate> playerFov, int width, int height);

        public abstract void HandleAction(ISystemContainer systemContainer, ActionEventData action);
        public abstract void HandleKeyboard(ISystemContainer systemContainer, KeyCombination keyboard);
        public virtual void HandleMouse(ISystemContainer systemContainer, MouseData mouse)
        {
            var mouseOverControl = GetMouseOverControl(systemContainer, mouse);

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

        protected IDataRogueControl GetMouseOverControl(ISystemContainer systemContainer, MouseData mouse)
        {
            return systemContainer.RendererSystem.Renderer.GetControlFromMousePosition(
                systemContainer,
                this,
                systemContainer.RendererSystem.CameraPosition,
                mouse.X,
                mouse.Y);
        }
    }
}