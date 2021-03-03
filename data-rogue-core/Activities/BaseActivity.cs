using data_rogue_core.EventSystem;
using data_rogue_core.IOSystems;
using data_rogue_core.Maps;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace data_rogue_core.Activities
{
    public abstract class BaseActivity : IActivity
    {
        public BaseActivity(Rectangle position, Padding padding)
        {
            Position = position;
            Padding = padding;
        }

        public bool Initialised { get; set; } = false;

        public abstract void InitialiseControls();

        public virtual void Layout(List<IDataRogueControlRenderer> controlRenderers, ISystemContainer systemContainer, List<MapCoordinate> playerFov, object handle)
        {
            if (!Initialised)
            {
                InitialiseControls();
                Initialised = true;
            }

            OnLayout?.Invoke(null, null);

            foreach (var control in Controls.ToList())
            {
                if (control.Visible)
                {
                    control.Layout(controlRenderers, systemContainer, handle, playerFov, Position);
                }
            }
        }

        public event EventHandler OnLayout;

        public IList<IDataRogueControl> Controls { get; set; } = new List<IDataRogueControl>();
        public abstract ActivityType Type { get; }
        public abstract bool RendersEntireSpace { get; }
        public abstract bool AcceptsInput { get; }

        public Rectangle Position { get; set; }
        public Padding Padding { get; set; }
        
        public abstract void HandleAction(ISystemContainer systemContainer, ActionEventData action);
        public abstract void HandleKeyboard(ISystemContainer systemContainer, KeyCombination keyboard);
        public virtual void HandleMouse(ISystemContainer systemContainer, MouseData mouse)
        {
            if (systemContainer.ActivitySystem.HasActivity(this))
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
    }
}