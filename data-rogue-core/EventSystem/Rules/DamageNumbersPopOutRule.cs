using data_rogue_core.Components;
using data_rogue_core.Data;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;
using System.Collections.Generic;
using System.Drawing;

namespace data_rogue_core.EventSystem.Rules
{
    public class DamageNumbersPopOutRule : IEventRule
    {
        private ISystemContainer _systemContainer;

        public DamageNumbersPopOutRule(ISystemContainer systemContainer)
        {
            _systemContainer = systemContainer;
        }

        public EventTypeList EventTypes => new EventTypeList { EventType.Damage };
        public uint RuleOrder => 0;
        public EventRuleType RuleType => EventRuleType.AfterSuccess;

        private IEventSystem EventRuleSystem { get; }
        public IMessageSystem MessageSystem { get; }

        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            var location = sender.TryGet<Position>();
            var data = eventData as DamageEventData;

            _systemContainer.ParticleSystem.CreateTextParticle(location.MapCoordinate, new List<AnimationMovement> {
                new AnimationMovement(new VectorDouble(0.5, 0.5), 1),
                new AnimationMovement(new VectorDouble(0, -1),500),
                new AnimationMovement(new VectorDouble(0, 0),500)
               }, data.Damage.ToString(), Color.Red);

            return true;
        }
    }
}
