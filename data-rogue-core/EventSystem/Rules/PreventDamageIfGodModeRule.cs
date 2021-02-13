using System;
using System.Drawing;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{

    public class PreventDamageIfGodModeRule : IEventRule
    {
        private IMessageSystem _messageSystem;

        public PreventDamageIfGodModeRule(ISystemContainer systemContainer)
        {
            _messageSystem = systemContainer.MessageSystem;
        }

        public EventTypeList EventTypes => new EventTypeList { EventType.Damage };
        public uint RuleOrder => 0;
        public EventRuleType RuleType => EventRuleType.BeforeEvent;
        

        public bool Apply(EventType type, IEntity sender, object eventData)
        {

            if (sender.Has<GodMode>())
            {
                var data = eventData as DamageEventData;
                _messageSystem.Write($"GodMode prevents {data.Damage} damage.");
                return false;
            }

            return true;
        }
    }
}
