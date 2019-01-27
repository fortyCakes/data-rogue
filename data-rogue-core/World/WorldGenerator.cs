using data_rogue_core.Components;
using data_rogue_core.Data;
using data_rogue_core.EntityEngine;
using data_rogue_core.Maps;
using System;
using System.Linq;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Forms;
using data_rogue_core.Menus.StaticMenus;
using data_rogue_core.Forms.StaticForms;
using data_rogue_core.Behaviours;

namespace data_rogue_core
{
    public class WorldGenerator
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
            var worldStructure = systemContainer.PrototypeSystem.Create("World").Get<World>();
            var initialBranchEntity = systemContainer.PrototypeSystem.Create(worldStructure.InitialBranch);
                
            var initialBranch = initialBranchEntity.Get<Branch>();

            GenerateBranch(systemContainer, world, initialBranch);

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

        public static void GenerateBranch(ISystemContainer systemContainer, WorldState world, Branch branchDefinition)
        {
            var branchGenerator = BranchGeneratorFactory.GetGenerator(branchDefinition.GenerationType);

            GeneratedBranch branch = branchGenerator.Generate(systemContainer, branchDefinition);

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

            world.Player = player;
        }
    }
}