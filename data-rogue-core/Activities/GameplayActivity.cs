using data_rogue_core.EventSystem;
using data_rogue_core.IOSystems;
using data_rogue_core.Maps;
using data_rogue_core.Renderers;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.Activities
{
    public class GameplayActivity : IActivity
    {
        public ActivityType Type => ActivityType.Gameplay;
        public object Data => null;

        public bool RendersEntireSpace => true;
        public IGameplayRenderer Renderer { get; set; }

        public void Render(ISystemContainer systemContainer)
        {
            Renderer.Render(systemContainer);
        }

        public void Initialise(IRenderer renderer)
        {
            Renderer = (IGameplayRenderer)renderer;
        }

        public void HandleAction(ISystemContainer systemContainer, ActionEventData action)
        {
            if (systemContainer.TimeSystem.WaitingForInput && action != null)
            {
                systemContainer.EventSystem.Try(EventType.Action, systemContainer.PlayerSystem.Player, action);
            }
        }

        public void HandleKeyboard(ISystemContainer systemContainer, KeyCombination keyboard)
        {
            //throw new System.NotImplementedException();
        }

        public void HandleMouse(ISystemContainer systemContainer, MouseData mouse)
        {
            systemContainer.PlayerControlSystem.HoveredCoordinate = Renderer.GetMapCoordinateFromMousePosition(systemContainer.RendererSystem.CameraPosition, mouse.X, mouse.Y);
        }
    }
}