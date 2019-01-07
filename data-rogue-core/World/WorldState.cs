using data_rogue_core.EntityEngine;
using System.Linq;
using data_rogue_core.Maps;
using data_rogue_core.Components;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core
{
    public class WorldState
    {
        public WorldState(IEntityEngine entityEngineSystem, ITimeSystem timeSystem, string seed)
        {
            EntityEngineSystem = entityEngineSystem;
            TimeSystem = timeSystem;
            Seed = seed;
        }

        public MapCollection Maps = new MapCollection();

        public IEntity Player;

        //public Map CurrentMap => Maps[Player.Get<Position>().MapCoordinate.Key];

        public MapCoordinate CameraPosition => Player.Get<Position>().MapCoordinate;

        public IEntityEngine EntityEngineSystem { get; private set; }
        public ITimeSystem TimeSystem { get; }
        public string Seed { get; }

        public SaveState GetSaveState()
        {
            return new SaveState
            {
                Entities = EntityEngineSystem.MutableEntities.Select(e => EntitySerializer.Serialize(e)).ToList(),
                Maps = Maps.AllMaps.Select(m => MapSerializer.Serialize(m)).ToList(),
                Time = TimeSystem.CurrentTime
            };
        }
    }
}