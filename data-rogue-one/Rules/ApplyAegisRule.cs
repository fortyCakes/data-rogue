using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Utils;
using System;
using System.Collections.Generic;

namespace data_rogue_core.EventSystem.Rules
{

    public class ApplyAegisRule : IEventRule
    {
        private ISystemContainer systemContainer;

        public ApplyAegisRule(ISystemContainer systemContainer)
        {
            this.systemContainer = systemContainer;
        }

        public EventTypeList EventTypes => new EventTypeList { EventType.Damage };

        public uint RuleOrder => 100;

        public EventRuleType RuleType => EventRuleType.BeforeEvent;

        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            var data = eventData as DamageEventData;

            var maxAegis = systemContainer.EventSystem.GetStat(sender, "Aegis");

            int currentAegis = (int)systemContainer.EventSystem.GetStat(sender, "CurrentAegisLevel");

            if (currentAegis > 0)
            {
                var damageAbsorbed = Math.Min(currentAegis, data.Damage);

                currentAegis -= damageAbsorbed;
                systemContainer.StatSystem.SetStat(sender, "CurrentAegisLevel", currentAegis);

                data.Damage -= damageAbsorbed;

                if (data.Damage == 0)
                {
                    data.Absorbed = true;
                    return false;
                }
            }

            return true;
        }
    }
}
