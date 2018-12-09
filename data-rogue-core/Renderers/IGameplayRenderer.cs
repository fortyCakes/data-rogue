using data_rogue_core.Systems;

namespace data_rogue_core.Renderers
{
    public interface IGameplayRenderer : IRenderer
    {
        void Render(WorldState worldState, IPositionSystem positionSystem);
    }
}