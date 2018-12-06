using System.Linq;
using data_rogue_core.EntitySystem;

namespace data_rogue_core.EventSystem
{
    public class EventRuleSystem : IEventRuleSystem
    {
        private RuleBook RuleBook;

        public bool Try(EventType eventType, IEntity sender, object eventData)
        {
            if (RuleBook.ContainsKey(eventType))
            {
                foreach (var rule in RuleBook[eventType].OrderByDescending(rule => rule.RuleOrder))
                {
                    var canContinue = rule.Apply(eventType, sender, eventData);

                    if (!canContinue) return false;
                }
            }

            return true;
        }

        public void RegisterRule(IEventRule eventRule)
        {
            foreach (EventType type in eventRule.EventTypes)
            {
                if (RuleBook.ContainsKey(type))
                {
                    if (!RuleBook[type].Contains(eventRule))
                    {
                        RuleBook[type].Add(eventRule);
                    }
                }
                else
                {
                    RuleBook.Add(type, new RulePage { eventRule });
                }
            }
        }

        public void Initialise()
        {
            RuleBook = new RuleBook();
        }
    }

    
}
