using data_rogue_core.EntityEngineSystem;
using RLNET;

namespace data_rogue_core.Systems
{
    public interface IPlayerControlSystem 
    {
        void HandleKeyPress(RLKeyPress keyPress);
        void HandleMouseInput(RLMouse mouse);

        IEntity HoveredEntity { get; }
    }
}
