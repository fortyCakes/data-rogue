using data_rogue_core.Components;
using data_rogue_core.Data;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using System;
using System.Linq;
using data_rogue_core.Activities;
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

            StartGameplayRunning(systemContainer);
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

            SetInitialStats(systemContainer, form, player);

            systemContainer.EntityEngine.AddComponent(player, new HasClass { Class = form.Class });

            LearnClassSkills(systemContainer, form, player);
            GiveStartingGear(systemContainer, form, player);

            systemContainer.PlayerSystem.Player = player;
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
            var playerClass = systemContainer.PrototypeSystem.Get($"Class:{form.Class}");
            return playerClass;
        }

    }
}