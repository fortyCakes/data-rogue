using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.Renderers
{
    public interface IGameplayRenderer : IRenderer
    {
        void Render(ISystemContainer systemContainer);
        MapCoordinate GetMapCoordinateFromMousePosition(MapCoordinate cameraPosition, int x, int y);
    }
}