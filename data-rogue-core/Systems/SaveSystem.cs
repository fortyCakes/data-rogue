﻿using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;
using System.IO;
using System.Reflection;

namespace data_rogue_core
{
    public class SaveSystem
    {
        public static WorldState Load(ISystemContainer systemContainer)
        {
            var directoryName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Saves");
            var fileName = Path.Combine(directoryName, "saveFile.sav");

            var loadedState = SaveStateSerializer.Deserialize(File.ReadAllText(fileName));

            systemContainer.TimeSystem.CurrentTime = loadedState.Time;

            systemContainer.Seed = loadedState.Seed;

            var world = new WorldState(systemContainer);

            systemContainer.EntityEngine.Initialise(systemContainer);

            foreach (var savedEntity in loadedState.Entities)
            {
                var entity = EntitySerializer.Deserialize(systemContainer, savedEntity);
                if (entity.Name == "Player")
                {
                    world.Player = entity;
                }
            }

            foreach(var savedMap in loadedState.Maps)
            {
                var map = MapSerializer.Deserialize(systemContainer, savedMap);

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