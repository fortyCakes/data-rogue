using data_rogue_core.Components;
using data_rogue_core.Data;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using System;
using System.Linq;
using data_rogue_core.Activities;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Forms.StaticForms;
using System.IO;
using System.Collections.Generic;

namespace data_rogue_core
{
    public class WorldGenerator : IWorldGenerator
    {
        private readonly IEntityDataProvider _worldEntityDataProvider;
        private readonly IEntityDataProvider _playerEntityDataProvider;
        private readonly IEntityDataProvider _vaultDataProvider;

        public WorldGenerator(IEntityDataProvider worldEntityDataProvider, IEntityDataProvider playerEntityDataProvider, IEntityDataProvider vaultDataProvider)
        {
            _worldEntityDataProvider = worldEntityDataProvider;
            _vaultDataProvider = vaultDataProvider;
            _playerEntityDataProvider = playerEntityDataProvider;
        }

        public void Create(ISystemContainer systemContainer, CharacterCreationForm characterCreationForm)
        {
            InitialiseBasicSystems(systemContainer);

            LoadEntityData(systemContainer);
            LoadVaultData(systemContainer);

            var spawnPoint = CreateInitialMapAndGetSpawnPoint(systemContainer);

            AddPlayerToWorld(systemContainer, spawnPoint, characterCreationForm);

            StartGameplayRunning(systemContainer);
        }

        private void LoadVaultData(ISystemContainer systemContainer)
        {
            var vaultFiles = _vaultDataProvider.GetData();

            foreach(var vaultFile in vaultFiles)
            {
                var vault = MapSerializer.Deserialize(systemContainer, vaultFile);
                systemContainer.MapSystem.AddVault(vault);
            }

            
        }

        private void LoadEntityData(ISystemContainer systemContainer)
        {
            var worldData = _worldEntityDataProvider.GetData();

            EntitySerializer.DeserializeAll(systemContainer, worldData);
        }

        private static void InitialiseBasicSystems(ISystemContainer systemContainer)
        {
            systemContainer.MessageSystem.Initialise();

            systemContainer.MapSystem.Initialise();

            systemContainer.EntityEngine.Initialise(systemContainer);
        }

        private static void StartGameplayRunning(ISystemContainer systemContainer)
        {
            systemContainer.ActivitySystem.ActivityStack.OfType<GameplayActivity>().Single().Running = true;
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
            var progress = new Progress<string>();

            GeneratedBranch branch = branchGenerator.Generate(systemContainer, branchEntity, progress);

            foreach (Map map in branch.Maps)
            {
                systemContainer.MapSystem.MapCollection.AddMap(map);
            }
        }

        private void AddPlayerToWorld(ISystemContainer systemContainer, MapCoordinate spawnPoint, CharacterCreationForm form)
        {
            var playerData = _playerEntityDataProvider.GetData().Single();

            var player = EntitySerializer.Deserialize(systemContainer, playerData);
            systemContainer.PositionSystem.SetPosition(player, spawnPoint);
            player.Get<Description>().Name = form.Name;

            SetInitialStats(systemContainer, form, player);

            systemContainer.EntityEngine.AddComponent(player, new HasClass { Class = form.Class });

            LearnClassSkills(systemContainer, form, player);
            GiveStartingGear(systemContainer, form, player);

            SetStartDetails(systemContainer, form, player);

            systemContainer.PlayerSystem.Player = player;
        }

        private void SetStartDetails(ISystemContainer systemContainer, CharacterCreationForm form, IEntity player)
        {
            var startDetails = new StartDetails { Class = form.Class, StartTime = DateTime.Now.ToString("f") };
            systemContainer.EntityEngine.AddComponent(player, startDetails);
        }

        private static void SetInitialStats(ISystemContainer systemContainer, CharacterCreationForm form, IEntity player)
        {
            systemContainer.StatSystem.SetStat(player, nameof(form.Muscle), form.Muscle);
            systemContainer.StatSystem.SetStat(player, nameof(form.Agility), form.Agility);
            systemContainer.StatSystem.SetStat(player, nameof(form.Intellect), form.Intellect);
            systemContainer.StatSystem.SetStat(player, nameof(form.Willpower), form.Willpower);
        }

        private static void LearnClassSkills(ISystemContainer systemContainer, CharacterCreationForm form, IEntity player)
        {
            IEntity playerClass = GetPlayerClass(systemContainer, form);
            var skills = playerClass.Components.OfType<KnownSkill>().Select(systemContainer.SkillSystem.GetSkillFromKnown);

            foreach (var skill in skills)
            {
                systemContainer.SkillSystem.Learn(player, skill);
            }
        }

        private static void GiveStartingGear(ISystemContainer systemContainer, CharacterCreationForm form, IEntity player)
        {
            IEntity playerClass = GetPlayerClass(systemContainer, form);
            var startingGear = playerClass.Components.OfType<StartsWithItem>();
            var spawnPoint = systemContainer.PositionSystem.CoordinateOf(player);

            foreach (var startingItem in startingGear)
            {
                var item = systemContainer.PrototypeSystem.CreateAt(startingItem.Item, spawnPoint);

                var inventory = player.Get<Inventory>();
                systemContainer.ItemSystem.MoveToInventory(item, inventory);

                if (startingItem.Equipped)
                {
                    systemContainer.EquipmentSystem.Equip(player, item);
                }
            }
        }

        private static IEntity GetPlayerClass(ISystemContainer systemContainer, CharacterCreationForm form)
        {
            return systemContainer.PrototypeSystem.Get($"Class:{form.Class}");
        }

    }
}