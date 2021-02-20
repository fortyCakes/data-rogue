using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{

    public class CantAffordItemRule : IEventRule
    {
        private readonly ISystemContainer _systemContainer;

        public CantAffordItemRule(ISystemContainer systemContainer)
        {
            _systemContainer = systemContainer;
        }

        public EventTypeList EventTypes => new EventTypeList { EventType.PurchaseItem };
        public EventRuleType RuleType => EventRuleType.BeforeEvent;
        public uint RuleOrder => 0;
        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            var data = eventData as PurchaseItemEventData;
            var price = data.PurchaseItem.Get<Price>();
            var wealth = _systemContainer.ItemSystem.CheckWealth(sender, price.Currency);

            if (wealth <= price.Amount)
            {
                return false;
            }

            return true;
        }
    }
}