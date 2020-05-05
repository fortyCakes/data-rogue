using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{
    public class ApplyEquipmentStatsRule : IEventRule
    {
        public ApplyEquipmentStatsRule(ISystemContainer systemContainer)
        {
            this.systemContainer = systemContainer;
        }

        public EventTypeList EventTypes => new EventTypeList{ EventType.GetStat };
        public uint RuleOrder => 1;
        public EventRuleType RuleType => EventRuleType.EventResolution;

        private ISystemContainer systemContainer { get; }

        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            var data = eventData as GetStatEventData;

            if (!sender.Has<Equipped>()) return true;

            var equipped = sender.Get<Equipped>();

            foreach(var equippedItem in equipped.EquippedItems)
            {
                var item = systemContainer.EntityEngine.Get(equippedItem.EquipmentId);

                data.Value += systemContainer.StatSystem.GetEntityStat(item, data.Stat);
            }

            return true;
        }
    }
}
