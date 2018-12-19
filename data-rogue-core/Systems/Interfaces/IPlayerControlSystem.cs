using data_rogue_core.EntityEngine;
using RLNET;

namespace data_rogue_core.Systems
{
    public interface IPlayerControlSystem : ISystem
    {
        void HandleKeyPress(RLKeyPress keyPress);
    }
}
