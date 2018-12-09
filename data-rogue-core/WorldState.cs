using data_rogue_core.Data;
using data_rogue_core.EntitySystem;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

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