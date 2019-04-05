using System;
using System.Collections.Generic;

namespace data_rogue_core.EventSystem
{
    public class RuleBook : Dictionary<EventType, RulePage>
    {

    }

    public class RulePage : Dictionary<EventRuleType , List<IEventRule>>
    {
        public RulePage()
        {
            foreach (EventRuleType ruleType in Enum.GetValues(typeof(EventRuleType)))
            {
                Add(ruleType, new List<IEventRule>());
            }
        }
    }
}