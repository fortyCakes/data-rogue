using data_rogue_core.EntityEngineSystem;
using System.Linq;
using data_rogue_core.Maps;
using data_rogue_core.Components;
using data_rogue_core.Data;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Systems;

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
            PlayerSystem = systemContainer.PlayerSystem;
        }

        private IMapSystem mapSystem;
        private IEntityEngine EntityEngineSystem;
        private ITimeSystem TimeSystem;
        private IMessageSystem MessageSystem;
        private IPlayerSystem PlayerSystem;

        public string Seed { get; }

        public SaveState GetSaveState()
        {
            return new SaveState
            {
                Entities = EntityEngineSystem.MutableEntities.Select(e => EntitySerializer.Serialize(e)).ToList(),
                Maps = mapSystem.MapCollection.AllMaps.Select(m => MapSerializer.Serialize(m)).ToList(),
                Time = TimeSystem.CurrentTime,
                Messages = MessageSystem.AllMessages.Select(m => MessageSerializer.Serialize(m)).ToList()
            };
        }
    }
}