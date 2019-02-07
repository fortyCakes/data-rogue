using RLNET;

namespace data_rogue_core.Systems
{
    public interface IPlayerControlSystem 
    {
        void HandleKeyPress(RLKeyPress keyPress);
    }
}
