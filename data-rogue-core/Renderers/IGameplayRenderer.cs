using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.Renderers
{
    public interface IGameplayRenderer : IRenderer
    {
        void Render(WorldState worldState, IPositionSystem positionSystem, IMessageSystem messageSystem);
    }
}