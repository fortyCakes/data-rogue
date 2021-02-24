using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.Systems
{
    public class FactionSystem : IFactionSystem
    {
        public FactionSystem()
        {
        }

        public Faction FactionOf(IEntity entity)
        {
            return entity.TryGet<Actor>()?.Faction;
        }

        public bool IsSameFaction(IEntity entity, IEntity entity2)
        {
            return FactionOf(entity)?.Equals(FactionOf(entity2)) ?? false;
        }
    }
}