using System.Linq;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{
    public class ApplyResistanceRule : IEventRule
    {
        public ApplyResistanceRule(ISystemContainer systemContainer)
        {
            EventRuleSystem = systemContainer.EventSystem;
            MessageSystem = systemContainer.MessageSystem;
        }

        public EventTypeList EventTypes => new EventTypeList {EventType.Attack};
        public uint RuleOrder => 0;
        public EventRuleType RuleType => EventRuleType.BeforeEvent;

        private IEventSystem EventRuleSystem { get; }
        public IMessageSystem MessageSystem { get; }

        public bool Apply(EventType type, IEntity sender, object eventData)
        {

            var data = eventData as AttackEventData;

            var resistances = data.Defender.Components.OfType<Resistant>();

            var applicable = resistances.Where(r => data?.Tags?.Contains(r.ResistantTo) ?? false);

            foreach (var resistance in applicable)
            {
                data.Damage -= resistance.Power;
                if (data.Damage <= 0)
                {
                    data.Damage = 0;
                    data.SuccessfulDefenceType = "Resistance";
                    return false;
                }
            }

            return true;
        }

    }
}