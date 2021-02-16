using System.Linq;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{
    public class ApplyStatBoostEnchantmentRule : IEventRule
    {
        public ApplyStatBoostEnchantmentRule(ISystemContainer systemContainer)
        {
            this.systemContainer = systemContainer;
        }

        public EventTypeList EventTypes => new EventTypeList{ EventType.GetStat };
        public uint RuleOrder => 1;
        public EventRuleType RuleType => EventRuleType.BeforeEvent;

        private ISystemContainer systemContainer { get; }

        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            var data = eventData as GetStatEventData;

            if (!sender.Has<Equipped>()) return true;

            var equipped = sender.Get<Equipped>();

            foreach(var equippedItem in equipped.EquippedItems.ToList())
            {
                var id = equippedItem.EquipmentId;
                var item = systemContainer.EntityEngine.Get(id);

                foreach(var boost in item.Components.OfType<StatBoostEnchantment>())
                {
                    if (boost.Stat == data.Stat)
                    {
                        data.Value += boost.Value;
                    }
                }
            }

            return true;
        }
    }
}
