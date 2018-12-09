using data_rogue_core.Data;
using data_rogue_core.EntitySystem;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;

namespace data_rogue_core
{
    public class SaveSystem
    {
        public static WorldState Load(IEntityEngineSystem entityEngineSystem)
        {
            var world = new WorldState(entityEngineSystem);

            var directoryName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Saves");
            var fileName = Path.Combine(directoryName, "saveFile.sav");

            var loadedState = JsonConvert.DeserializeObject<SaveState>(File.ReadAllText(fileName));

            entityEngineSystem.Initialise();

            foreach (var entity in loadedState.Entities)
            {
                entityEngineSystem.Load(entity.EntityId, entity);
            }

            foreach(var savedMap in loadedState.Maps)
            {
                var map = Map.Deserialize(savedMap, entityEngineSystem);

                world.Maps.Add(map.MapKey, map);
            }

            return world;
        }

        public static void Save(WorldState world)
        {
            var directoryName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Saves");
            var fileName = Path.Combine(directoryName, "saveFile.sav");

            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }

            var saveState = world.GetSaveState();

            File.WriteAllText(fileName, JsonConvert.SerializeObject(saveState));
        }
    }
}