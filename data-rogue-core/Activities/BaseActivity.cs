using data_rogue_core.EventSystem;
using data_rogue_core.IOSystems;
using data_rogue_core.Maps;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;
using System.Collections.Generic;
using System.Drawing;
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
            if (systemContainer.ActivitySystem.ActivityStack.Contains(this))
            {
                var mouseOverControl = GetMouseOverControl(systemContainer, mouse);

                if (mouseOverControl != null)
                {
                    if (mouseOverControl.CanHandleMouse)
                    {
                        var renderer = systemContainer.RendererSystem.Renderer.GetRendererFor(mouseOverControl);

                        var action = mouseOverControl.HandleMouse(mouse, renderer, systemContainer);
                        if (action != null)
                        {
                            systemContainer.EventSystem.Try(EventType.Action, systemContainer.PlayerSystem.Player, action);
                        }
                    }

                    if (mouse.IsLeftClick)
                    {
                        mouseOverControl.Click(mouse, new PositionEventHandlerArgs(mouse.X, mouse.Y));
                    }
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

        protected static void SetSize(IDataRogueControl control, ISystemContainer systemContainer, object rendererHandle, List<IDataRogueControlRenderer> controlRenderers, List<MapCoordinate> playerFov)
        {
            var size = controlRenderers.Single(c => c.DisplayType == control.GetType()).GetSize(rendererHandle, control, systemContainer, playerFov);
            control.Position = new Rectangle(control.Position.Location, size);
        }
        
        protected void CenterControls(List<IDataRogueControl> controls, int width, int height, int finalWidth, int finalHeight)
        {
            var x = width / 2 - finalWidth / 2;
            var y = height / 2 - finalHeight / 2;


            foreach (var control in controls)
            {
                control.Position = new Rectangle(control.Position.Left + x, control.Position.Top + y, control.Position.Width, control.Position.Height);
            }
        }
    }
}