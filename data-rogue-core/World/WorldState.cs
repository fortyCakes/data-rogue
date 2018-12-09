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

        public MapCollection Maps = new MapCollection();

        public IEntity Player;

        public Map CurrentMap { get; set; }

        public IEntityEngineSystem EntityEngineSystem { get; private set; }

        public SaveState GetSaveState()
        {
            return new SaveState
            {
                Entities = EntityEngineSystem.MutableEntities.Select(e => EntitySerializer.Serialize(e)).ToList(),
                Maps = Maps.AllMaps.Select(m => MapSerializer.Serialize(m)).ToList(),
                CurrentMapKey = CurrentMap.MapKey.Key
            };
        }
    }
}