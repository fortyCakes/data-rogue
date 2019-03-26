using data_rogue_core.EventSystem;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EntityEngineSystem
{
    public interface ITickUpdate : IEntityComponent
    {
        void Tick(IEventSystem eventSystem, IPlayerSystem playerSystem, IStatSystem statSystem, IEntity entity, ulong currentTime);
    }
}