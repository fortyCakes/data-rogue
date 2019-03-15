using System;
using System.Collections.Generic;
using System.Linq;
using data_rogue_core.EventSystem;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core
{
    public class EventRuleFactory
    {
        internal static IEventRule[] CreateRules(ISystemContainer systemContainer, List<Type> eventRules)
        {
            var cparams = new Type[] { typeof(ISystemContainer) };

            var rules = eventRules
                .Select(T => T.GetConstructor(cparams)
                .Invoke(new[] { systemContainer }))
                .Cast<IEventRule>();

            return rules.ToArray();
        }
    }
}