using data_rogue_core.EntityEngine;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;
using System.IO;
using System.Linq;
using System.Reflection;

namespace data_rogue_core
{
    public class SaveSystem
    {
        public static WorldState Load(IEntityEngine entityEngineSystem, ITimeSystem timeSystem, IPrototypeSystem prototypeSystem)
        {
            var directoryName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Saves");
            var fileName = Path.Combine(directoryName, "saveFile.sav");

            var loadedState = SaveStateSerializer.Deserialize(File.ReadAllText(fileName), entityEngineSystem);

            timeSystem.CurrentTime = loadedState.Time;

            var world = new WorldState(entityEngineSystem, timeSystem, loadedState.Seed);

            entityEngineSystem.Initialise();

            foreach (var savedEntity in loadedState.Entities)
            {
                var entity = EntitySerializer.Deserialize(savedEntity, entityEngineSystem);
                if (entity.Name == "Player")
                {
                    world.Player = entity;
                }
            }

            foreach(var savedMap in loadedState.Maps)
            {
                var map = MapSerializer.Deserialize(savedMap, entityEngineSystem, prototypeSystem);

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

            File.WriteAllText(fileName, SaveStateSerializer.Serialize(saveState));
        }
    }
}