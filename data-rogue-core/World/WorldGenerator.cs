using data_rogue_core.Components;
using data_rogue_core.Data;
using data_rogue_core.EntitySystem;
using data_rogue_core.Maps;
using data_rogue_core.Maps.Generators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace data_rogue_core
{
    public class WorldGenerator
    {
        public static WorldState Create(string seed, IEntityEngineSystem entityEngineSystem)
        {
            entityEngineSystem.Initialise();

            var world = new WorldState(entityEngineSystem, seed);

            var spawnPoint = CreateInitialMapAndGetSpawnPoint(seed, entityEngineSystem, world);

            AddPlayerToWorld(entityEngineSystem, world, spawnPoint);

            return world;
        }

        private static MapCoordinate CreateInitialMapAndGetSpawnPoint(string seed, IEntityEngineSystem entityEngineSystem, WorldState world)
        {
            var worldStructure = entityEngineSystem.GetEntityWithName("World").Get<World>();
            var initialBranchEntity = entityEngineSystem.GetEntityWithName(worldStructure.InitialBranch);
                
            var initialBranch = initialBranchEntity.Get<Branch>();

            GenerateBranch(world, initialBranch, entityEngineSystem, seed);

            var initialMap = world.Maps[new MapKey($"{initialBranch.BranchName}:1")];

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

        public static void GenerateBranch(WorldState world, Branch branchDefinition, IEntityEngineSystem entityEngineSystem, string seed)
        {
            var branchGenerator = BranchGenerators.Single(s => s.GenerationType == branchDefinition.GenerationType);

            GeneratedBranch branch = branchGenerator.Generate(branchDefinition, entityEngineSystem, seed);

            foreach (Map map in branch.Maps)
            {
                world.Maps.AddMap(map);
            }
        }

        private static void AddPlayerToWorld(IEntityEngineSystem entityEngineSystem, WorldState world, MapCoordinate spawnPoint)
        {
            var player = EntitySerializer.Deserialize(DataFileLoader.LoadFile(@"Entities\player.edt"), entityEngineSystem);
            player.Get<Position>().MapCoordinate = spawnPoint;

            world.Player = player;
        }

        public static List<IBranchGenerator> BranchGenerators =>
            AppDomain
            .CurrentDomain
            .GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => typeof(IBranchGenerator).IsAssignableFrom(p) && p != typeof(IBranchGenerator))
            .Select(type => (IBranchGenerator)Activator.CreateInstance(type))
            .ToList();
    }
}