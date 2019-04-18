using data_rogue_core.IOSystems;
using data_rogue_core.Maps;

namespace data_rogue_core.Systems
{
    public interface IControlSystem : IInitialisableSystem
    {
        void HandleInput(KeyCombination keyPress, MouseData mouse);

        MapCoordinate HoveredCoordinate { get; set; }
    }
}
