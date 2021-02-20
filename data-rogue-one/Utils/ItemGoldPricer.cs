using System;
using System.Linq;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Systems.Interfaces;
using data_rogue_one.Components;

namespace data_rogue_one.Utils
{
    internal class ItemGoldPricer : IItemPricer
    {
        public virtual string Currency => "Gold";

        private ISystemContainer _systemContainer;

        public ItemGoldPricer(ISystemContainer systemContainer)
        {
            _systemContainer = systemContainer;
        }

        public Price Price(IEntity item)
        {
            var amount = 0;

            if (item.Has<Price>())
            {
                var itemPrice = item.Get<Price>();
                if (itemPrice.Currency == Currency)
                {
                    amount = itemPrice.Amount;
                }
            }

            var premium = 1.0;
            foreach(var enchantment in item.Components.OfType<EnchantmentGeneration>().OrderBy(e=>e.GoldValue))
            {
                amount += (int)(enchantment.GoldValue * premium);

                premium += 0.1;
            }


            return new Price { Currency = Currency, Amount = amount };
        }
    }
}