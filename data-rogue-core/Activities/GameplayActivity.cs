using data_rogue_core.Renderers;
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
    }
}