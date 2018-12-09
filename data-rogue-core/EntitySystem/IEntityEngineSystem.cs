using System.Collections.Generic;

using data_rogue_core.Systems;

namespace data_rogue_core.EntitySystem
{
    public interface IEntityEngineSystem : IInitialisableSystem
    {
        void Destroy(Entity entity);
        Entity New(params IEntityComponent[] components);
        Entity New(string Name, params IEntityComponent[] components);

        Entity GetEntity(uint entityId);

        void Load(uint EntityId, Entity entity);

        void Register(ISystem system);

        List<Entity> AllEntities { get; }
    }
}