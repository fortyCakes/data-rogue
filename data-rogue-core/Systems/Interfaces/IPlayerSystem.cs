using data_rogue_core.EntityEngineSystem;

namespace data_rogue_core.Systems.Interfaces
{
    public interface IPlayerSystem
    {
        IEntity Player { get; set; }

        bool IsPlayer(IEntity sender);
    }
}
