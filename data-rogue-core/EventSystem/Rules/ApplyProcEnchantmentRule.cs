using System.Linq;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{
    public abstract class ApplyProcEnchantmentRule : IEventRule
    {
        public ApplyProcEnchantmentRule(ISystemContainer systemContainer)
        {
            _systemContainer = systemContainer;
        }

        public abstract EventType EventType { get; }

        public EventTypeList EventTypes => new EventTypeList{ EventType };
        public int RuleOrder => int.MinValue;

        private ISystemContainer _systemContainer { get; }

        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            if (!sender.Has<Equipped>()) return true;

            var equipped = sender.Get<Equipped>();

            foreach(var equippedItem in equipped.EquippedItems)
            {
                var id = equippedItem.EquipmentId;
                var item = _systemContainer.EntityEngine.Get(id);

                foreach(var enchantment in item.Components.OfType<ProcEnchantment>())
                {
                    if (enchantment.EventType == EventType && ProcRoll(enchantment))
                    {
                        _systemContainer.ScriptExecutor.ExecuteByName(sender, enchantment.ScriptName, item);
                    }
                }
            }

            return true;
        }

        private bool ProcRoll(ProcEnchantment enchantment)
        {
            return enchantment.ProcChance >= 100 || _systemContainer.Random.Between(1, 100) <= enchantment.ProcChance;
        }
    }
}
