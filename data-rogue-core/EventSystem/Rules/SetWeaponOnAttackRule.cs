using System.Linq;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{
    public class SetWeaponOnAttackRule : IEventRule
    {
        private ISystemContainer _systemContainer;

        public SetWeaponOnAttackRule(ISystemContainer systemContainer)
        {
            _systemContainer = systemContainer;
        }

        public EventTypeList EventTypes => new EventTypeList { EventType.Attack };
        public uint RuleOrder => 1000;
        public EventRuleType RuleType => EventRuleType.BeforeEvent;

        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            var data = eventData as AttackEventData;

            if (data.Weapon == null)
            {
                data.Weapon = GetWeapon(sender);
            }

            return true;
        }

        private IEntity GetWeapon(IEntity sender)
        {
            if (sender.Has<Equipped>())
            {
                var equipment = sender.Get<Equipped>().EquippedItems;

                var weapons = equipment
                    .Select(e => _systemContainer.EntityEngine.Get(e.EquipmentId))
                    .Where(e => e.Has<Weapon>())
                    .ToList();

                if (!weapons.Any()) return Unarmed();

                return _systemContainer.Random.PickOne(weapons);
            }
            else
            {
                return Unarmed();
            }
        }

        private IEntity Unarmed()
        {
            return _systemContainer.PrototypeSystem.Get("Item:UnarmedWeapon");
        }
    }
}
