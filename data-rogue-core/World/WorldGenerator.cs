using data_rogue_core.Components;
using data_rogue_core.Data;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using System;
using System.Linq;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Forms.StaticForms;

namespace data_rogue_core
{
    public static class WorldGenerator
    {
        public static WorldState Create(ISystemContainer systemContainer, CharacterCreationForm characterCreationForm)
        {
            systemContainer.MessageSystem.Initialise();

            systemContainer.EntityEngine.Initialise(systemContainer);

            var world = new WorldState(systemContainer);

            (new WorldEntityLoader()).Load(systemContainer);

            var spawnPoint = CreateInitialMapAndGetSpawnPoint(systemContainer, world);

            AddPlayerToWorld(systemContainer, world, spawnPoint,  characterCreationForm);

            return world;
        }

        private static MapCoordinate CreateInitialMapAndGetSpawnPoint(ISystemContainer systemContainer, WorldState world)
        {
            var worldStructure = systemContainer.PrototypeSystem.Get("World").Get<Components.World>();
            var initialBranchEntity = systemContainer.PrototypeSystem.Get(worldStructure.InitialBranch);

            GenerateBranch(systemContainer, world, initialBranchEntity);

            var initialMap = world.Maps[new MapKey($"{initialBranchEntity.Get<Branch>().BranchName}:1")];

            return GetSpawnPoint(initialMap);
        }

        private static MapCoordinate GetSpawnPoint(Map initialMap)
        {
            var definedSpawnPoint = initialMap.Cells.Where(c => c.Value.Has<SpawnPoint>());

            if (definedSpawnPoint.Count() == 0)
            {
                throw new ApplicationException("No SpawnPoint on initial map.");
            }

            return definedSpawnPoint.First().Key;
        }

        public static void GenerateBranch(ISystemContainer systemContainer, WorldState world, IEntity branchEntity)
        {
            var branchGenerator = MapGeneratorFactory.GetGenerator(branchEntity.Get<Branch>().MapGenerationType);

            GeneratedBranch branch = branchGenerator.Generate(systemContainer, branchEntity);

            foreach (Map map in branch.Maps)
            {
                world.Maps.AddMap(map);
            }
        }

        private static void AddPlayerToWorld(ISystemContainer systemContainer, WorldState world, MapCoordinate spawnPoint, CharacterCreationForm form)
        {
            var player = EntitySerializer.Deserialize(systemContainer, DataFileLoader.LoadFile(@"Entities\player.edt"));
            player.Get<Position>().MapCoordinate = spawnPoint;
            player.Get<Description>().Name = form.Name;

            var fighter = player.Get<Fighter>();

            fighter.Agility = form.Agility;
            fighter.Muscle = form.Muscle;
            fighter.Intellect = form.Intellect;
            fighter.Willpower = form.Willpower;

            world.Player = player;
        }
    }
}