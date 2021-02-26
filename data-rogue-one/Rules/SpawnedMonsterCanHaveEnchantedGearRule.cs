using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem;
using data_rogue_core.Systems.Interfaces;
using data_rogue_one.Utils;
using data_rogue_one.World.GenerationStrategies.ItemGeneration;
using System.Collections.Generic;

namespace data_rogue_one.Rules
{
    public class SpawnedMonsterCanHaveEnchantedGearRule : IEventRule
    {
        private ISystemContainer systemContainer;

        public SpawnedMonsterCanHaveEnchantedGearRule(ISystemContainer systemContainer)
        {
            this.systemContainer = systemContainer;
        }
        public EventTypeList EventTypes => new EventTypeList { EventType.SpawnEntity };

        public uint RuleOrder => 0;
        public EventRuleType RuleType => EventRuleType.AfterSuccess;
        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            if (sender.Has<Equipped>() && sender.Has<Challenge>())
            {
                var equippedItems = systemContainer.EquipmentSystem.GetEquippedItems(sender);
                var level = sender.Get<Challenge>().ChallengeRating;

                var gen = new EnchantedItemGenerator(systemContainer, new List<IEntity>());

                foreach(var item in equippedItems)
                {
                    var rarity = systemContainer.Random.PickOne(RarityPicker.RarityPickers);

                    gen.Enchant(item, ItemClassHelper.Classify(item), level, systemContainer.Random, rarity);
                }
            }

            return true;
        }
    }
}