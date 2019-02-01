using data_rogue_core.EventSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.Renderers
{
    public interface IGameplayRenderer : IRenderer
    {
        void Render(WorldState worldState, ISystemContainer systemContainer);
        MapCoordinate GetMapCoordinateFromMousePosition(WorldState world, int x, int y);
    }
}