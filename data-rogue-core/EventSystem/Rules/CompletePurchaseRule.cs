using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{
    public class CompletePurchaseRule : IEventRule
    {
        private readonly ISystemContainer _systemContainer;

        public CompletePurchaseRule(ISystemContainer systemContainer)
        {
            _systemContainer = systemContainer;
        }

        public EventTypeList EventTypes => new EventTypeList { EventType.PurchaseItem };
        public EventRuleType RuleType => EventRuleType.EventResolution;
        public uint RuleOrder => 0;
        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            var data = eventData as PurchaseItemEventData;
            var price = data.PurchaseItem.Get<Price>();

            _systemContainer.ItemSystem.RemoveWealth(sender, price.Currency, price.Amount);

            _systemContainer.ItemSystem.MoveToInventory(data.PurchaseItem, sender.Get<Inventory>());

            return true;
        }
    }
}