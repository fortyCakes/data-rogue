using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using RLNET;

namespace data_rogue_core.Systems
{
    public interface IPlayerControlSystem 
    {
        void HandleKeyPress(RLKeyPress keyPress);
        void HandleMouseInput(RLMouse mouse);

        MapCoordinate HoveredCoordinate { get; }
    }
}
