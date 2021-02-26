using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{
    public class PeopleUnequipTheirStuffWhenTheyAreKilledRule : IEventRule
    {
        public PeopleUnequipTheirStuffWhenTheyAreKilledRule(ISystemContainer systemContainer)
        {
            EntityEngine = systemContainer.EntityEngine;
            MessageSystem = systemContainer.MessageSystem;
            PlayerSystem = systemContainer.PlayerSystem;
            ItemSystem = systemContainer.ItemSystem;
            EquipmentSystem = systemContainer.EquipmentSystem;
        }

        public EventTypeList EventTypes => new EventTypeList { EventType.Death };
        public uint RuleOrder => 20;

        public EventRuleType RuleType => EventRuleType.EventResolution;

        private IEntityEngine EntityEngine { get; }
        public IMessageSystem MessageSystem { get; }
        public IPlayerSystem PlayerSystem { get; }
        public IItemSystem ItemSystem { get; }
        public IEquipmentSystem EquipmentSystem { get; }

        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            var equipped = EquipmentSystem.GetEquippedItems(sender);
            foreach (var item in equipped)
            {
                EquipmentSystem.Unequip(sender, item);
            }

            return true;
        }
    }
}
