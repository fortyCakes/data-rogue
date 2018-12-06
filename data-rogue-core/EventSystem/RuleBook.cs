using System.Collections.Generic;

namespace data_rogue_core.EventSystem
{
    public class RuleBook : Dictionary<EventType, RulePage>
    {

    }

    public class RulePage : List<IEventRule>
    {

    }
}