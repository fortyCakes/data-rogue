﻿using data_rogue_core.Components;
using data_rogue_core.Data;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using System;
using System.Linq;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Forms.StaticForms;

namespace data_rogue_core
{
    public class WorldGenerator : IWorldGenerator
    {
        private readonly IEntityDataProvider worldEntityDataProvider;
        private readonly IEntityDataProvider playerEntityDataProvider;

        public WorldGenerator(IEntityDataProvider worldEntityDataProvider, IEntityDataProvider playerEntityDataProvider)
        {
            this.worldEntityDataProvider = worldEntityDataProvider;
            this.playerEntityDataProvider = playerEntityDataProvider;
        }

        public void Create(ISystemContainer systemContainer, CharacterCreationForm characterCreationForm)
        {
            systemContainer.MessageSystem.Initialise();

            systemContainer.MapSystem.Initialise();

            systemContainer.EntityEngine.Initialise(systemContainer);

            var worldData = worldEntityDataProvider.GetData();

            EntitySerializer.DeserializeAll(systemContainer, worldData);

            var spawnPoint = CreateInitialMapAndGetSpawnPoint(systemContainer);

            AddPlayerToWorld(systemContainer, spawnPoint, characterCreationForm);
        }

        private static MapCoordinate CreateInitialMapAndGetSpawnPoint(ISystemContainer systemContainer)
        {
            var worldStructure = systemContainer.PrototypeSystem.Get("World").Get<Components.World>();
            var initialBranchEntity = systemContainer.PrototypeSystem.Get(worldStructure.InitialBranch);

            GenerateBranch(systemContainer, initialBranchEntity);

            var initialMap = systemContainer.MapSystem.MapCollection[new MapKey($"{initialBranchEntity.Get<Branch>().BranchName}:1")];

            return GetSpawnPoint(initialMap);
        }

        private static MapCoordinate GetSpawnPoint(IMap initialMap)
        {
            var definedSpawnPoint = initialMap.Cells.Where(c => c.Value.Has<SpawnPoint>());

            if (definedSpawnPoint.Count() == 0)
            {
                throw new ApplicationException("No SpawnPoint on initial map.");
            }

            return definedSpawnPoint.First().Key;
        }

        public static void GenerateBranch(ISystemContainer systemContainer, IEntity branchEntity)
        {
            var branchGenerator = new BranchGenerator();

            GeneratedBranch branch = branchGenerator.Generate(systemContainer, branchEntity);

            foreach (Map map in branch.Maps)
            {
                systemContainer.MapSystem.MapCollection.AddMap(map);
            }
        }

        private void AddPlayerToWorld(ISystemContainer systemContainer, MapCoordinate spawnPoint, CharacterCreationForm form)
        {
            var playerData = playerEntityDataProvider.GetData().Single();

            var player = EntitySerializer.Deserialize(systemContainer, playerData);
            systemContainer.PositionSystem.SetPosition(player, spawnPoint);
            player.Get<Description>().Name = form.Name;

            systemContainer.StatSystem.SetStat(player, nameof(form.Muscle), form.Muscle);
            systemContainer.StatSystem.SetStat(player, nameof(form.Agility), form.Agility);
            systemContainer.StatSystem.SetStat(player, nameof(form.Intellect), form.Intellect);
            systemContainer.StatSystem.SetStat(player, nameof(form.Willpower), form.Willpower);


            systemContainer.PlayerSystem.Player = player;
        }
    }
}