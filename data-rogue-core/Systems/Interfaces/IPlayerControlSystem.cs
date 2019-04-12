using data_rogue_core.Components;
using data_rogue_core.IOSystems;
using data_rogue_core.Maps;
using RLNET;

namespace data_rogue_core.Systems
{
    public interface IPlayerControlSystem : IInitialisableSystem
    {
        void HandleInput(KeyCombination keyPress, MouseData mouse);

        MapCoordinate HoveredCoordinate { get; set; }
    }
}
