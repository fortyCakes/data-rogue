using data_rogue_core.EntityEngineSystem;
using System.Linq;
using data_rogue_core.Maps;
using data_rogue_core.Components;
using data_rogue_core.Data;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core
{
    public class WorldState
    {
        public WorldState(ISystemContainer systemContainer)
        {
            EntityEngineSystem = systemContainer.EntityEngine;
            TimeSystem = systemContainer.TimeSystem;
            MessageSystem = systemContainer.MessageSystem;
            Seed = systemContainer.Seed;
        }

        public MapCollection Maps = new MapCollection();

        public IEntity Player;

        public MapCoordinate CameraPosition => Player.Get<Position>().MapCoordinate;

        public IEntityEngine EntityEngineSystem { get; }
        public ITimeSystem TimeSystem { get; }

        public IMessageSystem MessageSystem { get; }
        public string Seed { get; }

        public SaveState GetSaveState()
        {
            return new SaveState
            {
                Entities = EntityEngineSystem.MutableEntities.Select(e => EntitySerializer.Serialize(e)).ToList(),
                Maps = Maps.AllMaps.Select(m => MapSerializer.Serialize(m)).ToList(),
                Time = TimeSystem.CurrentTime,
                Messages = MessageSystem.AllMessages.Select(m => MessageSerializer.Serialize(m)).ToList()
            };
        }
    }
}