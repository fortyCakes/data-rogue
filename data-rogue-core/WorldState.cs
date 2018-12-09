using data_rogue_core.EntitySystem;
using System.Linq;
using data_rogue_core.Maps;

namespace data_rogue_core
{
    public class WorldState
    {
        public WorldState(IEntityEngineSystem entityEngineSystem)
        {
            EntityEngineSystem = entityEngineSystem;
        }

        public MapCollection Maps;

        public IEntity Player;

        public Map CurrentMap { get; set; }

        public IEntityEngineSystem EntityEngineSystem { get; private set; }

        public SaveState GetSaveState()
        {
            return new SaveState
            {
                Entities = EntityEngineSystem.AllEntities,
                Maps = Maps.AllMaps.Select(m => m.Serialize()).ToList(),
                CurrentMapKey = CurrentMap.MapKey.Key
            };
        }
    }
}