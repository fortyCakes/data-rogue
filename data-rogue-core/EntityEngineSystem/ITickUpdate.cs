using data_rogue_core.EventSystem;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EntityEngineSystem
{
    public interface ITickUpdate : IEntityComponent
    {
        void Tick(ISystemContainer systemContainer, IEntity entity, ulong currentTime);
    }
}