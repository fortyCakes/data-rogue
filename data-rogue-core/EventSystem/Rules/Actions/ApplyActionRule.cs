using System;
using System.Collections.Generic;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{
    public abstract class ApplyActionRule : IEventRule
    {
        public static List<Type> AllActionRules = new List<Type>
        {
            typeof(MoveAction),
            typeof(MeleeAttackAction),
            typeof(RangedAttackAction),
            typeof(SaveAction),
            typeof(EnterAction),
            typeof(RestAction),
            typeof(EquipmentMenuAction),
            typeof(InventoryMenuAction),
            typeof(GetItemAction),
            typeof(SkillMenuAction),
            typeof(UseSkillAction)
        };

        protected ISystemContainer _systemContainer;

        public ApplyActionRule(ISystemContainer systemContainer)
        {
            _systemContainer = systemContainer;
        }

        public EventTypeList EventTypes => new EventTypeList { EventType.Action };

        public abstract ActionType actionType { get; }

        public int RuleOrder => 0;

        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            var data = eventData as ActionEventData;

            if (data.Action == actionType)
            {
                return ApplyInternal(sender, data);
            }

            return true;
        }

        public abstract bool ApplyInternal(IEntity sender, ActionEventData eventData);
    }
}
