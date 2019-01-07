using System.Linq;
using data_rogue_core.Components;
using data_rogue_core.EntityEngine;
using data_rogue_core.Maps;
using data_rogue_core.Systems;

namespace data_rogue_core.EventSystem.Rules
{
    class AlwaysHitAttackRule : IEventRule
    {
        public AlwaysHitAttackRule(IEntityEngine engine)
        {
            EntityEngine = engine;
        }

        public EventTypeList EventTypes => new EventTypeList{ EventType.Attack };
        public int RuleOrder => 0;

        private IEntityEngine EntityEngine { get; }

        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            return true;
        }
    }
}
