using System;
using System.Collections.Generic;
using data_rogue_core.Activities;
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
            typeof(ResolveMeleeAttackAction),
            typeof(StartRangedAttackAction),
            typeof(ResolveRangedAttackAction),
            typeof(SaveAction),
            typeof(EnterAction),
            typeof(EquipmentMenuAction),
            typeof(InventoryMenuAction),
            typeof(DebugMenuAction),
            typeof(GetItemAction),
            typeof(SkillMenuAction),
            typeof(UseSkillAction),
            typeof(FollowPathAction),
            typeof(WaitAction),
            typeof(InteractAction),
            typeof(NextInteractionAction),
            typeof(HotbarAction),
            typeof(ChangeMapEditorToolAction),
            typeof(EscapeMenuAction),
            typeof(EscapeGameplayAction)
        };

        protected ISystemContainer _systemContainer;

        public ApplyActionRule(ISystemContainer systemContainer)
        {
            _systemContainer = systemContainer;
        }

        public EventTypeList EventTypes => new EventTypeList { EventType.Action };

        public abstract ActionType actionType { get; }
        public abstract ActivityType activityType { get; }

        public uint RuleOrder => 0;
        public EventRuleType RuleType => EventRuleType.EventResolution;

        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            var data = eventData as ActionEventData;
            var currentActivityType = _systemContainer.ActivitySystem.Peek().Type;

            if (data.Action == actionType && activityType == currentActivityType)
            {
                return ApplyInternal(sender, data);
            }

            return true;
        }

        public abstract bool ApplyInternal(IEntity sender, ActionEventData eventData);
    }
}
