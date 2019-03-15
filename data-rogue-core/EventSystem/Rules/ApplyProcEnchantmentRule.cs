using System;
using System.Linq;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{
    public class ApplyProcEnchantmentRule : IEventRule
    {
        public ApplyProcEnchantmentRule(ISystemContainer systemContainer, EventType eventType)
        {
            this.systemContainer = systemContainer;
            this.eventType = eventType;
        }

        public EventType eventType;

        public EventTypeList EventTypes => new EventTypeList{ eventType };
        public int RuleOrder => int.MinValue;

        private ISystemContainer systemContainer { get; }

        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            if (!sender.Has<Equipped>()) return true;

            var equipped = sender.Get<Equipped>();

            foreach(var equippedItem in equipped.EquippedItems)
            {
                var id = equippedItem.EquipmentId;
                var item = systemContainer.EntityEngine.Get(id);

                foreach(var enchantment in item.Components.OfType<ProcEnchantment>())
                {
                    if (enchantment.EventType == eventType && ProcRoll(enchantment))
                    {
                        systemContainer.ScriptExecutor.ExecuteByName(sender, enchantment.ScriptName, item);
                    }
                }
            }

            return true;
        }

        private bool ProcRoll(ProcEnchantment enchantment)
        {
            return enchantment.ProcChance >= 100 || systemContainer.Random.Between(1, 100) <= enchantment.ProcChance;
        }
    }
}
