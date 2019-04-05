using System.Collections.Generic;
using System.Linq;
using data_rogue_core.EntityEngineSystem;

namespace data_rogue_core.EventSystem
{
    public class EventSystem : IEventSystem
    {
        private RuleBook RuleBook;

        public bool Try(EventType eventType, IEntity sender, object eventData)
        {
            bool eventSuccess = true;

            if (RuleBook.ContainsKey(eventType))
            {
                var rulePage = RuleBook[eventType];

                var beforeEventRules = rulePage[EventRuleType.BeforeEvent];

                eventSuccess = RunRules(eventType, sender, eventData, beforeEventRules, true);

                if (eventSuccess)
                {
                    var resolutionRules = rulePage[EventRuleType.EventResolution];
                    RunRules(eventType, sender, eventData, resolutionRules, false);

                    var afterSuccessRules = rulePage[EventRuleType.AfterSuccess];
                    RunRules(eventType, sender, eventData, afterSuccessRules, false);
                }
                else
                {
                    var afterFailureRules = rulePage[EventRuleType.AfterFailure];
                    RunRules(eventType, sender, eventData, afterFailureRules, false);
                }

                var finallyRules = rulePage[EventRuleType.Finally];

                RunRules(eventType, sender, eventData, finallyRules, false);
            }

            return eventSuccess;
        }

        private static bool RunRules(EventType eventType, IEntity sender, object eventData, List<IEventRule> beforeEventRules, bool breakOnRuleFailure)
        {
            bool eventSuccess = true;

            foreach (var rule in beforeEventRules.OrderByDescending(rule => rule.RuleOrder))
            {
                var canContinue = rule.Apply(eventType, sender, eventData);

                if (!canContinue)
                {
                    eventSuccess = false;
                    if (breakOnRuleFailure)
                    {
                        break;
                    }
                }
            }

            return eventSuccess;
        }

        public void RegisterRule(IEventRule eventRule)
        {
            foreach (EventType eventType in eventRule.EventTypes)
            {
                if (RuleBook.ContainsKey(eventType))
                {
                    var page = RuleBook[eventType];

                    if (!page[eventRule.RuleType].Contains(eventRule))
                    {
                        page[eventRule.RuleType].Add(eventRule);
                    }
                }
                else
                {
                    var page = new RulePage();
                    RuleBook.Add(eventType, page);
                    page[eventRule.RuleType].Add(eventRule);
                }
            }
        }

        public void RegisterRules(params IEventRule[] rules)
        {
            foreach (var rule in rules)
            {
                RegisterRule(rule);
            }
        }

        public void Initialise()
        {
            RuleBook = new RuleBook();
        }
    }

    
}
