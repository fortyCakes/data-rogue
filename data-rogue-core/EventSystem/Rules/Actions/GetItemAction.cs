using System.Linq;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{

    public class GetItemAction : ApplyActionRule
    {
        public GetItemAction(ISystemContainer systemContainer) : base(systemContainer)
        {
        }

        public override ActionType actionType => ActionType.GetItem;

        public override bool ApplyInternal(IEntity sender, ActionEventData eventData)
        {
            var senderMapCoordinate = _systemContainer.PositionSystem.CoordinateOf(sender);

            var items = _systemContainer.PositionSystem.EntitiesAt(senderMapCoordinate)
                .Except(new[] { sender })
                .Where(e => e.Has<Item>() || e.Has<Wealth>())
                .OrderBy(e => e.EntityId);

            var firstItem = items.FirstOrDefault();

            if (firstItem != null)
            {
                if (firstItem.Has<Wealth>())
                {
                    var wealth = firstItem.Get<Wealth>();

                    var wealthEventData = new PickupWealthEventData { Item = firstItem };

                    var ok = _systemContainer.EventSystem.Try(EventType.PickUpWealth, sender, wealthEventData);

                    if (ok)
                    {
                        _systemContainer.ItemSystem.TransferWealth(firstItem, sender, wealth.Currency, wealth.Amount);

                        _systemContainer.EntityEngine.Destroy(firstItem);
                    }
                }
                else
                {
                    var itemEventData = new PickupItemEventData { Item = firstItem };

                    var ok = _systemContainer.EventSystem.Try(EventType.PickUpItem, sender, itemEventData);

                    if (ok)
                    {
                        var inventory = sender.Get<Inventory>();

                        var done = _systemContainer.ItemSystem.MoveToInventory(firstItem, inventory);

                        if (done)
                        {
                            _systemContainer.MessageSystem.Write($"{sender.DescriptionName} picks up the {firstItem.Get<Description>().Name}.");
                        }
                    }
                }

                return true;
            }

            return false;
        }
    }
}
