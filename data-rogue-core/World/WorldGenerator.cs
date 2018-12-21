using data_rogue_core.Components;
using data_rogue_core.Data;
using data_rogue_core.EntityEngine;
using data_rogue_core.Maps;
using System;
using System.Linq;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core
{
    public class WorldGenerator
    {
        public static WorldState Create(string seed, IEntityEngine entityEngineSystem, IPositionSystem positionSystem, IPrototypeSystem prototypeSystem)
        {
            entityEngineSystem.Initialise();

            var world = new WorldState(entityEngineSystem, seed);

            (new WorldEntityLoader()).Load(entityEngineSystem);

            var spawnPoint = CreateInitialMapAndGetSpawnPoint(seed, entityEngineSystem, positionSystem, prototypeSystem, world);

            AddPlayerToWorld(entityEngineSystem, world, spawnPoint);

            return world;
        }

        private static MapCoordinate CreateInitialMapAndGetSpawnPoint(string seed, IEntityEngine entityEngineSystem, IPositionSystem positionSystem, IPrototypeSystem prototypeSystem, WorldState world)
        {
            var worldStructure = prototypeSystem.Create("World").Get<World>();
            var initialBranchEntity = prototypeSystem.Create(worldStructure.InitialBranch);
                
            var initialBranch = initialBranchEntity.Get<Branch>();

            GenerateBranch(world, initialBranch, entityEngineSystem, positionSystem, prototypeSystem, seed);

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

        public static void GenerateBranch(WorldState world, Branch branchDefinition, IEntityEngine entityEngineSystem, IPositionSystem positionSystem, IPrototypeSystem prototypeSystem, string seed)
        {
            var branchGenerator = BranchGeneratorFactory.GetGenerator(branchDefinition.GenerationType);

            GeneratedBranch branch = branchGenerator.Generate(branchDefinition, entityEngineSystem, positionSystem, prototypeSystem, seed);

            foreach (Map map in branch.Maps)
            {
                world.Maps.AddMap(map);
            }
        }

        private static void AddPlayerToWorld(IEntityEngine entityEngineSystem, WorldState world, MapCoordinate spawnPoint)
        {
            var player = EntitySerializer.Deserialize(DataFileLoader.LoadFile(@"Entities\player.edt"), entityEngineSystem);
            player.Get<Position>().MapCoordinate = spawnPoint;

            world.Player = player;
        }
    }
}