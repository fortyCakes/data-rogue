using data_rogue_core.Renderers;
using data_rogue_core.Systems;

namespace data_rogue_core.Activities
{
    public interface IGameplayRenderer : IRenderer
    {
        void Render(WorldState worldState, IPositionSystem positionSystem);
    }
}