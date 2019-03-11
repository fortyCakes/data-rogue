using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;
using System.IO;
using System.Reflection;
using data_rogue_core.Data;
using System;

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

            systemContainer.MapSystem.Initialise();

            systemContainer.EntityEngine.Initialise(systemContainer);

            foreach (var savedEntity in loadedState.Entities)
            {
                var entity = EntitySerializer.Deserialize(systemContainer, savedEntity);
                if (entity.Name == "Player")
                {
                    systemContainer.PlayerSystem.Player = entity;
                }
            }

            foreach(var savedMap in loadedState.Maps)
            {
                var map = MapSerializer.Deserialize(systemContainer, savedMap);

                systemContainer.MapSystem.MapCollection.Add(map.MapKey, map);
            }

            systemContainer.MessageSystem.Initialise();

            foreach (var savedMessage in loadedState.Messages)
            {
                var message = MessageSerializer.Deserialize(savedMessage);

                systemContainer.MessageSystem.AllMessages.Add(message);
            }

            return world;
        }

        public static void Save()
        {
            var directoryName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Saves");
            var fileName = Path.Combine(directoryName, "saveFile.sav");

            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }

            var saveState = GetSaveState();

            File.WriteAllText(fileName, SaveStateSerializer.Serialize(saveState));
        }

        private static SaveState GetSaveState()
        {
            throw new NotImplementedException();
        }
    }
}