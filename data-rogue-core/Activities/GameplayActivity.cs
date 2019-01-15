using data_rogue_core.Renderers;

namespace data_rogue_core.Activities
{
    public class GameplayActivity : IActivity
    {
        public ActivityType Type => ActivityType.Gameplay;
        public object Data => null;

        public bool RendersEntireSpace => true;
        public IGameplayRenderer Renderer { get; set; }

        public GameplayActivity(IRendererFactory rendererFactory)
        {
            Renderer = (IGameplayRenderer)rendererFactory.GetRendererFor(Type);
        }
        public void Render()
        {
            Renderer.Render(Game.WorldState, Game.PositionSystem, Game.MessageSystem, Game.EventSystem);
        }
    }
}